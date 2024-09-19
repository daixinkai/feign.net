using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface IReceivingResponseMiddleware<T> : IFeignClientMiddleware
    {
        ValueTask InvokeAsync(IReceivingResponsePipelineContext<T> context);
    }
    public class DefaultReceivingResponseMiddleware<T> : IReceivingResponseMiddleware<T>
    {
        public DefaultReceivingResponseMiddleware(ReceivingResponseDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly ReceivingResponseDelegate<T> _middleware;

        public ValueTask InvokeAsync(IReceivingResponsePipelineContext<T> context) => _middleware.Invoke(context);
    }
}
