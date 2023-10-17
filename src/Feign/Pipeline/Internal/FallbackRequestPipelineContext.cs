using Feign.Fallback;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
    /// <summary>
    /// Representing the fallback request pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>

    //#if NET5_0_OR_GREATER
    //    internal record FallbackRequestPipelineContext<TService> : FeignClientPipelineContext<TService>, IFallbackRequestPipelineContext<TService>
    //#else
    internal class FallbackRequestPipelineContext<TService> : FeignClientPipelineContext<TService>, IFallbackRequestPipelineContext<TService>
    //#endif
    {
        internal FallbackRequestPipelineContext(
            IFeignClient<TService> feignClient,
            FeignClientHttpRequest request,
            TService fallback,
            IFallbackProxy? fallbackProxy,
            MethodInfo? method,
            Exception exception
            ) : base(feignClient)
        {
            Request = request;
            Fallback = fallback;
            FallbackProxy = fallbackProxy;
            _method = method;
            Exception = exception;
        }
        /// <inheritdoc/>
        public FeignClientHttpRequest Request { get; }
        /// <inheritdoc/>
        public IFallbackProxy? FallbackProxy { get; }
        /// <inheritdoc/>
        public TService Fallback { get; }
        private MethodInfo? _method;
        /// <inheritdoc/>
        public MethodInfo Method
        {
            get
            {
                if (_method == null)
                {
                    _method = Fallback!.GetType().GetRequiredMethod(FallbackProxy!.MethodName, FallbackProxy.GetParameterTypes());
                }
                return _method;
            }
        }
        /// <inheritdoc/>
        public Exception Exception { get; }
        /// <inheritdoc/>
        public IDictionary<string, object> GetParameters()
        {
            return FallbackProxy?.GetParameters() ?? new Dictionary<string, object>();
        }
        /// <inheritdoc/>
        public Type[] GetParameterTypes()
        {
            return FallbackProxy?.GetParameterTypes() ?? Type.EmptyTypes;
        }
        /// <inheritdoc/>
        public bool IsTerminated => _isTerminated;

        private bool _isTerminated;
        /// <inheritdoc/>
        public void Terminate()
        {
            _isTerminated = true;
        }

    }
}
