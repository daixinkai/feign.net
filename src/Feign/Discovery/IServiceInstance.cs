using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    /// <summary>
    /// An interface that represents a service
    /// </summary>
    public interface IServiceInstance
    {
        /// <summary>
        /// Gets the serviceId of the service
        /// </summary>
        string ServiceId { get; }
        /// <summary>
        /// Gets the host of the service
        /// </summary>
        string Host { get; }
        /// <summary>
        /// Gets the port of the service
        /// </summary>
        int Port { get; }
        /// <summary>
        /// Gets the uri of the service
        /// </summary>
        Uri Uri { get; }
    }
}
