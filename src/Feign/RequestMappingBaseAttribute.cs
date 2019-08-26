using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public abstract class RequestMappingBaseAttribute : Attribute, IRequestMapping
    {
        protected RequestMappingBaseAttribute()
        { }

        protected RequestMappingBaseAttribute(string value)
        {
            Value = value;
        }
        public string Value { get; set; }

        public string ContentType { get; set; }

        public abstract string GetMethod();

    }
}
