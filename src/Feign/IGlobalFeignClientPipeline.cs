using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 全局Pipeline
    /// </summary>
    public interface IGlobalFeignClientPipeline : IFeignClientPipeline<object>
    {
        IFeignClientPipeline<object> GetServicePipeline(string serviceId);
        IFeignClientPipeline<object> GetOrAddServicePipeline(string serviceId);
        IFeignClientPipeline<TService> GetServicePipeline<TService>();
        IFeignClientPipeline<TService> GetOrAddServicePipeline<TService>();
    }
}
