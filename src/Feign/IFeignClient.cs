using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 一个接口,表示服务对象
    /// </summary>
    public interface IFeignClient
    {
        /// <summary>
        /// Gets the serviceId
        /// </summary>
        string ServiceId { get; }
    }
    /// <summary>
    /// 一个接口,表示已知类型的服务对象
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IFeignClient<out TService> : IFeignClient
    {
        /// <summary>
        /// 获取服务对象
        /// </summary>
        TService Service { get; }
        /// <summary>
        /// 获取服务类型
        /// </summary>
        Type ServiceType { get; }
    }
}
