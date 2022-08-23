using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 映射一个PATCH请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class PatchMappingAttribute : RequestMappingBaseAttribute
    {
        public PatchMappingAttribute() { }
        public PatchMappingAttribute(string value) : base(value)
        {
        }

        public override string GetMethod()
        {
            return "PATCH";
        }
    }
}
