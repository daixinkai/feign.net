using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 请求参数将转换到QueryString中
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
        /// 获取参数名
        /// </summary>
        public string Name { get; }
    }
}
