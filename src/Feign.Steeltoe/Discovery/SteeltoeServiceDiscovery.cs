using Feign.Discovery;
using Steeltoe.Common.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Feign.Discovery
{
    public class SteeltoeServiceDiscovery : IServiceDiscovery
    {
        public SteeltoeServiceDiscovery(IDiscoveryClient discoveryClient)
        {
            _discoveryClient = discoveryClient;
        }
        public IDiscoveryClient _discoveryClient;

        public IList<string> Services => _discoveryClient.Services;

        public IList<IServiceInstance> GetServiceInstances(string serviceId)
        {
            return _discoveryClient.GetInstances(serviceId).Select(s => new SteeltoeServiceInstance(s)).ToList<IServiceInstance>();
        }
    }
}
