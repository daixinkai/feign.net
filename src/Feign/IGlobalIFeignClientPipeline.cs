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
        /// 获取指定的服务Pipeline
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IFeignClientPipeline<object>? GetServicePipeline(string serviceId);
        /// <summary>
        /// 获取指定的服务Pipeline
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IFeignClientPipeline<object> GetOrAddServicePipeline(string serviceId);
        /// <summary>
        /// 获取指定的服务Pipeline
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IFeignClientPipeline<TService>? GetServicePipeline<TService>();
        /// <summary>
        /// 获取指定的服务Pipeline
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IFeignClientPipeline<TService> GetOrAddServicePipeline<TService>();
    }
}
