using Polly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Polly
{
    public class FeignPollyOptions
    {

        private readonly PolicyCollection _policyCollection = new PolicySingletonCollection();


        public void Configure(Func<IAsyncPolicy, IAsyncPolicy> setup)
        {
            if (setup != null)
            {
                _policyCollection._globalPolicySetups.Add(setup);
            }
        }

        public void Configure(string serviceId, Func<IAsyncPolicy, IAsyncPolicy> setup)
        {
            if (setup != null)
            {
                var setups = _policyCollection._serviceIdPolicySetupsMap.GetOrAdd(serviceId, key => new List<Func<IAsyncPolicy, IAsyncPolicy>>());
                setups.Add(setup);
            }
        }

        public void Configure<TService>(Func<IAsyncPolicy, IAsyncPolicy> setup)
        {
            if (setup != null)
            {
                var setups = _policyCollection._serviceTypePolicySetupsMap.GetOrAdd(typeof(TService), key => new List<Func<IAsyncPolicy, IAsyncPolicy>>());
                setups.Add(setup);
            }
        }

        public void ConfigureAsync(Func<IAsyncPolicy, Task<IAsyncPolicy>> setup)
        {
            if (setup != null)
            {
                _policyCollection._globalPolicyAsyncSetups.Add(setup);
            }
        }

        public void ConfigureAsync(string serviceId, Func<IAsyncPolicy, Task<IAsyncPolicy>> setup)
        {
            if (setup != null)
            {
                var setups = _policyCollection._serviceIdPolicyAsyncSetupsMap.GetOrAdd(serviceId, key => new List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>());
                setups.Add(setup);
            }
        }

        public void ConfigureAsync<TService>(Func<IAsyncPolicy, Task<IAsyncPolicy>> setup)
        {
            if (setup != null)
            {
                var setups = _policyCollection._serviceTypePolicyAsyncSetupsMap.GetOrAdd(typeof(TService), key => new List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>());
                setups.Add(setup);
            }
        }

        public IAsyncPolicy GetAsyncPolicy(IFeignClient<object> feignClient)
        {
            return _policyCollection.GetAsyncPolicy(feignClient);
        }


    }
}
