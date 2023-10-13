using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    /// <summary>
    /// Map a request
    /// </summary>
    public class RequestMappingAttribute : RequestMappingBaseAttribute
    {
        public RequestMappingAttribute()
        {
            Method = "GET";
        }
        public RequestMappingAttribute(string value) : this(value, "GET")
        {
        }
        public RequestMappingAttribute(string value, string method) : base(value)
        {
            Method = method;
        }
        /// <summary>
        /// Gets or sets the http method of the request
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Gets the http method of the request
        /// </summary>
        public override string GetMethod()
        {
            return Method;
        }
    }
}
