using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    /// <summary>
    /// An interface that represents the service object
    /// </summary>
    public interface IFeignClient
    {
        /// <summary>
        /// Gets the serviceId
        /// </summary>
        string ServiceId { get; }
        /// <summary>
        /// Gets the collection of features provided by the  available on this client
        /// </summary>
        IDictionary<Type, object?> Features { get; }
    }
    /// <summary>
    /// An interface that represents a service object of a known type
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IFeignClient<out TService> : IFeignClient
    {
        /// <summary>
        /// Gets the service object
        /// </summary>
        TService Service { get; }
        /// <summary>
        /// Gets the service type
        /// </summary>
        Type ServiceType { get; }
    }
}
