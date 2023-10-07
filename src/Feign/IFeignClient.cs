using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    /// <summary>
    /// An interface that represents the service object
    /// </summary>
    public interface IFeignClient
    {
        /// <summary>
        /// Gets the serviceId
        /// </summary>
        string ServiceId { get; }
    }
    /// <summary>
    /// An interface that represents a service object of a known type
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
