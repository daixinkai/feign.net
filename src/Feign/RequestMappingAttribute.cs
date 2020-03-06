using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 映射一个请求
    /// </summary>
    public class RequestMappingAttribute : RequestMappingBaseAttribute
    {
        public RequestMappingAttribute() : this(null) { }
        public RequestMappingAttribute(string value) : this(value, "GET")
        {
        }
        public RequestMappingAttribute(string value, string method) : base(value)
        {
            Method = method;
        }
        /// <summary>
        /// 获取或设置请求的http方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 获取请求的http方法
        /// </summary>
        public override string GetMethod()
        {
            return Method;
        }
    }
}
