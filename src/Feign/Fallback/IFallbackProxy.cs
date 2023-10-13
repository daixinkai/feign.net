using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Fallback
{
    /// <summary>
    /// An interface that represents the service fallback proxy
    /// </summary>
    public interface IFallbackProxy
    {
        IDictionary<string, object> GetParameters();
        Type[] GetParameterTypes();
        string MethodName { get; }
        Type ReturnType { get; }
    }
}
