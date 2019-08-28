using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Fallback
{
    /// <summary>
    /// 一个接口,表示支持降级操作的服务对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFallbackFactoryFeignClient<TService> : IFeignClient<TService>
    {
        /// <summary>
        /// 获取降级服务提供者
        /// </summary>
        IFallbackFactory<TService> FallbackFactory { get; }
    }
}
