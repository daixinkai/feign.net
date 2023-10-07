using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    /// <summary>
    /// mapping HTTP PUT requests
    /// </summary>
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
