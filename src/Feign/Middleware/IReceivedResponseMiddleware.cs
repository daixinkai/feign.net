using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface IReceivedResponseMiddleware<T> : IFeignClientMiddleware<T>
    {
        ValueTask InvokeAsync(IReceivedResponsePipelineContext<T> context);
    }
    public class DefaultReceivedResponseMiddleware<T> : IReceivedResponseMiddleware<T>
    {
        public DefaultReceivedResponseMiddleware(ReceivedResponseDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly ReceivedResponseDelegate<T> _middleware;

        public ValueTask InvokeAsync(IReceivedResponsePipelineContext<T> context) => _middleware.Invoke(context);
    }
}
