using Feign.Middleware;
using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    public class TestFeignClientMiddleware<T> :
        IBuildingRequestMiddleware<T>,
        ICancelRequestMiddleware<T>,
        IDisposingMiddleware<T>,
        IErrorRequestMiddleware<T>,
        IFallbackRequestMiddleware<T>,
        IInitializingMiddleware<T>,
        IReceivingResponseMiddleware<T>,
        IReceivedResponseMiddleware<T>,
        ISendingRequestMiddleware<T>
    {
        public void Invoke(IDisposingPipelineContext<T> context)
        {

        }

        public void Invoke(IInitializingPipelineContext<T> context)
        {

        }

        public ValueTask InvokeAsync(IBuildingRequestPipelineContext<T> context)
        {
            return default;
        }

        public ValueTask InvokeAsync(ICancelRequestPipelineContext<T> context)
        {
            return default;
        }

        public ValueTask InvokeAsync(IErrorRequestPipelineContext<T> context)
        {
            return default;
        }

        public ValueTask InvokeAsync(IFallbackRequestPipelineContext<T> context)
        {
            return default;
        }

        public ValueTask InvokeAsync(IReceivingResponsePipelineContext<T> context)
        {
            return default;
        }

        public ValueTask InvokeAsync(IReceivedResponsePipelineContext<T> context)
        {
            return default;
        }

        public ValueTask InvokeAsync(ISendingRequestPipelineContext<T> context)
        {
            return default;
        }
    }
}
