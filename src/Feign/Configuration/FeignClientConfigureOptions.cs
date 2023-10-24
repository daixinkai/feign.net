using Feign.Cache;
using Feign.Discovery;
using Feign.Logging;
using System;

namespace Feign.Configuration
{
    public class FeignClientConfigureOptions<TService>
    {
        public FeignClientConfigureOptions(
            IFeignOptions feignOptions,
            IServiceDiscovery serviceDiscovery,
            ICacheProvider? cacheProvider,
            ILoggerFactory? loggerFactory,
            IFeignClientConfiguration? configuration = null,
            IFeignClientConfiguration<TService>? serviceConfiguration = null
            )
        {
            FeignOptions = feignOptions;
            ServiceDiscovery = serviceDiscovery;
            CacheProvider = cacheProvider;
            LoggerFactory = loggerFactory;
            Configuration = configuration;
            ServiceConfiguration = serviceConfiguration;
        }

        public IFeignOptions FeignOptions { get; }
        public IServiceDiscovery ServiceDiscovery { get; }
        public ICacheProvider? CacheProvider { get; }
        public ILoggerFactory? LoggerFactory { get; }
        public IFeignClientConfiguration? Configuration { get; set; }
        public IFeignClientConfiguration<TService>? ServiceConfiguration { get; }
    }
}
