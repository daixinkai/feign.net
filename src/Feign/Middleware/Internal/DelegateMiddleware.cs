using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    internal class DelegateBuildingRequestMiddleware<T> : IBuildingRequestMiddleware<T>
    {
        public DelegateBuildingRequestMiddleware(BuildingRequestDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly BuildingRequestDelegate<T> _middleware;

        public ValueTask InvokeAsync(IBuildingRequestPipelineContext<T> context) => _middleware.Invoke(context);
    }

    internal class DelegateCancelRequestMiddleware<T> : ICancelRequestMiddleware<T>
    {
        public DelegateCancelRequestMiddleware(CancelRequestDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly CancelRequestDelegate<T> _middleware;

        public ValueTask InvokeAsync(ICancelRequestPipelineContext<T> context) => _middleware.Invoke(context);
    }

    internal class DelegateDisposingMiddleware<T> : IDisposingMiddleware<T>
    {
        public DelegateDisposingMiddleware(DisposingDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly DisposingDelegate<T> _middleware;

        public void Invoke(IDisposingPipelineContext<T> context) => _middleware.Invoke(context);
    }

    internal class DelegateErrorRequestMiddleware<T> : IErrorRequestMiddleware<T>
    {
        public DelegateErrorRequestMiddleware(ErrorRequestDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly ErrorRequestDelegate<T> _middleware;

        public ValueTask InvokeAsync(IErrorRequestPipelineContext<T> context) => _middleware.Invoke(context);
    }

    internal class DelegateFallbackRequestMiddleware<T> : IFallbackRequestMiddleware<T>
    {
        public DelegateFallbackRequestMiddleware(FallbackRequestDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly FallbackRequestDelegate<T> _middleware;

        public ValueTask InvokeAsync(IFallbackRequestPipelineContext<T> context) => _middleware.Invoke(context);
    }

    internal class DelegateInitializingMiddleware<T> : IInitializingMiddleware<T>
    {
        public DelegateInitializingMiddleware(InitializingDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly InitializingDelegate<T> _middleware;

        public void Invoke(IInitializingPipelineContext<T> context) => _middleware.Invoke(context);
    }

    internal class DelegateReceivedResponseMiddleware<T> : IReceivedResponseMiddleware<T>
    {
        public DelegateReceivedResponseMiddleware(ReceivedResponseDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly ReceivedResponseDelegate<T> _middleware;

        public ValueTask InvokeAsync(IReceivedResponsePipelineContext<T> context) => _middleware.Invoke(context);
    }

    internal class DelegateReceivingResponseMiddleware<T> : IReceivingResponseMiddleware<T>
    {
        public DelegateReceivingResponseMiddleware(ReceivingResponseDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly ReceivingResponseDelegate<T> _middleware;

        public ValueTask InvokeAsync(IReceivingResponsePipelineContext<T> context) => _middleware.Invoke(context);
    }

    internal class DelegateSendingRequestMiddleware<T> : ISendingRequestMiddleware<T>
    {
        public DelegateSendingRequestMiddleware(SendingRequestDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly SendingRequestDelegate<T> _middleware;

        public ValueTask InvokeAsync(ISendingRequestPipelineContext<T> context) => _middleware.Invoke(context);
    }

}
