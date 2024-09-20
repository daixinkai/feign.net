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

}
