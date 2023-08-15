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

        public override IAsyncPolicy GetAsyncPolicy(IFeignClient<object> feignClient)
        {
            IAsyncPolicy asyncPolicy;
            if (_serviceTypePolicyMap.TryGetValue(feignClient.ServiceType, out asyncPolicy))
            {
                return asyncPolicy;
            }
            return _serviceTypePolicyMap.GetOrAdd(feignClient.ServiceType, type => SetupAllPolly(feignClient));
        }

    }
}
