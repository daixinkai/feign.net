using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    /// <summary>
    /// 一个接口,表示服务发现
    /// </summary>
    public interface IServiceDiscovery
    {
        /// <summary>
        /// 服务的serviceId 集合
        /// </summary>
        IList<string> Services { get; }
        /// <summary>
        /// 根据指定的serviceId获取服务对象集合
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        IList<IServiceInstance> GetServiceInstances(string serviceId);

    }
}
