using Feign.Internal;
using Feign.Logging;
using Feign.Pipeline.Internal;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    /// <summary>
    /// FeignHttpClient使用的消息处理程序
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class FeignProxyHttpClientHandler<TService> : HttpClientHandler where TService : class
    {
        private readonly ILogger _logger;
        private FeignClientHttpProxy<TService> _feignClient;

        public FeignClientHttpProxy<TService> FeignClient => _feignClient;

        //IFeignClient IFeignHttpClientHandler.FeignClient => _feignClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeignProxyHttpClientHandler{T}"/> class.
        /// </summary>
        /// <param name="feignClient"></param>
        /// <param name="logger"></param>
        public FeignProxyHttpClientHandler(FeignClientHttpProxy<TService> feignClient, ILogger logger)
        {
            _feignClient = feignClient;
            _logger = logger;
        }
        /// <summary>
        /// 查找服务路径
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        protected virtual Uri LookupRequestUri(FeignHttpRequestMessage requestMessage)
        {
            return requestMessage.RequestUri;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            FeignHttpRequestMessage feignRequest = request as FeignHttpRequestMessage;
            return SendInternalAsync(feignRequest, cancellationToken);
        }


        private async Task<HttpResponseMessage> SendInternalAsync(FeignHttpRequestMessage request, CancellationToken cancellationToken)
        {
            var current = request.RequestUri;
            CancellationTokenSource cts = null;
            try
            {

                #region BuildingRequest
                var buildingContext = new BuildingRequestPipelineContext<TService>(_feignClient, request.Method.ToString(), request.RequestUri, new Dictionary<string, string>(), request.FeignClientRequest);
                await _feignClient.OnBuildingRequestAsync(buildingContext)
#if CONFIGUREAWAIT_FALSE
                        .ConfigureAwait(false)
#endif
                      ;
                //request.Method = new HttpMethod(buildingArgs.Method);
                request.RequestUri = buildingContext.RequestUri;
                if (buildingContext.Headers != null && buildingContext.Headers.Count > 0)
                {
                    foreach (var item in buildingContext.Headers)
                    {
                        request.Headers.TryAddWithoutValidation(item.Key, item.Value);
                    }
                }
                #endregion
                request.RequestUri = LookupRequestUri(request);
                #region SendingRequest
                var sendingContext = new SendingRequestPipelineContext<TService>(_feignClient, request, cancellationToken);
                await _feignClient.OnSendingRequestAsync(sendingContext)
#if CONFIGUREAWAIT_FALSE
                            .ConfigureAwait(false)
#endif
                        ;

                if (sendingContext.IsTerminated)
                {
                    //请求被终止
                    throw new TerminatedRequestException();
                }
                if (sendingContext._cancellationTokenSource != null)
                {
                    cts = sendingContext._cancellationTokenSource;
                    cancellationToken = cts.Token;
                }
                request = sendingContext.RequestMessage;
                if (request == null)
                {
                    _logger?.LogError($"SendingRequest is null;");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.ExpectationFailed)
                    {
                        Content = new StringContent(""),
                        //Headers = new System.Net.Http.Headers.HttpResponseHeaders(),
                        RequestMessage = request
                    };
                }
                #endregion

                #region CannelRequest
                var cancelContext = new CancelRequestPipelineContext<TService>(_feignClient, request, cancellationToken);
                await _feignClient.OnCancelRequestAsync(cancelContext)
#if CONFIGUREAWAIT_FALSE
                      .ConfigureAwait(false)
#endif
                      ;
                #endregion

                return await base.SendAsync(request, cancelContext.CancellationToken)
#if CONFIGUREAWAIT_FALSE
           .ConfigureAwait(false)
#endif
                    ;
            }
            catch (Exception e)
            {
                if (!e.IsSkipLog())
                {
                    _logger?.LogError(e, "Exception during SendAsync()");
                }
                if (e is HttpRequestException)
                {
                    FeignHttpRequestException feignHttpRequestException = new FeignHttpRequestException(_feignClient, request, (HttpRequestException)e);
                    ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(feignHttpRequestException);
                    exceptionDispatchInfo.Throw();
                }
                throw;
            }
            finally
            {
                request.RequestUri = current;
                if (cts != null)
                {
                    cts.Dispose();
                }
            }
        }

    }
}
