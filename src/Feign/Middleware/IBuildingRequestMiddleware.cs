using Feign.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Middleware
{
    public interface IBuildingRequestMiddleware<T> : IFeignClientMiddleware
    {
        ValueTask InvokeAsync(IBuildingRequestPipelineContext<T> context);
    }

    public class DefaultBuildingRequestMiddleware<T> : IBuildingRequestMiddleware<T>
    {
        public DefaultBuildingRequestMiddleware(BuildingRequestDelegate<T> middleware)
        {
            _middleware = middleware;
        }

        private readonly BuildingRequestDelegate<T> _middleware;

        public ValueTask InvokeAsync(IBuildingRequestPipelineContext<T> context) => _middleware.Invoke(context);
    }

}
