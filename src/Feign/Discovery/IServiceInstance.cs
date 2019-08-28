using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    /// <summary>
    /// 一个接口,表示服务
    /// </summary>
    public interface IServiceInstance
    {
        /// <summary>
        /// 获取服务的serviceId
        /// </summary>
        string ServiceId { get; }
        /// <summary>
        /// 获取服务host
        /// </summary>
        string Host { get; }
        /// <summary>
        /// 获取服务port
        /// </summary>
        int Port { get; }
        /// <summary>
        /// 获取服务地址
        /// </summary>
        Uri Uri { get; }
    }
}
