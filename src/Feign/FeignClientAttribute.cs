﻿using System;
using System.Collections.Generic;
using Feign.Configuration;

namespace Feign
{
    /// <summary>
    /// a feign client service
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public class FeignClientAttribute : Attribute
    {
        public FeignClientAttribute(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
        }

        public FeignClientAttribute(string name, FeignClientLifetime lifetime)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            Lifetime = lifetime;
        }

        /// <summary>
        /// gets the serviceId
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// gets the lifetime of a service
        /// </summary>
        public FeignClientLifetime? Lifetime { get; }
        /// <summary>
        /// gets or sets the service url
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// <para>gets or sets the UriKind. default value is <see cref="UriKind.Relative"/></para>
        /// <para><see cref="UriKind.Relative"/> : "api/user"+"create"="api/user/create" ; "api/user"+"/create"="api/user/create"</para>
        /// <para><see cref="UriKind.Absolute"/> : "api/user"+"create"="create" ; "api/user"+"/create"="create"</para>
        /// <para><see cref="UriKind.RelativeOrAbsolute"/> : "api/user"+"create"="api/user/create" ; "api/user"+"/create"="create"</para>
        /// </summary>
        public UriKind UriKind { get; set; } = UriKind.Relative;
        /// <summary>
        /// gets or sets the service fallback type
        /// </summary>
        public Type? Fallback { get; set; }
        /// <summary>
        /// <para>gets or sets the service fallback factory type</para>
        /// <para><see cref="Fallback.IFallbackFactory{T}"/></para>
        /// </summary>
        public Type? FallbackFactory { get; set; }
        /// <summary>
        /// A custom <see cref="IFeignClientConfiguration"/> or <see cref="IFeignClientConfiguration{T}"/> class for the feign client. 
        /// </summary>
        public Type? Configuration { get; set; }
        /// <summary>
        /// whether 404s should be decoded instead of throwing FeignExceptions
        /// </summary>
        public bool Dismiss404 { get; set; }
    }
}
