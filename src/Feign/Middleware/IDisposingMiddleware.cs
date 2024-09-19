using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface IDisposingMiddleware<T> : IFeignClientMiddleware<T>
    {
        void Invoke(IDisposingPipelineContext<T> context);
    }

    public class DefaultDisposingMiddleware<T> : IDisposingMiddleware<T>
    {
        public DefaultDisposingMiddleware(DisposingDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly DisposingDelegate<T> _middleware;

        public void Invoke(IDisposingPipelineContext<T> context) => _middleware.Invoke(context);
    }

}
