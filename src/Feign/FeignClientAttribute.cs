using System;

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
        public virtual string Name { get; }
        /// <summary>
        /// gets or sets the service url
        /// </summary>
        public virtual string Url { get; set; }
        /// <summary>
        /// gets or sets the service fallback type
        /// </summary>
        public virtual Type Fallback { get; set; }
        /// <summary>
        /// gets or sets the service fallback factory type
        /// </summary>
        public virtual Type FallbackFactory { get; set; }
        /// <summary>
        /// gets or sets the lifetime of a service
        /// </summary>
        public FeignClientLifetime? Lifetime { get; set; }

    }
}
