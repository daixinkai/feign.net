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
        public virtual string Name { get; }
        public virtual string Url { get; set; }
        public virtual Type Fallback { get; set; }
        public virtual Type FallbackFactory { get; set; }

        public FeignClientLifetime? Lifetime { get; set; }

    }
}
