using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 映射一个POST请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class PostMappingAttribute : RequestMappingBaseAttribute
    {
        public PostMappingAttribute() { }
        public PostMappingAttribute(string value) : base(value)
        {
        }

        public override string GetMethod()
        {
            return "POST";
        }

    }
}
