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
        /// <summary>
        /// Get the specified service Pipeline
        /// </summary>
        /// <param name="key"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        IFeignClientPipeline<object> KeyedService(string key, string serviceId);
        /// <summary>
        /// Get the specified service Pipeline
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        IFeignClientPipeline<TService> KeyedService<TService>(string key);
    }
}
