using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface IFallbackRequestMiddleware<T> : IFeignClientMiddleware
    {
        ValueTask InvokeAsync(IFallbackRequestPipelineContext<T> context);
    }
    public class DefaultFallbackRequestMiddleware<T> : IFallbackRequestMiddleware<T>
    {
        public DefaultFallbackRequestMiddleware(FallbackRequestDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly FallbackRequestDelegate<T> _middleware;

        public ValueTask InvokeAsync(IFallbackRequestPipelineContext<T> context) => _middleware.Invoke(context);
    }
}
