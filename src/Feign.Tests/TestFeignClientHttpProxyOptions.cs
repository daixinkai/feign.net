using Feign.Cache;
using Feign.Discovery;
using Feign.Logging;
using Feign.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    internal class TestFeignClientHttpProxyOptions : FeignClientHttpProxyOptions<ITestControllerService>
    {
        public TestFeignClientHttpProxyOptions(IFeignOptions feignOptions, IServiceDiscovery serviceDiscovery, ICacheProvider cacheProvider, ILoggerFactory loggerFactory, TestConfiguration configuration) : base(feignOptions, serviceDiscovery, cacheProvider, loggerFactory)
        {
            //Configuration = configuration;
            //ServiceConfiguration = configuration;
        }
    }
}
