using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    public class RequestMappingAttribute : RequestMappingBaseAttribute
    {
        public RequestMappingAttribute() { }
        public RequestMappingAttribute(string value) : this(value, "GET")
        {
        }
        public RequestMappingAttribute(string value, string method) : base(value)
        {
            Method = method;
        }
        public string Method { get; set; }

        public override string GetMethod()
        {
            return Method;
        }
    }
}
