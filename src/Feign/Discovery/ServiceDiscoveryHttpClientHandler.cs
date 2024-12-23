using Feign.Cache;
using Feign.Discovery.LoadBalancing;
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
    /// HttpClient message handler for discoverable services
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class ServiceDiscoveryHttpClientHandler<TService> : FeignProxyHttpClientHandler<TService> where TService : class
    {

        private readonly IServiceResolve _serviceResolve;
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly ICacheProvider? _serviceCacheProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceDiscoveryHttpClientHandler{TService}"/> class.
        /// </summary>
        public ServiceDiscoveryHttpClientHandler(
            FeignClientHttpProxy<TService> feignClient,
            IServiceDiscovery serviceDiscovery,
            ICacheProvider? serviceCacheProvider,
            ILogger? logger) : base(feignClient, logger)
        {
            _serviceResolve = feignClient.FeignOptions.Request.LoadBalancingPolicy switch
            {
                LoadBalancingPolicy.FirstAlphabetical => new FirstServiceResolve(logger),
                LoadBalancingPolicy.Random => new RandomServiceResolve(logger),
                LoadBalancingPolicy.RoundRobin => new RoundRobinServiceResolve(logger),
                LoadBalancingPolicy.LeastRequests => new LeastRequestsServiceResolve(logger),
                LoadBalancingPolicy.PowerOfTwoChoices => new PowerOfTwoChoicesServiceResolve(logger),
                _ => new RandomServiceResolve(logger)
            };
            _serviceDiscovery = serviceDiscovery;
            _serviceCacheProvider = serviceCacheProvider;
            ShouldResolveService = true;
        }


        public bool ShouldResolveService { get; set; }

#if USE_VALUE_TASK
        protected override async ValueTask<Uri?> LookupRequestUriAsync(FeignHttpRequestMessage requestMessage)
#else
        protected override async Task<Uri?> LookupRequestUriAsync(FeignHttpRequestMessage requestMessage)
#endif
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

            string serviceId = requestMessage.ServiceId ?? requestMessage.RequestUri!.Host;

            var cacheTime = FeignClient.FeignOptions.Request.DiscoverServiceCacheTime;
            IList<IServiceInstance>? services = cacheTime.HasValue ?
            await _serviceDiscovery.GetServiceInstancesWithCacheAsync(serviceId, _serviceCacheProvider, cacheTime.Value)
#if USE_CONFIGUREAWAIT_FALSE
                .ConfigureAwait(false)
#endif
            : _serviceDiscovery.GetServiceInstances(serviceId);
            if (services == null || services.Count == 0)
            {
                ServiceResolveFail(requestMessage);
            }
            return _serviceResolve.ResolveService(serviceId, requestMessage.RequestUri!, services);
        }

        private static void ServiceResolveFail(FeignHttpRequestMessage requestMessage)
        {
            throw new ServiceResolveFailException($"Resolve service fail : {requestMessage.RequestUri}");
        }

    }
}
