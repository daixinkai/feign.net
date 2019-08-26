using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    public interface IServiceDiscovery
    {
        IList<string> Services { get; }

        IList<IServiceInstance> GetServiceInstances(string serviceId);

    }
}
