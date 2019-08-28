using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 请求参数将作为RequestBody传输
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public sealed class RequestBodyAttribute : Attribute, IRequestParameter
    {
    }
}
