using Feign.Middleware;
using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    public class TestFeignClientMiddleware
    {
        public class BuildingRequestMiddleware<T> : IBuildingRequestMiddleware<T>
        {
            public ValueTask InvokeAsync(IBuildingRequestPipelineContext<T> context)
            {
                throw new NotImplementedException();
            }
        }
    }
}
