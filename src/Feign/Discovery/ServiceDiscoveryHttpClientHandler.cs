using Feign.Cache;
using Feign.Internal;
using Feign.Logging;
using Feign.Proxy;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feign.Discovery
{
    /// <summary>
    /// 可发现服务的HttpClient消息处理程序
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class ServiceDiscoveryHttpClientHandler<TService> : FeignProxyHttpClientHandler<TService> where TService : class
    {

        private IServiceResolve _serviceResolve;
        private IServiceDiscovery _serviceDiscovery;
        private ICacheProvider _serviceCacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDiscoveryHttpClientHandler{TService}"/> class.
        /// </summary>
        public ServiceDiscoveryHttpClientHandler(FeignClientHttpProxy<TService> feignClient, IServiceDiscovery serviceDiscovery, ICacheProvider serviceCacheProvider, ILogger logger) : base(feignClient, logger)
        {
            _serviceResolve = new RandomServiceResolve(logger);
            _serviceDiscovery = serviceDiscovery;
            _serviceCacheProvider = serviceCacheProvider;
            ShouldResolveService = true;
        }


        public bool ShouldResolveService { get; set; }


        protected override Uri LookupRequestUri(FeignHttpRequestMessage requestMessage)
        {
            if (!ShouldResolveService)
            {
                return requestMessage.RequestUri;
            }
            if (_serviceDiscovery == null)
            {
                ServiceResolveFail(requestMessage);
                return requestMessage.RequestUri;
            }
            IList<IServiceInstance> services = _serviceDiscovery.GetServiceInstancesWithCache(requestMessage.RequestUri.Host, _serviceCacheProvider);
            if (services == null || services.Count == 0)
            {
                ServiceResolveFail(requestMessage);
            }
            return _serviceResolve.ResolveService(requestMessage.RequestUri, services);
        }

        void ServiceResolveFail(FeignHttpRequestMessage requestMessage)
        {
            throw new ServiceResolveFailException($"Resolve service fail : {requestMessage.RequestUri.ToString()}");
        }

    }
}
