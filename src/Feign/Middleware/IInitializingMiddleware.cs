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

}
