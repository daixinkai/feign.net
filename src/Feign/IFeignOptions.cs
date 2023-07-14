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
#if USE_SYSTEM_TEXT_JSON
using JsonProviderType = Feign.Formatting.SystemTextJsonProvider;
#else
using JsonProviderType = Feign.Formatting.NewtonsoftJsonProvider;
#endif

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
        /// 获取或设置一个值,指示是否需要包含声明服务的方法元数据 默认 : false
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
        /// Is enable url encode (如 : RequestQuery,PathVariable)
        /// </summary>
        bool UseUrlEncode { get; set; }

        /// <summary>
        ///   Gets or sets the type of decompression method used by the handler for automatic decompression of the HTTP content response.
        ///   Default value : null
        /// </summary>
        DecompressionMethods? AutomaticDecompression { get; set; }

        /// <summary>
        /// 默认HttpClientHandler的UseCookies值
        /// </summary>
        bool? UseCookies { get; set; }

    }
}
