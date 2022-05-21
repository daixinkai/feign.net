using Feign.Fallback;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// 一个接口,表示服务降级时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IFallbackRequestPipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        /// <summary>
        /// 获取请求对象
        /// </summary>
        FeignClientHttpRequest Request { get; }
        /// <summary>
        /// 获取降级代理对象
        /// </summary>
        IFallbackProxy FallbackProxy { get; }
        /// <summary>
        /// 获取降级服务对象
        /// </summary>
        TService Fallback { get; }
        /// <summary>
        /// 获取降级的服务方法
        /// </summary>
        MethodInfo Method { get; }
        /// <summary>
        /// 获取请求的参数描述
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetParameters();
        /// <summary>
        /// 获取请求的参数类型
        /// </summary>
        /// <returns></returns>
        Type[] GetParameterTypes();
        /// <summary>
        /// 获取一个值,指示是否终止降级
        /// </summary>
        bool IsTerminated { get; }
        /// <summary>
        /// 终止降级
        /// </summary>
        void Terminate();

    }
}
