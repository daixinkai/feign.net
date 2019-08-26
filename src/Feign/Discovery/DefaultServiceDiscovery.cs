using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Discovery
{
    public class DefaultServiceDiscovery : IServiceDiscovery
    {
        public IList<string> Services => null;

        public IList<IServiceInstance> GetServiceInstances(string serviceId)
        {
            return null;
        }
    }
}
