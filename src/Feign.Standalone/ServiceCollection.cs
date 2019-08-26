using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Standalone
{
    class ServiceCollection //: IList<ServiceDescriptor>
    {
        readonly IDictionary<Type, ServiceDescriptor> _map = new Dictionary<Type, ServiceDescriptor>();

        object _lockObject = new object();

        public void AddOrUpdate(ServiceDescriptor serviceDescriptor)
        {
            lock (_lockObject)
            {
                if (_map.ContainsKey(serviceDescriptor.ServiceType))
                {
                    _map[serviceDescriptor.ServiceType] = serviceDescriptor;
                }
                else
                {
                    _map.Add(serviceDescriptor.ServiceType, serviceDescriptor);
                }
            }
        }

        public ServiceDescriptor Get(Type serviceType)
        {
            ServiceDescriptor serviceDescriptor;
            if (_map.TryGetValue(serviceType, out serviceDescriptor))
            {
                return serviceDescriptor;
            }
            return null;
        }

        public void Remove(Type type)
        {
            _map.Remove(type);
        }

    }
}
