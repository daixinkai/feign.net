using Feign.Fallback;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public class FallbackRequestEventArgs<TService> : FeignClientEventArgs<TService>, IFallbackRequestEventArgs<TService>
    {
        public FallbackRequestEventArgs(IFeignClient<TService> feignClient, FeignClientHttpRequest request, TService fallback, IFallbackProxy fallbackProxy, MethodInfo method) : base(feignClient)
        {
            Request = request;
            Fallback = fallback;
            FallbackProxy = fallbackProxy;
            _method = method;
        }
        public FeignClientHttpRequest Request { get; }
        public IFallbackProxy FallbackProxy { get; }
        public TService Fallback { get; }
        MethodInfo _method;
        public MethodInfo Method
        {
            get
            {
                if (_method == null)
                {
                    _method = Fallback.GetType().GetMethod(FallbackProxy.MethodName, FallbackProxy.GetParameterTypes());
                }
                return _method;
            }
        }

        public IDictionary<string, object> GetParameters()
        {
            return FallbackProxy?.GetParameters() ?? new Dictionary<string, object>();
        }

        public Type[] GetParameterTypes()
        {
            return FallbackProxy?.GetParameterTypes() ?? Type.EmptyTypes;
        }

        public bool IsTerminated => _isTerminated;

        bool _isTerminated;

        public void Terminate()
        {
            _isTerminated = true;
        }

    }
}
