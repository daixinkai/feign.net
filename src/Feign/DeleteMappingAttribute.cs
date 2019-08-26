using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
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
