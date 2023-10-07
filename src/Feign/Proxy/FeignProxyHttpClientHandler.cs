using Feign.Internal;
using Feign.Logging;
using Feign.Pipeline.Internal;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
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
        private readonly ILogger? _logger;
        private readonly FeignClientHttpProxy<TService> _feignClient;

        public FeignClientHttpProxy<TService> FeignClient => _feignClient;

#if NETCOREAPP2_1_OR_GREATER

        private static readonly Func<HttpClientHandler, SocketsHttpHandler>? SocketsHttpHandlerGetter = CreateSocketsHttpHandlerGetter();

        private static Func<HttpClientHandler, SocketsHttpHandler>? CreateSocketsHttpHandlerGetter()
        {
            var fieldInfo = typeof(HttpClientHandler).GetField("_underlyingHandler", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo == null || fieldInfo.FieldType != typeof(SocketsHttpHandler))
            {
                return null;
            }
            ParameterExpression instance = Expression.Parameter(typeof(HttpClientHandler));
            Expression body = Expression.Field(instance, fieldInfo);
            return Expression.Lambda<Func<HttpClientHandler, SocketsHttpHandler>>(body, instance).Compile();
        }

        internal SocketsHttpHandler? HttpHandler
                    => SocketsHttpHandlerGetter?.Invoke(this);
#else
        internal HttpClientHandler HttpHandler
            => this;
#endif



        /// <summary>
        /// Initializes a new instance of the <see cref="FeignProxyHttpClientHandler{T}"/> class.
        /// </summary>
        /// <param name="feignClient"></param>
        /// <param name="logger"></param>
        public FeignProxyHttpClientHandler(FeignClientHttpProxy<TService> feignClient, ILogger? logger)
        {
            _feignClient = feignClient;
            _logger = logger;
        }
        /// <summary>
        /// 查找服务路径
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        protected virtual Task<Uri?> LookupRequestUriAsync(FeignHttpRequestMessage requestMessage)
        {
#if NET5_0_OR_GREATER
            return Task.FromResult(requestMessage.RequestUri);
#else
            return Task.FromResult((Uri?)requestMessage.RequestUri);
#endif
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return SendInternalAsync((FeignHttpRequestMessage)request, cancellationToken);
        }


        private async Task<HttpResponseMessage> SendInternalAsync(FeignHttpRequestMessage request, CancellationToken cancellationToken)
        {
            var current = request.RequestUri;
            CancellationTokenSource? cts = null;
            try
            {

                var requestContext = new RequestPipelineContext<TService>(_feignClient, request, cancellationToken);

                #region BuildingRequest                
                await _feignClient.OnBuildingRequestAsync(requestContext).ConfigureAwait(false);
                request.RequestUri = requestContext.RequestUri;
                if (requestContext.Headers != null && requestContext.Headers.Count > 0)
                {
                    foreach (var item in requestContext.Headers)
                    {
                        request.Headers.TryAddWithoutValidation(item.Key, item.Value);
                    }
                }
                #endregion
                request.RequestUri = await LookupRequestUriAsync(request).ConfigureAwait(false);
                #region SendingRequest
                await _feignClient.OnSendingRequestAsync(requestContext).ConfigureAwait(false);

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
                await _feignClient.OnCancelRequestAsync(requestContext).ConfigureAwait(false);
                #endregion

                return await base.SendAsync(request, requestContext.CancellationToken).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                if (!e.IsSkipLog())
                {
                    _logger?.LogError(e, "Exception during SendAsync()");
                }
                if (e is HttpRequestException exception)
                {
                    FeignHttpRequestException feignHttpRequestException = new(_feignClient, request, exception);
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

    }
}
