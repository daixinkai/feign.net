using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// An interface representing the request pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IFeignClientPipelineContext<out TService>
    {
        /// <summary>
        /// Gets the service object
        /// </summary>
        IFeignClient<TService> FeignClient { get; }
    }
}
