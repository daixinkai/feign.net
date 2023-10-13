using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Fallback
{
    /// <summary>
    /// An interface representing a service object that supports service
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFallbackFeignClient<out T> : IFeignClient<T>
    {
        /// <summary>
        /// Gets a fallback service
        /// </summary>
        T Fallback { get; }
    }
}
