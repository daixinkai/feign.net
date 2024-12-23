using Feign.Discovery.LoadBalancing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    public class FeignRequestOptions
    {
        /// <summary>
        /// Gets or sets the DiscoverServiceCacheTime.
        /// default value : 10min.
        /// Set to null to not use cache
        /// </summary>
        public TimeSpan? DiscoverServiceCacheTime { get; set; }
        /// <summary>
        /// Is enable url encode (like : RequestQuery,PathVariable)
        /// </summary>
        public bool UseUrlEncode { get; set; }

        /// <summary>
        ///   Gets or sets the type of decompression method used by the handler for automatic decompression of the HTTP content response.
        ///   Default value : null
        /// </summary>
        public DecompressionMethods? AutomaticDecompression { get; set; }

        /// <summary>
        /// HttpClient.Handler.UseCookies
        /// </summary>
        public bool? UseCookies { get; set; }
        /// <summary>
        /// Gets or sets Load balancing policy.
        /// Default value : <see cref="LoadBalancingPolicy.Random"/>
        /// </summary>
        public LoadBalancingPolicy LoadBalancingPolicy { get; set; }
        /// <summary>
        /// Is include root ParameterName when RequestQuery is Object
        /// <para>true : ?param.Id=1&amp;param.Name=2</para>
        /// <para>false : ?Id=1&amp;Name=2</para>
        /// <para>default value is false</para>
        /// </summary>
        public bool IncludeRootParameterName { get; set; }

    }
}
