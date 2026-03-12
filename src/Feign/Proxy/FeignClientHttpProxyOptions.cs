using Feign.Cache;
using Feign.Configuration;
using Feign.Discovery;
using Feign.Logging;
using System;

namespace Feign.Proxy
{
    public class FeignClientHttpProxyOptions
    {
        public FeignClientHttpProxyOptions(
            FeignOptions feignOptions,
            IServiceDiscovery serviceDiscovery,
            ICacheProvider? cacheProvider,
            ILoggerFactory? loggerFactory
            )
        {
            FeignOptions = feignOptions;
            ServiceDiscovery = serviceDiscovery;
            CacheProvider = cacheProvider;
            LoggerFactory = loggerFactory;
        }

        public FeignOptions FeignOptions { get; }
        public IServiceDiscovery ServiceDiscovery { get; }
        public ICacheProvider? CacheProvider { get; }
        public ILoggerFactory? LoggerFactory { get; }
        public IFeignClientConfiguration? Configuration { get; set; }
    }

    public class FeignClientHttpProxyOptions<TService> : FeignClientHttpProxyOptions
    {
        public FeignClientHttpProxyOptions(FeignOptions feignOptions, IServiceDiscovery serviceDiscovery, ICacheProvider? cacheProvider, ILoggerFactory? loggerFactory) : base(feignOptions, serviceDiscovery, cacheProvider, loggerFactory)
        {
        }

        public IFeignClientConfiguration<TService>? ServiceConfiguration { get; set; }
    }
}
