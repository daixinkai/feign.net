using Feign.Fallback;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public interface IFallbackRequestEventArgs<out TService> : IFeignClientEventArgs<TService>
    {

        FeignClientHttpRequest Request { get; }
        IFallbackProxy FallbackProxy { get; }
        TService Fallback { get; }

        MethodInfo Method { get; }

        IDictionary<string, object> GetParameters();

        Type[] GetParameterTypes();
        /// <summary>
        /// 是否终止降级
        /// </summary>
        bool IsTerminated { get; }
        /// <summary>
        /// 终止降级
        /// </summary>
        void Terminate();

    }
}
