using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// Representing the request pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
//#if NET5_0_OR_GREATER
//    public record FeignClientPipelineContext<TService> : IFeignClientPipelineContext<TService>
//#else
    public abstract class FeignClientPipelineContext<TService> : IFeignClientPipelineContext<TService>
//#endif
    {
        protected FeignClientPipelineContext(IFeignClient<TService> feignClient)
        {
            FeignClient = feignClient;
        }
        /// <summary>
        /// Gets the service object
        /// </summary>
        public IFeignClient<TService> FeignClient { get; }
    }
}
