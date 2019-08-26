using Feign.Cache;
using Feign.Discovery;
using Feign.Internal;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    public abstract partial class FeignClientHttpProxy<TService> : IFeignClient<TService>, IDisposable where TService : class
    {
        public FeignClientHttpProxy(IFeignOptions feignOptions, IServiceDiscovery serviceDiscovery, ICacheProvider cacheProvider, ILoggerFactory loggerFactory)
        {
            _feignOptions = feignOptions;
            _globalFeignClientPipeline = _feignOptions?.FeignClientPipeline as GlobalFeignClientPipeline;
            _serviceIdFeignClientPipeline = _globalFeignClientPipeline?.GetServicePipeline(ServiceId);
            _serviceFeignClientPipeline = _globalFeignClientPipeline?.GetServicePipeline<TService>();
            _logger = loggerFactory?.CreateLogger(typeof(FeignClientHttpProxy<TService>));
            ServiceDiscoveryHttpClientHandler<TService> serviceDiscoveryHttpClientHandler = new ServiceDiscoveryHttpClientHandler<TService>(this, serviceDiscovery, cacheProvider, _logger);
            serviceDiscoveryHttpClientHandler.ShouldResolveService = string.IsNullOrWhiteSpace(Url);
            serviceDiscoveryHttpClientHandler.AllowAutoRedirect = false;
            HttpClient = new HttpClient(serviceDiscoveryHttpClientHandler);
            string baseUrl = serviceDiscoveryHttpClientHandler.ShouldResolveService ? ServiceId ?? "" : Url;
            if (!baseUrl.StartsWith("http"))
            {
                baseUrl = $"http://{baseUrl}";
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
            BaseUrl = baseUrl;

            InitializingEventArgs<TService> initializingEventArgs = new InitializingEventArgs<TService>(this);
            initializingEventArgs.HttpClient = HttpClient;
            OnInitializing(initializingEventArgs);
            HttpClient = initializingEventArgs.HttpClient;
            if (HttpClient == null)
            {
                throw new ArgumentNullException(nameof(HttpClient));
            }
        }



        internal GlobalFeignClientPipeline _globalFeignClientPipeline;

        internal ServiceIdFeignClientPipeline _serviceIdFeignClientPipeline;

        internal ServiceFeignClientPipeline<TService> _serviceFeignClientPipeline;

        ILogger _logger;

        IFeignOptions _feignOptions;

        protected IFeignOptions FeignOptions => _feignOptions;

        TService IFeignClient<TService>.Service { get { return this as TService; } }

        public abstract string ServiceId { get; }

        protected virtual bool IsResponseTerminatedRequest => true;

        public virtual string BaseUri { get { return null; } }

        public virtual string Url { get { return null; } }

        protected string BaseUrl { get; }

        protected HttpClient HttpClient { get; }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                DisposingEventArgs<TService> disposingEventArgs = new DisposingEventArgs<TService>(this, disposing);
                OnDisposing(disposingEventArgs);
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                HttpClient.Dispose();
                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        //~FeignClientServiceBase()
        //{
        //    // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //    Dispose(false);
        //}

        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            //GC.SuppressFinalize(this);
        }
        #endregion

        #region PathVariable
        protected string ReplacePathVariable<T>(string uri, string name, T value)
        {
            return FeignClientUtils.ReplacePathVariable<T>(_feignOptions.Converters, uri, name, value);
        }
        #endregion
        #region RequestParam
        protected string ReplaceRequestParam<T>(string uri, string name, T value)
        {
            return FeignClientUtils.ReplaceRequestParam<T>(_feignOptions.Converters, uri, name, value);
        }
        #endregion
        #region RequestQuery
        protected string ReplaceRequestQuery<T>(string uri, string name, T value)
        {
            return FeignClientUtils.ReplaceRequestQuery<T>(_feignOptions.Converters, uri, name, value);
        }
        #endregion

    }
}
