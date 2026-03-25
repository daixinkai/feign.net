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
            Options = options.FeignOptions;

            _globalPipeline = Options.Pipeline as GlobalFeignClientPipeline ?? new();

            _serviceIdPipeline = MergePipeline(
                new ServiceIdFeignClientPipeline(ServiceId),
                _globalPipeline.GetServicePipeline(ServiceId),
                _globalPipeline.GetKeyedServicePipeline(Key, ServiceId)
                );

            _servicePipeline = MergePipeline(
                new ServiceFeignClientPipeline<TService>(),
                _globalPipeline.GetServicePipeline<TService>(),
                _globalPipeline.GetKeyedServicePipeline<TService>(Key)
                );

            _features = FeignClientUtils.CreateDictionary<Type>();

            _logger = options.LoggerFactory?.CreateLogger(typeof(FeignClientHttpProxy<TService>));

            var httpClientHandler = new ServiceDiscoveryHttpClientHandler<TService>(this, options.ServiceDiscovery, options.CacheProvider, _logger);
            httpClientHandler.SetOptions(Options);
            HttpClient = new FeignHttpClient(new FeignDelegatingHandler(httpClientHandler));
            //string baseUrl = httpClientHandler.ShouldResolveService ? ServiceId : Url!;
            string baseUrl = Url ?? ServiceId;

            BaseUrl = BuildBaseUrl(baseUrl);

            Origin = $"{s_httpScheme}://{ServiceId}";

            #region Configuration
            if (options is FeignClientHttpProxyOptions<TService> serviceOptions && serviceOptions.ServiceConfiguration != null)
            {
                var servicePipeline = new ServiceFeignClientPipeline<TService>();
                var context = new FeignClientConfigurationContext<TService>(this, servicePipeline, HttpClient, httpClientHandler.HttpHandler);
                serviceOptions.ServiceConfiguration.Configure(context);
                _servicePipeline = MergePipeline(servicePipeline, _servicePipeline);
            }
            if (options.Configuration != null)
            {
                var serviceIdPipeline = new ServiceIdFeignClientPipeline(ServiceId);
                var context = new FeignClientConfigurationContext(this, serviceIdPipeline, HttpClient, httpClientHandler.HttpHandler);
                options.Configuration.Configure(context);
                _serviceIdPipeline = MergePipeline(serviceIdPipeline, _serviceIdPipeline);
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

        private readonly IDictionary<Type, object?> _features;

        private readonly ILogger? _logger;

        private const string s_httpScheme = "http";

        protected internal FeignOptions Options { get; private set; }

        TService IFeignClient<TService>.Service => (this as TService)!;

        Type IFeignClient<TService>.ServiceType => typeof(TService);

        string? IFeignClient.Key => Key;

        string IFeignClient.ServiceId => ServiceId;

        IDictionary<Type, object?> IFeignClient.Features => _features;


        protected virtual UriKind UriKind => UriKind.Relative;


        protected virtual string? Key { get; }



        /// <summary>
        /// Gets the serviceId
        /// </summary>
        protected abstract string ServiceId { get; }



        /// <summary>
        /// Whether to respond to the terminated request? If this value is false, exceptions will continue to be thrown to the upper layer.
        /// </summary>
        protected virtual bool IsResponseTerminatedRequest => true;

        /// <summary>
        /// Gets the base uri
        /// </summary>
        protected virtual string? BaseUri => null;
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
            return Options.Converters.ConvertStringValue(value, true);
        }
        #endregion

        #region PathVariable
        protected string ReplacePathVariable<T>(string uri, string name, T value)
        {
            return FeignClientUtils.ReplacePathVariable(Options.Converters, uri, name, value, Options.Request.UseUrlEncode);
        }
        protected string ReplaceStringPathVariable(string uri, string name, string value)
        {
            return FeignClientUtils.ReplacePathVariable(uri, name, value, Options.Request.UseUrlEncode);
        }
        protected string ReplaceToStringPathVariable<T>(string uri, string name, T value) where T : struct
        {
            return FeignClientUtils.ReplacePathVariable(uri, name, value.ToString(), Options.Request.UseUrlEncode);
        }
        protected string ReplaceNullablePathVariable<T>(string uri, string name, T? value) where T : struct
        {
            return FeignClientUtils.ReplacePathVariable(uri, name, value.ToString(), Options.Request.UseUrlEncode);
        }
        #endregion
        #region RequestQuery
        protected string ReplaceRequestQuery<T>(string uri, string name, T value)
        {
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value, Options);
        }
        protected string ReplaceStringRequestQuery(string uri, string name, string value)
        {
            if (value == null)
            {
                return uri;
            }
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value, Options.Request.UseUrlEncode);
        }
        protected string ReplaceToStringRequestQuery<T>(string uri, string name, T value) where T : struct
        {
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value.ToString(), Options.Request.UseUrlEncode);
        }
        protected string ReplaceNullableRequestQuery<T>(string uri, string name, T? value) where T : struct
        {
            if (!value.HasValue)
            {
                return uri;
            }
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value.Value.ToString(), Options.Request.UseUrlEncode);
        }
        #endregion

    }
}
