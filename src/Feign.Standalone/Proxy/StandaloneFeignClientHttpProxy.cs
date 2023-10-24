using Feign.Cache;
using Feign.Configuration;
using Feign.Discovery;
using Feign.Logging;
using Feign.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Standalone.Proxy
{
    public abstract class StandaloneFeignClientHttpProxy<TService> : FeignClientHttpProxy<TService> where TService : class
    {
        public StandaloneFeignClientHttpProxy() : base(FeignClients.CreateFeignClientConfigureOptions<TService>())
        {
        }
    }
}
