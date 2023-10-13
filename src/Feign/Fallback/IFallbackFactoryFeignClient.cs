using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Fallback
{
    /// <summary>
    /// An interface representing a service object that supports service fallback
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IFallbackFactoryFeignClient<TService> : IFeignClient<TService>
    {
        /// <summary>
        /// Gets a fallback service factory
        /// </summary>
        IFallbackFactory<TService> FallbackFactory { get; }
    }
}
