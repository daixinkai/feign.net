using Feign.Discovery.LoadBalancing;
using Feign.Formatting;
using Feign.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public interface IFeignOptions
    {
        /// <summary>
        /// Gets the Assemblies
        /// </summary>
        IList<Assembly> Assemblies { get; }
        /// <summary>
        /// Gets the Converters
        /// </summary>
        ConverterCollection Converters { get; }
        /// <summary>
        /// Gets the MediaTypeFormatters
        /// </summary>
        MediaTypeFormatterCollection MediaTypeFormatters { get; }
        /// <summary>
        /// Gets the Global Pipeline
        /// </summary>
        IGlobalFeignClientPipeline FeignClientPipeline { get; }
        /// <summary>
        /// Gets or sets the Lifetime.
        /// default value is <see cref="FeignClientLifetime.Singleton"/>
        /// </summary>
        FeignClientLifetime Lifetime { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether method metadata for declared services needs to be included. 
        /// Default: false
        /// </summary>
        bool IncludeMethodMetadata { get; set; }

        /// <summary>
        /// Gets or sets the PropertyNamingPolicy
        /// </summary>
        NamingPolicy PropertyNamingPolicy { get; set; }

        /// <summary>
        /// Gets or sets the JsonProvider.
        /// default value is <see cref="JsonProviderType"/>
        /// </summary>
        IJsonProvider JsonProvider { get; set; }

        /// <summary>
        /// Gets or sets the Types
        /// </summary>
        IList<FeignClientTypeInfo> Types { get; }
        /// <summary>
        /// Gets or sets the DiscoverServiceCacheTime.
        /// default value : 10min.
        /// Set to null to not use cache
        /// </summary>
        TimeSpan? DiscoverServiceCacheTime { get; set; }

        /// <summary>
        /// Is enable url encode (like : RequestQuery,PathVariable)
        /// </summary>
        bool UseUrlEncode { get; set; }

        /// <summary>
        ///   Gets or sets the type of decompression method used by the handler for automatic decompression of the HTTP content response.
        ///   Default value : null
        /// </summary>
        DecompressionMethods? AutomaticDecompression { get; set; }

        /// <summary>
        /// HttpClient.Handler.UseCookies
        /// </summary>
        bool? UseCookies { get; set; }

        /// <summary>
        /// Gets or sets Load balancing policy.
        /// Default value : <see cref="LoadBalancingPolicy.Random"/>
        /// </summary>
        LoadBalancingPolicy LoadBalancingPolicy { get; set; }

    }
}
