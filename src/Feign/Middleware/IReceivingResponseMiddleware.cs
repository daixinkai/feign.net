using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface IReceivingResponseMiddleware<T> : IFeignClientMiddleware<T>
    {
        ValueTask InvokeAsync(IReceivingResponsePipelineContext<T> context);
    }

}
