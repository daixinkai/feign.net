using Feign.Cache;
using Feign.Discovery;
using Feign.Fallback;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Proxy
{
    /// <summary>
    /// 支持服务降级的HttpProxy
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TFallbackFactory"></typeparam>
    public abstract class FallbackFactoryFeignClientHttpProxy<TService, TFallbackFactory> : FallbackFeignClientHttpProxy<TService, TService>, IFallbackFactoryFeignClient<TService>, IFeignClient<TService> where TService : class
        where TFallbackFactory : IFallbackFactory<TService>
    {
        public FallbackFactoryFeignClientHttpProxy(TFallbackFactory fallbackFactory, IFeignOptions feignOptions, IServiceDiscovery serviceDiscovery, ICacheProvider cacheProvider = null, ILoggerFactory loggerFactory = null) : base(GetFallback(fallbackFactory), feignOptions, serviceDiscovery, cacheProvider, loggerFactory)
        {
            FallbackFactory = fallbackFactory;
        }

        public TFallbackFactory FallbackFactory { get; }

        IFallbackFactory<TService> IFallbackFactoryFeignClient<TService>.FallbackFactory
        {
            get
            {
                return FallbackFactory;
            }
        }


        private static TService GetFallback(IFallbackFactory<TService> fallbackFactory)
        {
            if (fallbackFactory != null)
            {
                return fallbackFactory.GetFallback();
            }
            return default(TService);
        }

        protected override void Dispose(bool disposing)
        {
            if (FallbackFactory != null && Fallback != null)
            {
                FallbackFactory.ReleaseFallback(Fallback);
            }
            base.Dispose(disposing);
        }

    }

}
