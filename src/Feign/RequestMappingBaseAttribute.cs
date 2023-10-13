using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    /// <summary>
    /// Map a request
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public abstract class RequestMappingBaseAttribute : Attribute, IRequestMapping
    {
        protected RequestMappingBaseAttribute()
        {
        }
        protected RequestMappingBaseAttribute(string value)
        {
            Value = value;
        }
        /// <summary>
        /// Get or set the request path
        /// </summary>
        public string? Value { get; set; }
        /// <summary>
        /// Gets or sets the ContentType
        /// </summary>
        public string? ContentType { get; set; }
        /// <summary>
        /// Gets or sets the Accept
        /// </summary>
        public string? Accept { get; set; }

        /// <summary>
        /// Gets or sets the HttpCompletionOption
        /// </summary>
        public HttpCompletionOption CompletionOption { get; set; }

        ///// <summary>
        ///// Gets or sets the UriKind
        ///// </summary>
        //public UriKind UriKind { get; set; }

        /// <summary>
        /// Gets the http method of the request
        /// </summary>
        /// <returns></returns>
        public abstract string GetMethod();

    }
}
