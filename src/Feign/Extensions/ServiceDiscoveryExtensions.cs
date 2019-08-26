using Feign.Cache;
using Feign.Discovery;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    static class ServiceDiscoveryExtensions
    {
        public static async Task<IList<IServiceInstance>> GetServiceInstancesWithCacheAsync(this IServiceDiscovery serviceDiscovery, string serviceId, ICacheProvider cacheProvider, string serviceInstancesKeyPrefix = "ServiceDiscovery-ServiceInstances-")
        {
            // if distributed cache was provided, just make the call back to the provider
            if (cacheProvider != null)
            {
                // check the cache for existing service instances
                var services = await cacheProvider.GetAsync<IList<IServiceInstance>>(serviceInstancesKeyPrefix + serviceId);
                if (services != null && services.Count > 0)
                {
                    return services;
                }
            }

            // cache not found or instances not found, call out to the provider
            var instances = serviceDiscovery.GetServiceInstances(serviceId) ?? new List<IServiceInstance>();
            if (cacheProvider != null)
            {
                await cacheProvider.SetAsync(serviceInstancesKeyPrefix + serviceId, instances, TimeSpan.FromMinutes(10));
            }

            return instances;
        }

        public static IList<IServiceInstance> GetServiceInstancesWithCache(this IServiceDiscovery serviceDiscovery, string serviceId, ICacheProvider cacheProvider, string serviceInstancesKeyPrefix = "ServiceDiscovery-ServiceInstances-")
        {
            // if distributed cache was provided, just make the call back to the provider
            if (cacheProvider != null)
            {
                // check the cache for existing service instances
                var services = cacheProvider.Get<IList<IServiceInstance>>(serviceInstancesKeyPrefix + serviceId);
                if (services != null && services.Count > 0)
                {
                    return services;
                }
            }

            // cache not found or instances not found, call out to the provider
            var instances = serviceDiscovery.GetServiceInstances(serviceId) ?? new List<IServiceInstance>();
            if (cacheProvider != null)
            {
                cacheProvider.Set(serviceInstancesKeyPrefix + serviceId, instances, TimeSpan.FromMinutes(10));
            }

            return instances;
        }

    }
}
