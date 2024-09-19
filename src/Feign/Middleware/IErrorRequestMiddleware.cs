using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface IErrorRequestMiddleware<T> : IFeignClientMiddleware<T>
    {
        ValueTask InvokeAsync(IErrorRequestPipelineContext<T> context);
    }
    public class DefaultErrorRequestMiddleware<T> : IErrorRequestMiddleware<T>
    {
        public DefaultErrorRequestMiddleware(ErrorRequestDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly ErrorRequestDelegate<T> _middleware;

        public ValueTask InvokeAsync(IErrorRequestPipelineContext<T> context) => _middleware.Invoke(context);
    }
}
