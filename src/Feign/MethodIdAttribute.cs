using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// Describe a method Id
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class MethodIdAttribute : Attribute
    {
        public MethodIdAttribute(string methodId)
        {
            MethodId = methodId;
        }
        /// <summary>
        /// Gets the method Id
        /// </summary>
        public string MethodId { get; }
    }
}
