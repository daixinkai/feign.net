using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface IInitializingMiddleware<T> : IFeignClientMiddleware<T>
    {
        void Invoke(IInitializingPipelineContext<T> context);
    }
    public class DefaultInitializingMiddleware<T> : IInitializingMiddleware<T>
    {
        public DefaultInitializingMiddleware(InitializingDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly InitializingDelegate<T> _middleware;

        public void Invoke(IInitializingPipelineContext<T> context) => _middleware.Invoke(context);
    }
}
