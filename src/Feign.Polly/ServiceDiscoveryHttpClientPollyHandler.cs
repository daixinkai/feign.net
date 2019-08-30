using Feign.Cache;
using Feign.Discovery;
using Feign.Logging;
using Feign.Proxy;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Polly
{
    [Obsolete]
    public class ServiceDiscoveryHttpClientPollyHandler<TService> : ServiceDiscoveryHttpClientHandler<TService> where TService : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDiscoveryHttpClientHandler"/> class.
        /// </summary>
        public ServiceDiscoveryHttpClientPollyHandler(FeignClientHttpProxy<TService> feignClient, IServiceDiscovery serviceDiscovery, ICacheProvider serviceCacheProvider, ILogger logger) : base(feignClient, serviceDiscovery, serviceCacheProvider, logger)
        {
        }



    }
}
