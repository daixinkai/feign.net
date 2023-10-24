using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// Global Pipeline
    /// </summary>
    public interface IGlobalFeignClientPipeline : IFeignClientPipeline<object>
    {
        /// <summary>
        /// Get the specified service Pipeline
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        IFeignClientPipeline<object> Service(string serviceId);
        /// <summary>
        /// Get the specified service Pipeline
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        IFeignClientPipeline<TService> Service<TService>();
    }
}
