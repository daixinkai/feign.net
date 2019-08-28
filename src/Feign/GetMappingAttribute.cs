using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 映射一个GET请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class GetMappingAttribute : RequestMappingBaseAttribute
    {
        public GetMappingAttribute() { }
        public GetMappingAttribute(string value) : base(value)
        {
        }

        public override string GetMethod()
        {
            return "GET";
        }
    }
}
