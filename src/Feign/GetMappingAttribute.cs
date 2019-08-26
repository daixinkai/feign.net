using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
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
