using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Fallback
{
    /// <summary>
    /// 一个接口,表示服务降级代理描述对象
    /// </summary>
    public interface IFallbackProxy
    {
        IDictionary<string, object> GetParameters();
        Type[] GetParameterTypes();
        string MethodName { get; }
        Type ReturnType { get; }
    }
}
