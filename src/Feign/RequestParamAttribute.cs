using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// Request parameters will be transmitted as RequestQuery
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
    public sealed class RequestParamAttribute : Attribute, IRequestParameter
    {
        public RequestParamAttribute()
        {
        }
        public RequestParamAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Get or set the parameter name
        /// </summary>
        public string? Name { get; }
    }
}
