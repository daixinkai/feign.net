using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Reflection
{
    /// <summary>
    /// Describes an agent type
    /// </summary>
    public class FeignClientTypeInfo
    {
        public FeignClientTypeInfo(FeignClientAttribute feignClient, Type serviceType, FeignClientLifetime lifetime)
        {
            FeignClient = feignClient;
            ServiceType = serviceType;
            Methods = new List<FeignClientMethodInfo>();
            Lifetime = lifetime;
        }
        public FeignClientAttribute FeignClient { get; }
        /// <summary>
        /// Gets service type
        /// </summary>
        public Type ServiceType { get; }
        /// <summary>
        /// Gets or sets the parent type
        /// </summary>
        public Type? ParentType { get; set; }
        /// <summary>
        /// Gets or sets the generated type
        /// </summary>
        public Type? BuildType { get; set; }
        /// <summary>
        /// Gets or sets the ProxyOptionsType
        /// </summary>
        public FeignClientProxyOptionsTypeInfo? ProxyOptionsType { get; set; }
        /// <summary>
        /// Gets method collection
        /// </summary>
        public List<FeignClientMethodInfo> Methods { get; }

        public FeignClientLifetime Lifetime { get; }

    }
}
