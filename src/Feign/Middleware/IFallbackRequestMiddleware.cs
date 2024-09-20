using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface IFallbackRequestMiddleware<T> : IFeignClientMiddleware<T>
    {
        ValueTask InvokeAsync(IFallbackRequestPipelineContext<T> context);
    }

}
