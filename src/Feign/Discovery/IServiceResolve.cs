using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    /// <summary>
    /// 一个接口,表示服务决定者
    /// </summary>
    public interface IServiceResolve
    {
        /// <summary>
        /// 根据服务集合查找要使用的服务路径
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        Uri ResolveService(Uri uri, IList<IServiceInstance>? services);
    }
}
