using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// headers
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class HeadersAttribute : Attribute
    {
        public HeadersAttribute(params string[] headers)
        {
            Headers = headers;
        }
        public virtual string[] Headers { get; }
    }
}
