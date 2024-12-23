using Feign.Configuration;
using Feign.Discovery;
using Feign.Internal;
using Feign.Logging;
using Feign.Pipeline.Internal;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Feign.Proxy
{
    /// <summary>
    /// Default HttpProxy
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public abstract partial class FeignClientHttpProxy<TService> : IFeignClient<TService>, IDisposable where TService : class
    {
        public FeignClientHttpProxy(FeignClientHttpProxyOptions options)
        {
            FeignOptions = options.FeignOptions;

            _globalPipeline = FeignOptions.FeignClientPipeline as GlobalFeignClientPipeline;
            _serviceIdPipeline = _globalPipeline?.GetServicePipeline(ServiceId);
            _servicePipeline = _globalPipeline?.GetServicePipeline<TService>();
            _features = new Dictionary<Type, object?>();

            _logger = options.LoggerFactory?.CreateLogger(typeof(FeignClientHttpProxy<TService>));

            var httpClientHandler = new ServiceDiscoveryHttpClientHandler<TService>(this, options.ServiceDiscovery, options.CacheProvider, _logger);
            if (FeignOptions.Request.AutomaticDecompression.HasValue)
            {
                httpClientHandler.AutomaticDecompression = FeignOptions.Request.AutomaticDecompression.Value;
            }
            if (FeignOptions.Request.UseCookies.HasValue)
            {
                httpClientHandler.UseCookies = FeignOptions.Request.UseCookies.Value;
            }
            httpClientHandler.ShouldResolveService = Url == null;
            httpClientHandler.AllowAutoRedirect = false;
            HttpClient = new FeignHttpClient(new FeignDelegatingHandler(httpClientHandler));
            //string baseUrl = httpClientHandler.ShouldResolveService ? (ServiceId ?? "") : Url!;
            string baseUrl = httpClientHandler.ShouldResolveService ? ServiceId : Url!;

            BaseUrl = BuildBaseUrl(baseUrl);

            Origin = $"{s_httpScheme}://{ServiceId}";

            #region Configuration
            if (options is FeignClientHttpProxyOptions<TService> serviceOptions && serviceOptions.ServiceConfiguration != null)
            {
                var servicePipeline = new ServiceFeignClientPipeline<TService>();
                var context = new FeignClientConfigurationContext<TService>(this, servicePipeline, HttpClient, httpClientHandler.HttpHandler);
                serviceOptions.ServiceConfiguration.Configure(context);
                if (servicePipeline.HasMiddleware())
                {
                    servicePipeline.Add(_servicePipeline);
                    _servicePipeline = servicePipeline;
                }
            }
            if (options.Configuration != null)
            {
                var serviceIdPipeline = new ServiceIdFeignClientPipeline(ServiceId);
                var context = new FeignClientConfigurationContext(this, serviceIdPipeline, HttpClient, httpClientHandler.HttpHandler);
                options.Configuration.Configure(context);
                if (serviceIdPipeline.HasMiddleware())
                {
                    serviceIdPipeline.Add(_serviceIdPipeline);
                    _serviceIdPipeline = serviceIdPipeline;
                }
            }
            #endregion


            var initializingContext = new InitializingPipelineContext<TService>(this, HttpClient, httpClientHandler.HttpHandler);
            OnInitializing(initializingContext);
            HttpClient = initializingContext.HttpClient;
            if (HttpClient == null)
            {
                throw new ArgumentNullException(nameof(HttpClient));
            }
        }


        private string BuildBaseUrl(string baseUrl)
        {
            if (!baseUrl.StartsWith("http") && baseUrl != "")
            {
                baseUrl = $"{s_httpScheme}://{baseUrl}";
            }
            if (!string.IsNullOrWhiteSpace(BaseUri))
            {
                if (baseUrl.EndsWith("/"))
                {
                    baseUrl = baseUrl.TrimEnd('/');
                }
                if (BaseUri!.StartsWith("/"))
                {
                    baseUrl += BaseUri;
                }
                else
                {
                    baseUrl += "/" + BaseUri;
                }
            }
            if (baseUrl.EndsWith("/"))
            {
                baseUrl = baseUrl.TrimEnd('/');
            }
            return baseUrl;
        }


        /// <summary>
        /// global pipeline
        /// </summary>
        internal readonly GlobalFeignClientPipeline? _globalPipeline;
        /// <summary>
        /// serviceId pipeline
        /// </summary>
        internal readonly ServiceIdFeignClientPipeline? _serviceIdPipeline;
        /// <summary>
        /// TService pipeline
        /// </summary>
        internal readonly ServiceFeignClientPipeline<TService>? _servicePipeline;

        private readonly Dictionary<Type, object?> _features;

        private readonly ILogger? _logger;

        private const string s_httpScheme = "http";

        protected internal IFeignOptions FeignOptions { get; private set; }

        TService IFeignClient<TService>.Service => (this as TService)!;

        Type IFeignClient<TService>.ServiceType => typeof(TService);

        IDictionary<Type, object?> IFeignClient.Features => _features;


        protected virtual UriKind UriKind => UriKind.Relative;

        /// <summary>
        /// Gets the serviceId
        /// </summary>
        public abstract string ServiceId { get; }
        /// <summary>
        /// Whether to respond to the terminated request? If this value is false, exceptions will continue to be thrown to the upper layer.
        /// </summary>
        protected virtual bool IsResponseTerminatedRequest => true;

        /// <summary>
        /// Gets the base uri
        /// </summary>
        public virtual string? BaseUri => null;
        /// <summary>
        /// Gets the url
        /// </summary>
        public virtual string? Url => null;

        protected string BaseUrl { get; }

        protected string Origin { get; }

        protected virtual string[]? DefaultHeaders => null;

        protected FeignHttpClient HttpClient { get; }


        #region IDisposable Support
        private bool _disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                var disposingContext = new DisposingPipelineContext<TService>(this, disposing);
                OnDisposing(disposingContext);
                if (disposing)
                {
                    HttpClient.Dispose();
                }
                _disposedValue = true;
            }
        }

        ~FeignClientHttpProxy()
        {
            Dispose(false);
        }
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region ConvertToStringValue
        protected virtual string? ConvertToStringValue<T>(T value)
        {
            return FeignOptions.Converters.ConvertStringValue(value, true);
        }
        #endregion

        #region PathVariable
        protected string ReplacePathVariable<T>(string uri, string name, T value)
        {
            return FeignClientUtils.ReplacePathVariable(FeignOptions.Converters, uri, name, value, FeignOptions.Request.UseUrlEncode);
        }
        protected string ReplaceStringPathVariable(string uri, string name, string value)
        {
            return FeignClientUtils.ReplacePathVariable(uri, name, value, FeignOptions.Request.UseUrlEncode);
        }
        protected string ReplaceToStringPathVariable<T>(string uri, string name, T value) where T : struct
        {
            return FeignClientUtils.ReplacePathVariable(uri, name, value.ToString(), FeignOptions.Request.UseUrlEncode);
        }
        protected string ReplaceNullablePathVariable<T>(string uri, string name, T? value) where T : struct
        {
            return FeignClientUtils.ReplacePathVariable(uri, name, value.ToString(), FeignOptions.Request.UseUrlEncode);
        }
        #endregion
        #region RequestQuery
        protected string ReplaceRequestQuery<T>(string uri, string name, T value)
        {
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value, FeignOptions);
        }
        protected string ReplaceStringRequestQuery(string uri, string name, string value)
        {
            if (value == null)
            {
                return uri;
            }
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value, FeignOptions.Request.UseUrlEncode);
        }
        protected string ReplaceToStringRequestQuery<T>(string uri, string name, T value) where T : struct
        {
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value.ToString(), FeignOptions.Request.UseUrlEncode);
        }
        protected string ReplaceNullableRequestQuery<T>(string uri, string name, T? value) where T : struct
        {
            if (!value.HasValue)
            {
                return uri;
            }
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value.Value.ToString(), FeignOptions.Request.UseUrlEncode);
        }
        #endregion

    }
}
