using Polly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Polly
{
    internal class PolicyCollection
    {
        internal readonly List<Func<IAsyncPolicy, IAsyncPolicy>> _globalPolicySetups = new List<Func<IAsyncPolicy, IAsyncPolicy>>();
        internal readonly List<Func<IAsyncPolicy, Task<IAsyncPolicy>>> _globalPolicyAsyncSetups = new List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>();

        internal readonly ConcurrentDictionary<string, List<Func<IAsyncPolicy, IAsyncPolicy>>> _serviceIdPolicySetupsMap = new ConcurrentDictionary<string, List<Func<IAsyncPolicy, IAsyncPolicy>>>();
        internal readonly ConcurrentDictionary<string, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>> _serviceIdPolicyAsyncSetupsMap = new ConcurrentDictionary<string, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>>();


        internal readonly ConcurrentDictionary<Type, List<Func<IAsyncPolicy, IAsyncPolicy>>> _serviceTypePolicySetupsMap = new ConcurrentDictionary<Type, List<Func<IAsyncPolicy, IAsyncPolicy>>>();
        internal readonly ConcurrentDictionary<Type, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>> _serviceTypePolicyAsyncSetupsMap = new ConcurrentDictionary<Type, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>>();

        public virtual IAsyncPolicy GetAsyncPolicy(string serviceId, Type serviceType)
        {
            return SetupAllPolly(serviceId, serviceType);
        }



        internal IAsyncPolicy SetupAllPolly(string serviceId, Type serviceType)
        {
            IAsyncPolicy asyncPolicy = Policy.NoOpAsync();
            asyncPolicy = SetupServiceTypePolly(asyncPolicy, serviceType);
            asyncPolicy = SetupServiceIdPolly(asyncPolicy, serviceId);
            asyncPolicy = SetupGlobalPolly(asyncPolicy);
            return asyncPolicy;
        }

        internal IAsyncPolicy SetupServiceTypePolly(IAsyncPolicy asyncPolicy, Type serviceType)
        {
            //serviceType
            if (_serviceTypePolicySetupsMap.TryGetValue(serviceType, out var setups))
            {
                setups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy));
            }
            if (_serviceTypePolicyAsyncSetupsMap.TryGetValue(serviceType, out var asyncSetups))
            {
                asyncSetups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy).ConfigureAwait(false).GetAwaiter().GetResult());
            }
            return asyncPolicy;
        }
        internal IAsyncPolicy SetupServiceIdPolly(IAsyncPolicy asyncPolicy, string serviceId)
        {
            //serviceId
            if (_serviceIdPolicySetupsMap.TryGetValue(serviceId, out var setups))
            {
                setups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy));
            }
            if (_serviceIdPolicyAsyncSetupsMap.TryGetValue(serviceId, out var asyncSetups))
            {
                asyncSetups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy).ConfigureAwait(false).GetAwaiter().GetResult());
            }
            return asyncPolicy;
        }
        internal IAsyncPolicy SetupGlobalPolly(IAsyncPolicy asyncPolicy)
        {
            //global
            _globalPolicySetups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy));
            _globalPolicyAsyncSetups.ForEach(setup => asyncPolicy = setup.Invoke(asyncPolicy).ConfigureAwait(false).GetAwaiter().GetResult());
            return asyncPolicy;
        }


    }
}
