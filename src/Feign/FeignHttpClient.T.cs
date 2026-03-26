using Feign.Cache;
using Feign.Discovery;
using Feign.Discovery.LoadBalancing;
using Feign.Internal;
using Feign.Logging;
using Feign.Pipeline.Internal;
using Feign.Proxy;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feign
{
    public class FeignHttpClient<TService> : FeignHttpClient where TService : class
    {
        public FeignHttpClient(FeignClientHttpProxy<TService> feignClient, HttpMessageHandler handler) : base(handler)
        {
            _feignClient = feignClient;
            ShouldResolveService = _feignClient.Url == null;
            _serviceResolve = _feignClient.Options.Request.LoadBalancingPolicy switch
            {
                LoadBalancingPolicy.FirstAlphabetical => new FirstServiceResolve(_feignClient.Logger),
                LoadBalancingPolicy.Random => new RandomServiceResolve(_feignClient.Logger),
                LoadBalancingPolicy.RoundRobin => new RoundRobinServiceResolve(_feignClient.Logger),
                LoadBalancingPolicy.LeastRequests => new LeastRequestsServiceResolve(_feignClient.Logger),
                LoadBalancingPolicy.PowerOfTwoChoices => new PowerOfTwoChoicesServiceResolve(_feignClient.Logger),
                _ => new RandomServiceResolve(_feignClient.Logger)
            };

        }

        private readonly FeignClientHttpProxy<TService> _feignClient;

        private readonly IServiceResolve _serviceResolve;

        public bool ShouldResolveService { get; }


        public override async Task<HttpResponseMessage> SendAsync(FeignHttpRequestMessage request, HttpCompletionOption completionOption)
        {
            var cancellationToken = CancellationToken.None;
            var current = request.RequestUri;
            CancellationTokenSource? cts = null;
            try
            {

                var requestContext = new RequestPipelineContext<TService>(_feignClient, request, cancellationToken);

                #region BuildingRequest                
                await _feignClient.OnBuildingRequestAsync(requestContext)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
                request.RequestUri = requestContext.RequestUri;
                if (requestContext.Headers != null && requestContext.Headers.Count > 0)
                {
                    foreach (var item in requestContext.Headers)
                    {
                        request.Headers.TryAddWithoutValidation(item.Key, item.Value);
                    }
                }
                #endregion
                request.RequestUri = await LookupRequestUriAsync(request)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;

                #region RequestTransforms
                if (request.FeignClientRequest.RequestTransforms != null && request.FeignClientRequest.RequestTransforms.Count > 0)
                {
                    foreach (var transform in request.FeignClientRequest.RequestTransforms)
                    {
                        await transform.ApplyAsync(request);
                    }
                }
                if (request.ResponseMessage != null)
                {
                    return request.ResponseMessage;
                }
                #endregion

                #region SendingRequest
                await _feignClient.OnSendingRequestAsync(requestContext)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;

                if (requestContext.IsTerminated)
                {
                    //Request terminated
                    throw new TerminatedRequestException();
                }
                if (requestContext._cancellationTokenSource != null)
                {
                    cts = requestContext._cancellationTokenSource;
                    //cancellationToken = cts.Token;
                    requestContext.CancellationToken = cts.Token;
                }
                request = requestContext.RequestMessage;
                if (request == null)
                {
                    _feignClient.Logger.LogError($"SendingRequest is null;");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.ExpectationFailed)
                    {
                        Content = new StringContent(""),
                        //Headers = new System.Net.Http.Headers.HttpResponseHeaders(),
                        RequestMessage = request
                    };
                }
                #endregion

                #region CannelRequest
                await _feignClient.OnCancelRequestAsync(requestContext)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
                #endregion

                request.ResponseMessage = await Channel.SendAsync(request, completionOption, requestContext.CancellationToken)
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;

                return request.ResponseMessage;
            }
            catch (Exception e)
            {
                if (!e.IsSkipLog())
                {
                    _feignClient.Logger.LogError(e, "Exception during SendAsync()");
                }
                if (e is HttpRequestException exception)
                {
                    var feignHttpRequestException = new FeignHttpRequestException(_feignClient, request, exception);
                    ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(feignHttpRequestException);
                    exceptionDispatchInfo.Throw();
                }
                throw;
            }
            finally
            {
                request.RequestUri = current;
                cts?.Dispose();
            }
        }

#if USE_VALUE_TASK
        private async ValueTask<Uri?> LookupRequestUriAsync(FeignHttpRequestMessage requestMessage)
#else
        private async Task<Uri?> LookupRequestUriAsync(FeignHttpRequestMessage requestMessage)
#endif
        {
            if (!ShouldResolveService)
            {
                return requestMessage.RequestUri;
            }
            //if (ServiceDiscovery == null)
            //{
            //    ServiceResolveFail(requestMessage);
            //    return requestMessage.RequestUri;
            //}

            string serviceId = requestMessage.ServiceId ?? requestMessage.RequestUri!.Host;

            var cacheTime = _feignClient.Options.Request.DiscoverServiceCacheTime;
            IList<IServiceInstance>? services = cacheTime.HasValue ?
            await _feignClient.ServiceDiscovery.GetServiceInstancesWithCacheAsync(serviceId, _feignClient.CacheProvider, cacheTime.Value)
#if USE_CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
            : _feignClient.ServiceDiscovery.GetServiceInstances(serviceId);
            if (services == null || services.Count == 0)
            {
                ServiceResolveFail(requestMessage);
            }
            return _serviceResolve.ResolveService(serviceId, requestMessage.RequestUri!, services);
        }

        private static void ServiceResolveFail(FeignHttpRequestMessage requestMessage)
        {
            throw new ServiceResolveFailException($"Resolve service fail : {requestMessage.RequestUri}");
        }


    }
}
