using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// 一个接口,表示服务事件参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IFeignClientPipelineContext<out TService>
    {
        /// <summary>
        /// 获取服务对象
        /// </summary>
        IFeignClient<TService> FeignClient { get; }
    }
}
