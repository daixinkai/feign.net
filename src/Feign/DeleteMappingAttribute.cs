using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 映射一个DELETE请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class DeleteMappingAttribute : RequestMappingBaseAttribute
    {
        public DeleteMappingAttribute() { }
        public DeleteMappingAttribute(string value) : base(value)
        {
        }

        public override string GetMethod()
        {
            return "DELETE";
        }

    }
}
