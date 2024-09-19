using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface ISendingRequestMiddleware<T> : IFeignClientMiddleware
    {
        ValueTask InvokeAsync(ISendingRequestPipelineContext<T> context);
    }
    public class DefaultSendingRequestMiddleware<T> : ISendingRequestMiddleware<T>
    {
        public DefaultSendingRequestMiddleware(SendingRequestDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly SendingRequestDelegate<T> _middleware;

        public ValueTask InvokeAsync(ISendingRequestPipelineContext<T> context) => _middleware.Invoke(context);
    }

}
