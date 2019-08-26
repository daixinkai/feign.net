using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// Specifies the lifetime of a service
    /// </summary>
    public enum FeignClientLifetime
    {
        /// <summary>
        /// Specifies that a single instance of the service will be created.   
        /// </summary>
        Singleton = 0,
        /// <summary>
        /// Specifies that a new instance of the service will be created for each scope.
        /// </summary>
        Scoped = 1,
        /// <summary>
        /// Specifies that a new instance of the service will be created every time it is
        /// </summary>
        Transient = 2
    }
}
