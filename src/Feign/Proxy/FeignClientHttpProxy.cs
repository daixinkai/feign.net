using Feign.Cache;
using Feign.Discovery;
using Feign.Internal;
using Feign.Logging;
using Feign.Pipeline.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    /// <summary>
    /// 默认的HttpProxy代理
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public abstract partial class FeignClientHttpProxy<TService> : IFeignClient<TService>, IDisposable where TService : class
    {
        public FeignClientHttpProxy(
            IFeignOptions feignOptions,
            IServiceDiscovery serviceDiscovery,
            ICacheProvider cacheProvider = null,
            ILoggerFactory loggerFactory = null
            )
        {
            FeignOptions = feignOptions;
            _globalFeignClientPipeline = FeignOptions.FeignClientPipeline as GlobalFeignClientPipeline;
            _serviceIdFeignClientPipeline = _globalFeignClientPipeline?.GetServicePipeline(ServiceId);
            _serviceFeignClientPipeline = _globalFeignClientPipeline?.GetServicePipeline<TService>();
            _logger = loggerFactory?.CreateLogger(typeof(FeignClientHttpProxy<TService>));
            var serviceDiscoveryHttpClientHandler = new ServiceDiscoveryHttpClientHandler<TService>(this, serviceDiscovery, cacheProvider, _logger);
            if (FeignOptions.AutomaticDecompression.HasValue)
            {
                serviceDiscoveryHttpClientHandler.AutomaticDecompression = feignOptions.AutomaticDecompression.Value;
            }
            if (FeignOptions.UseCookies.HasValue)
            {
                serviceDiscoveryHttpClientHandler.UseCookies = FeignOptions.UseCookies.Value;
            }
            //serviceDiscoveryHttpClientHandler.ShouldResolveService = string.IsNullOrWhiteSpace(Url);
            serviceDiscoveryHttpClientHandler.ShouldResolveService = Url == null;
            serviceDiscoveryHttpClientHandler.AllowAutoRedirect = false;
            HttpClient = new FeignHttpClient(new FeignDelegatingHandler(serviceDiscoveryHttpClientHandler));
            string baseUrl = serviceDiscoveryHttpClientHandler.ShouldResolveService ? ServiceId ?? "" : Url;

            BaseUrl = BuildBaseUrl(baseUrl);

            Origin = $"{_scheme}://{ServiceId}";

            var initializingContext = new InitializingPipelineContext<TService>(this);
            initializingContext.HttpClient = HttpClient;
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
                baseUrl = $"{_scheme}://{baseUrl}";
            }
            if (!string.IsNullOrWhiteSpace(BaseUri))
            {
                if (baseUrl.EndsWith("/"))
                {
                    baseUrl = baseUrl.TrimEnd('/');
                }
                if (BaseUri.StartsWith("/"))
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
        /// 全局pipeline
        /// </summary>
        internal GlobalFeignClientPipeline _globalFeignClientPipeline;
        /// <summary>
        /// serviceId pipeline
        /// </summary>
        internal ServiceIdFeignClientPipeline _serviceIdFeignClientPipeline;
        /// <summary>
        /// TService pipeline
        /// </summary>
        internal ServiceFeignClientPipeline<TService> _serviceFeignClientPipeline;

        private ILogger _logger;

        private string _scheme = "http";

        protected internal IFeignOptions FeignOptions { get; private set; }

        TService IFeignClient<TService>.Service { get { return this as TService; } }

        Type IFeignClient<TService>.ServiceType { get { return typeof(TService); } }

        protected virtual UriKind UriKind => UriKind.Relative;

        /// <summary>
        /// 获取服务的 serviceId
        /// </summary>
        public abstract string ServiceId { get; }
        /// <summary>
        /// 是否响应终止的请求? 此值如果是false的话将继续往上层抛异常
        /// </summary>
        protected virtual bool IsResponseTerminatedRequest => true;

        /// <summary>
        /// 获取服务的base uri
        /// </summary>
        public virtual string BaseUri { get { return null; } }
        /// <summary>
        /// 获取服务的url
        /// </summary>
        public virtual string Url { get { return null; } }

        protected string BaseUrl { get; }

        protected string Origin { get; }

        protected virtual string[] DefaultHeaders => null;

        protected FeignHttpClient HttpClient { get; }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                var disposingContext = new DisposingPipelineContext<TService>(this, disposing);
                OnDisposing(disposingContext);
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    HttpClient.Dispose();
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        //~FeignClientHttpProxy()
        //{
        //    // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //    Dispose(false);
        //}

        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            //GC.SuppressFinalize(this);
        }
        #endregion

        #region ConvertToStringValue
        protected virtual string ConvertToStringValue<T>(T value)
        {
            return FeignOptions.Converters.ConvertValue<T, string>(value, true);
        }
        #endregion

        #region PathVariable
        protected string ReplacePathVariable<T>(string uri, string name, T value)
        {
            return FeignClientUtils.ReplacePathVariable<T>(FeignOptions.Converters, uri, name, value, FeignOptions.UseUrlEncode);
        }
        protected string ReplaceStringPathVariable(string uri, string name, string value)
        {
            return FeignClientUtils.ReplacePathVariable(uri, name, value, FeignOptions.UseUrlEncode);
        }
        protected string ReplaceToStringPathVariable<T>(string uri, string name, T value) where T : struct
        {
            return FeignClientUtils.ReplacePathVariable(uri, name, value.ToString(), FeignOptions.UseUrlEncode);
        }
        protected string ReplaceNullablePathVariable<T>(string uri, string name, T? value) where T : struct
        {
            return FeignClientUtils.ReplacePathVariable(uri, name, value.ToString(), FeignOptions.UseUrlEncode);
        }
        #endregion
        #region RequestQuery
        protected string ReplaceRequestQuery<T>(string uri, string name, T value)
        {
            return FeignClientUtils.ReplaceRequestQuery<T>(FeignOptions.Converters, FeignOptions.PropertyNamingPolicy, uri, name, value, FeignOptions.UseUrlEncode);
        }
        protected string ReplaceStringRequestQuery(string uri, string name, string value)
        {
            if (value == null)
            {
                return uri;
            }
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value, FeignOptions.UseUrlEncode);
        }
        protected string ReplaceToStringRequestQuery<T>(string uri, string name, T value) where T : struct
        {
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value.ToString(), FeignOptions.UseUrlEncode);
        }
        protected string ReplaceNullableRequestQuery<T>(string uri, string name, T? value) where T : struct
        {
            if (!value.HasValue)
            {
                return uri;
            }
            return FeignClientUtils.ReplaceRequestQuery(uri, name, value.Value.ToString(), FeignOptions.UseUrlEncode);
        }
        #endregion

    }
}
