using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Fallback
{
    public interface IFallbackProxy
    {
        IDictionary<string, object> GetParameters();
        Type[] GetParameterTypes();
        string MethodName { get; }
        Type ReturnType { get; }
    }
}
