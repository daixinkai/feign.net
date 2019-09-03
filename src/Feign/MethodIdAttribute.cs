using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 描述一个方法Id
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class MethodIdAttribute : Attribute
    {
        public MethodIdAttribute(string methodId)
        {
            MethodId = methodId;
        }
        /// <summary>
        /// 获取方法id
        /// </summary>
        public string MethodId { get; }
    }
}
