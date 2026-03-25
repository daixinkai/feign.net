using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
    internal class ServiceFeignClientPipeline<TService> : FeignClientPipelineBase<TService>, IFeignClientPipeline<TService>
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
