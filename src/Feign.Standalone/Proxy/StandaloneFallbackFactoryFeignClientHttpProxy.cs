using Feign.Cache;
using Feign.Configuration;
using Feign.Discovery;
using Feign.Fallback;
using Feign.Logging;
using Feign.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Standalone.Proxy
{
    public abstract class StandaloneFallbackFactoryFeignClientHttpProxy<TService, TFallbackFactory> : FallbackFactoryFeignClientHttpProxy<TService, TFallbackFactory>
        where TFallbackFactory : IFallbackFactory<TService>
        where TService : class
    {
        public StandaloneFallbackFactoryFeignClientHttpProxy() : base(FeignClients.Get<TFallbackFactory>(), FeignClients.CreateFeignClientConfigureOptions<TService>())
        {
        }

    }
}
