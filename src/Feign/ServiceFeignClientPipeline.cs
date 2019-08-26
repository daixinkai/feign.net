using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    interface IServiceFeignClientPipeline<out TService>
    {
        Type ServiceType { get; }
    }
    class ServiceFeignClientPipeline<TService> : FeignClientPipelineBase<TService>, IFeignClientPipeline<TService>, IServiceFeignClientPipeline<TService>
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
