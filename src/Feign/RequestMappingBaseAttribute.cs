using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 映射一个请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public abstract class RequestMappingBaseAttribute : Attribute, IRequestMapping
    {
        protected RequestMappingBaseAttribute()
        { }

        protected RequestMappingBaseAttribute(string value)
        {
            Value = value;
        }
        /// <summary>
        /// 获取或设置请求路径
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 获取或设置ContentType
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 获取或设置Accept
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// 获取或设置HttpCompletionOption
        /// </summary>
        public HttpCompletionOption CompletionOption { get; set; }

        /// <summary>
        /// 获取请求的http方法
        /// </summary>
        /// <returns></returns>
        public abstract string GetMethod();

    }
}
