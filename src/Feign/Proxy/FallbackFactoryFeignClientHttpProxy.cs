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
    public abstract class FallbackFactoryFeignClientHttpProxy<TService, TFallbackFactory> : FallbackFeignClientHttpProxy<TService, TService>, IFallbackFactoryFeignClient<TService>, IFeignClient<TService> where TService : class
        where TFallbackFactory : IFallbackFactory<TService>
    {
        public FallbackFactoryFeignClientHttpProxy(IFeignOptions feignOptions, IServiceDiscovery serviceDiscovery, ICacheProvider cacheProvider, ILoggerFactory loggerFactory, TFallbackFactory fallbackFactory) : base(feignOptions, serviceDiscovery, cacheProvider, loggerFactory, GetFallback(fallbackFactory))
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


        static TService GetFallback(IFallbackFactory<TService> fallbackFactory)
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
