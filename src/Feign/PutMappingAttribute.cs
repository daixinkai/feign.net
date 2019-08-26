using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class PutMappingAttribute : RequestMappingBaseAttribute
    {
        public PutMappingAttribute() { }
        public PutMappingAttribute(string value) : base(value)
        {
        }

        public override string GetMethod()
        {
            return "PUT";
        }

    }
}
