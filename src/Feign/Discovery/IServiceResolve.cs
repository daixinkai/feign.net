using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    /// <summary>
    /// An interface that represents the service resolve
    /// </summary>
    public interface IServiceResolve
    {
        /// <summary>
        /// Find the service path to use based on the service collection
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="services"></param>
        /// <returns></returns>
        Uri ResolveService(Uri uri, IList<IServiceInstance>? services);
    }
}
