using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    /// <summary>
    /// An interface that represents service discovery
    /// </summary>
    public interface IServiceDiscovery
    {
        /// <summary>
        /// serviceId collection of services
        /// </summary>
        IList<string>? Services { get; }
        /// <summary>
        /// Get the service object collection based on the specified serviceId
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        IList<IServiceInstance>? GetServiceInstances(string serviceId);

    }
}
