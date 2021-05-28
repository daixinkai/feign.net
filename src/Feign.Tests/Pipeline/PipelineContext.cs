using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    public abstract class PipelineContext<TService> : IPipelineContext<TService>
    {
        protected PipelineContext(IFeignClient<TService> feignClient)
        {
            FeignClient = feignClient;
        }
        /// <summary>
        /// 获取服务对象
        /// </summary>
        public IFeignClient<TService> FeignClient { get; }
    }
}
