using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface IBuildingRequestMiddleware<T> : IFeignClientMiddleware<T>
    {
        ValueTask InvokeAsync(IBuildingRequestPipelineContext<T> context);
    }

}
