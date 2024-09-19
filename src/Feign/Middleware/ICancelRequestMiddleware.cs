using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface ICancelRequestMiddleware<T> : IFeignClientMiddleware
    {
        ValueTask InvokeAsync(ICancelRequestPipelineContext<T> context);
    }

    public class DefaultCancelRequestMiddleware<T> : ICancelRequestMiddleware<T>
    {
        public DefaultCancelRequestMiddleware(CancelRequestDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly CancelRequestDelegate<T> _middleware;

        public ValueTask InvokeAsync(ICancelRequestPipelineContext<T> context) => _middleware.Invoke(context);
    }

}
