using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    /// <summary>
    /// 一个接口,表示代理类型生成器
    /// </summary>
    public interface IFeignClientTypeBuilder
    {
        bool IsServiceType(Type type);
        /// <summary>
        /// 将指定的服务生成代理类型
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        FeignClientTypeInfo? Build(Type serviceType, FeignClientLifetime lifetime);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="feignClientTypeInfo"></param>
        /// <returns></returns>
        Type BuildKeyedType(string key, FeignClientTypeInfo feignClientTypeInfo);
    }
}
