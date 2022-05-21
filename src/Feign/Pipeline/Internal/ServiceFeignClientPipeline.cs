using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
    internal interface IServiceFeignClientPipeline<out TService>
    {
        Type ServiceType { get; }
    }
    internal class ServiceFeignClientPipeline<TService> : FeignClientPipelineBase<TService>, IFeignClientPipeline<TService>, IServiceFeignClientPipeline<TService>
    {
        public ServiceFeignClientPipeline(Type serviceType)
        {
            ServiceType = serviceType;
        }
        public ServiceFeignClientPipeline() : this(typeof(TService))
        {
        }
        public Type ServiceType { get; }
    }

}
