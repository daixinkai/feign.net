﻿using Feign.Discovery;
using IDiscoveryClient = Steeltoe.Discovery.IDiscoveryClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Feign.Discovery
{
    public class SteeltoeServiceDiscovery : IServiceDiscovery
    {
        public SteeltoeServiceDiscovery(IDiscoveryClient discoveryClient = null)
        {
            _discoveryClient = discoveryClient;
        }
        public IDiscoveryClient _discoveryClient;

        public IList<string> Services => _discoveryClient?.Services;

        public IList<IServiceInstance> GetServiceInstances(string serviceId)
        {
            if (_discoveryClient == null)
            {
                return null;
            }
            return _discoveryClient.GetInstances(serviceId).Select(s => new SteeltoeServiceInstance(s)).ToList<IServiceInstance>();
        }
    }
}
