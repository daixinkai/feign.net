using Polly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Polly
{
    internal class PolicySingletonCollection : PolicyCollection
    {

        internal readonly ConcurrentDictionary<Type, IAsyncPolicy> _serviceTypePolicyMap = new ConcurrentDictionary<Type, IAsyncPolicy>();

        public override IAsyncPolicy GetAsyncPolicy(string serviceId, Type serviceType)
        {
            if (_serviceTypePolicyMap.TryGetValue(serviceType, out var asyncPolicy))
            {
                return asyncPolicy;
            }
            return _serviceTypePolicyMap.GetOrAdd(serviceType, type => SetupAllPolly(serviceId, serviceType));
        }

    }
}
