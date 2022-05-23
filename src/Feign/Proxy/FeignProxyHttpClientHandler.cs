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

                var requestContext = new RequestPipelineContext<TService>(_feignClient, request, cancellationToken);

                #region BuildingRequest                
                await _feignClient.OnBuildingRequestAsync(requestContext)
#if CONFIGUREAWAIT_FALSE
                        .ConfigureAwait(false)
#endif
                      ;
                //request.Method = new HttpMethod(requestContext.Method);
                request.RequestUri = requestContext.RequestUri;
                if (requestContext.Headers != null && requestContext.Headers.Count > 0)
                {
                    foreach (var item in requestContext.Headers)
                    {
                        request.Headers.TryAddWithoutValidation(item.Key, item.Value);
                    }
                }
                #endregion
                request.RequestUri = LookupRequestUri(request);
                #region SendingRequest
                await _feignClient.OnSendingRequestAsync(requestContext)
#if CONFIGUREAWAIT_FALSE
                            .ConfigureAwait(false)
#endif
                        ;

                if (requestContext.IsTerminated)
                {
                    //请求被终止
                    throw new TerminatedRequestException();
                }
                if (requestContext._cancellationTokenSource != null)
                {
                    cts = requestContext._cancellationTokenSource;
                    cancellationToken = cts.Token;
                }
                request = requestContext.RequestMessage;
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
                await _feignClient.OnCancelRequestAsync(requestContext)
#if CONFIGUREAWAIT_FALSE
                      .ConfigureAwait(false)
#endif
                      ;
                #endregion

                return await base.SendAsync(request, requestContext.CancellationToken)
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
