using Polly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Polly
{
    class PolicyCollection
    {
        internal readonly List<Func<IAsyncPolicy, IAsyncPolicy>> _globalPolicySetups = new List<Func<IAsyncPolicy, IAsyncPolicy>>();
        internal readonly List<Func<IAsyncPolicy, Task<IAsyncPolicy>>> _globalPolicyAsyncSetups = new List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>();

        internal readonly ConcurrentDictionary<string, List<Func<IAsyncPolicy, IAsyncPolicy>>> _serviceIdPolicySetupsMap = new ConcurrentDictionary<string, List<Func<IAsyncPolicy, IAsyncPolicy>>>();
        internal readonly ConcurrentDictionary<string, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>> _serviceIdPolicyAsyncSetupsMap = new ConcurrentDictionary<string, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>>();


        internal readonly ConcurrentDictionary<Type, List<Func<IAsyncPolicy, IAsyncPolicy>>> _serviceTypePolicySetupsMap = new ConcurrentDictionary<Type, List<Func<IAsyncPolicy, IAsyncPolicy>>>();
        internal readonly ConcurrentDictionary<Type, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>> _serviceTypePolicyAsyncSetupsMap = new ConcurrentDictionary<Type, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>>();

        public virtual IAsyncPolicy GetAsyncPolicy(IFeignClient<object> feignClient)
        {
            return SetupAllPolly(feignClient);
        }



        internal IAsyncPolicy SetupAllPolly(IFeignClient<object> feignClient)
        {
            IAsyncPolicy asyncPolicy = Policy.NoOpAsync();
            asyncPolicy = SetupServiceTypePolly(asyncPolicy, feignClient);
            asyncPolicy = SetupServiceIdPolly(asyncPolicy, feignClient);
            asyncPolicy = SetupGlobalPolly(asyncPolicy, feignClient);
            return asyncPolicy;
        }

        internal IAsyncPolicy SetupServiceTypePolly(IAsyncPolicy asyncPolicy, IFeignClient<object> feignClient)
        {
            List<Func<IAsyncPolicy, IAsyncPolicy>> setups;
            List<Func<IAsyncPolicy, Task<IAsyncPolicy>>> asyncSetups;
            //serviceType
            if (_serviceTypePolicySetupsMap.TryGetValue(feignClient.ServiceType, out setups))
            {
                setups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy));
            }
            if (_serviceTypePolicyAsyncSetupsMap.TryGetValue(feignClient.ServiceType, out asyncSetups))
            {
                asyncSetups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy).ConfigureAwait(false).GetAwaiter().GetResult());
            }
            return asyncPolicy;
        }
        internal IAsyncPolicy SetupServiceIdPolly(IAsyncPolicy asyncPolicy, IFeignClient<object> feignClient)
        {
            List<Func<IAsyncPolicy, IAsyncPolicy>> setups;
            List<Func<IAsyncPolicy, Task<IAsyncPolicy>>> asyncSetups;
            //serviceId
            if (_serviceIdPolicySetupsMap.TryGetValue(feignClient.ServiceId, out setups))
            {
                setups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy));
            }
            if (_serviceIdPolicyAsyncSetupsMap.TryGetValue(feignClient.ServiceId, out asyncSetups))
            {
                asyncSetups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy).ConfigureAwait(false).GetAwaiter().GetResult());
            }
            return asyncPolicy;
        }
        internal IAsyncPolicy SetupGlobalPolly(IAsyncPolicy asyncPolicy, IFeignClient<object> feignClient)
        {
            //global
            _globalPolicySetups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy));
            _globalPolicyAsyncSetups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy).ConfigureAwait(false).GetAwaiter().GetResult());
            return asyncPolicy;
        }


    }
}
