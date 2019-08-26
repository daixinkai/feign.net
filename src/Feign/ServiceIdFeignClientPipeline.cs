using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    class ServiceIdFeignClientPipeline : FeignClientPipelineBase<object>, IFeignClientPipeline<object>
    {
        public ServiceIdFeignClientPipeline(string serviceId)
        {
            ServiceId = serviceId;
        }
        public string ServiceId { get; }
    }
}
