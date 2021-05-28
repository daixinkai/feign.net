using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// 一个接口,表示服务管道Context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IPipelineContext<out TService>
    {
        /// <summary>
        /// 获取服务对象
        /// </summary>
        IFeignClient<TService> FeignClient { get; }
    }
}
