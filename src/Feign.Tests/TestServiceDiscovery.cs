using Feign.Discovery;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Tests
{
    class TestServiceDiscovery : IServiceDiscovery
    {
        public IList<string> Services => new List<string>();

        public IList<IServiceInstance> GetServiceInstances(string serviceId)
        {
            return new List<IServiceInstance>() {
                new TestServiceInstance("http://www.baidu.com")
            };
        }
    }
}
