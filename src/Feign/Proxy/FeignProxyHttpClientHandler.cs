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
    /// Message handler used by FeignHttpClient
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class FeignProxyHttpClientHandler<TService> : HttpClientHandler where TService : class
    {
        private readonly ILogger? _logger;
        private readonly FeignClientHttpProxy<TService> _feignClient;

        public FeignClientHttpProxy<TService> FeignClient => _feignClient;

#if NETCOREAPP2_1_OR_GREATER

        private static readonly Func<HttpClientHandler, SocketsHttpHandler> SocketsHttpHandlerGetter = CreateSocketsHttpHandlerGetter();

        private static Func<HttpClientHandler, SocketsHttpHandler> CreateSocketsHttpHandlerGetter()
        {
            var fieldInfo = typeof(HttpClientHandler).GetField("_underlyingHandler", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo == null || fieldInfo.FieldType != typeof(SocketsHttpHandler))
            {
                fieldInfo = typeof(HttpClientHandler).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).First(s => s.FieldType == typeof(SocketsHttpHandler));
            }
            ParameterExpression instance = Expression.Parameter(typeof(HttpClientHandler));
            Expression body = Expression.Field(instance, fieldInfo);
            return Expression.Lambda<Func<HttpClientHandler, SocketsHttpHandler>>(body, instance).Compile();
        }

        internal SocketsHttpHandler HttpHandler
                    => SocketsHttpHandlerGetter.Invoke(this);
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
        /// Find service uri
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
#if USE_VALUE_TASK
        protected virtual ValueTask<Uri?> LookupRequestUriAsync(FeignHttpRequestMessage requestMessage)
        {
            return new ValueTask<Uri?>(requestMessage.RequestUri);
        }
#else
        protected virtual Task<Uri?> LookupRequestUriAsync(FeignHttpRequestMessage requestMessage)
        {
            return Task.FromResult<Uri?>(requestMessage.RequestUri);
        }
#endif
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
#if USE_CONFIGUREAWAIT_FALSE
                    .ConfigureAwait(false)
#endif
                    ;
                #endregion

                return await base.SendAsync(request, requestContext.CancellationToken)
#if USE_CONFIGUREAWAIT_FALSE
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
