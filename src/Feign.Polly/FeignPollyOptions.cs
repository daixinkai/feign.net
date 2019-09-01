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

        internal readonly List<Func<IAsyncPolicy, IAsyncPolicy>> _globalPolicySetups = new List<Func<IAsyncPolicy, IAsyncPolicy>>();
        internal readonly List<Func<IAsyncPolicy, Task<IAsyncPolicy>>> _globalPolicyAsyncSetups = new List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>();

        internal readonly ConcurrentDictionary<string, List<Func<IAsyncPolicy, IAsyncPolicy>>> _serviceIdPolicySetupsMap = new ConcurrentDictionary<string, List<Func<IAsyncPolicy, IAsyncPolicy>>>();
        internal readonly ConcurrentDictionary<string, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>> _serviceIdPolicyAsyncSetupsMap = new ConcurrentDictionary<string, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>>();


        internal readonly ConcurrentDictionary<Type, List<Func<IAsyncPolicy, IAsyncPolicy>>> _serviceTypePolicySetupsMap = new ConcurrentDictionary<Type, List<Func<IAsyncPolicy, IAsyncPolicy>>>();
        internal readonly ConcurrentDictionary<Type, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>> _serviceTypePolicyAsyncSetupsMap = new ConcurrentDictionary<Type, List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>>();

        public void Configure(Func<IAsyncPolicy, IAsyncPolicy> setup)
        {
            if (setup != null)
            {
                _globalPolicySetups.Add(setup);
            }
        }

        public void Configure(string serviceId, Func<IAsyncPolicy, IAsyncPolicy> setup)
        {
            var setups = _serviceIdPolicySetupsMap.GetOrAdd(serviceId, key => new List<Func<IAsyncPolicy, IAsyncPolicy>>());
            setups.Add(setup);
        }

        public void Configure<TService>(Func<IAsyncPolicy, IAsyncPolicy> setup)
        {
            var setups = _serviceTypePolicySetupsMap.GetOrAdd(typeof(TService), key => new List<Func<IAsyncPolicy, IAsyncPolicy>>());
            setups.Add(setup);
        }

        public void ConfigureAsync(Func<IAsyncPolicy, Task<IAsyncPolicy>> setup)
        {
            if (setup != null)
            {
                _globalPolicyAsyncSetups.Add(setup);
            }
        }

        public void ConfigureAsync(string serviceId, Func<IAsyncPolicy, Task<IAsyncPolicy>> setup)
        {
            var setups = _serviceIdPolicyAsyncSetupsMap.GetOrAdd(serviceId, key => new List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>());
            setups.Add(setup);
        }

        public void ConfigureAsync<TService>(Func<IAsyncPolicy, Task<IAsyncPolicy>> setup)
        {
            var setups = _serviceTypePolicyAsyncSetupsMap.GetOrAdd(typeof(TService), key => new List<Func<IAsyncPolicy, Task<IAsyncPolicy>>>());
            setups.Add(setup);
        }

        internal void SetupAllPolly(IFeignClient<object> feignClient, PollyDelegatingHandler pollyDelegatingHandler)
        {
            SetupServiceTypePolly(feignClient, pollyDelegatingHandler);
            SetupServiceIdPolly(feignClient, pollyDelegatingHandler);
            SetupGlobalPolly(feignClient, pollyDelegatingHandler);
        }

        internal void SetupServiceTypePolly(IFeignClient<object> feignClient, PollyDelegatingHandler pollyDelegatingHandler)
        {
            List<Func<IAsyncPolicy, IAsyncPolicy>> setups;
            List<Func<IAsyncPolicy, Task<IAsyncPolicy>>> asyncSetups;
            //serviceType
            if (_serviceTypePolicySetupsMap.TryGetValue(feignClient.ServiceType, out setups))
            {
                setups.ForEach(setup => pollyDelegatingHandler._asyncPolicy = setup.Invoke(pollyDelegatingHandler.AsyncPolicy));
            }
            if (_serviceTypePolicyAsyncSetupsMap.TryGetValue(feignClient.ServiceType, out asyncSetups))
            {
                asyncSetups.ForEach(setup => pollyDelegatingHandler._asyncPolicy = setup.Invoke(pollyDelegatingHandler.AsyncPolicy).ConfigureAwait(false).GetAwaiter().GetResult());
            }
        }
        internal void SetupServiceIdPolly(IFeignClient<object> feignClient, PollyDelegatingHandler pollyDelegatingHandler)
        {
            List<Func<IAsyncPolicy, IAsyncPolicy>> setups;
            List<Func<IAsyncPolicy, Task<IAsyncPolicy>>> asyncSetups;
            //serviceId
            if (_serviceIdPolicySetupsMap.TryGetValue(feignClient.ServiceId, out setups))
            {
                setups.ForEach(setup => pollyDelegatingHandler._asyncPolicy = setup.Invoke(pollyDelegatingHandler.AsyncPolicy));
            }
            if (_serviceIdPolicyAsyncSetupsMap.TryGetValue(feignClient.ServiceId, out asyncSetups))
            {
                asyncSetups.ForEach(setup => pollyDelegatingHandler._asyncPolicy = setup.Invoke(pollyDelegatingHandler.AsyncPolicy).ConfigureAwait(false).GetAwaiter().GetResult());
            }
        }
        internal void SetupGlobalPolly(IFeignClient<object> feignClient, PollyDelegatingHandler pollyDelegatingHandler)
        {
            //global
            _globalPolicySetups.ForEach(setup => pollyDelegatingHandler._asyncPolicy = setup.Invoke(pollyDelegatingHandler.AsyncPolicy));
            _globalPolicyAsyncSetups.ForEach(setup => pollyDelegatingHandler._asyncPolicy = setup.Invoke(pollyDelegatingHandler.AsyncPolicy).ConfigureAwait(false).GetAwaiter().GetResult());
        }

    }
}
