using Feign.Formatting;
using Feign.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public interface IFeignOptions
    {
        /// <summary>
        /// 获取程序集集合
        /// </summary>
        IList<Assembly> Assemblies { get; }
        /// <summary>
        /// 获取获取转换器集合
        /// </summary>
        ConverterCollection Converters { get; }
        /// <summary>
        /// 获取媒体处理器集合
        /// </summary>
        MediaTypeFormatterCollection MediaTypeFormatters { get; }
        /// <summary>
        /// 获取全局工作管道
        /// </summary>
        IGlobalFeignClientPipeline FeignClientPipeline { get; }
        /// <summary>
        /// 获取或设置服务的生命周期 默认值 <see cref="FeignClientLifetime.Singleton"/>
        /// </summary>
        FeignClientLifetime Lifetime { get; set; }
        /// <summary>
        /// 获取或设置一个值,指示是否需要包含声明服务的方法元数据 默认 : false
        /// </summary>
        bool IncludeMethodMetadata { get; set; }

        /// <summary>
        /// 获取或设置属性命名规则
        /// </summary>
        NamingPolicy PropertyNamingPolicy { get; set; }

        /// <summary>
        /// 获取或设置JsonProvider
        /// </summary>
        IJsonProvider JsonProvider { get; set; }

        /// <summary>
        /// 获取或设置处理程序用于自动解压缩内容响应的解压缩方法类型。默认 : null
        /// </summary>
        DecompressionMethods? AutomaticDecompression { get; set; }

        /// <summary>
        /// 获取生成的类型
        /// </summary>
        IList<FeignClientTypeInfo> Types { get; }
        /// <summary>
        /// 缓存服务时间 default : 10min
        /// 设置为null则不使用缓存
        /// </summary>
        TimeSpan? DiscoverServiceCacheTime { get; set; }

        /// <summary>
        /// 默认HttpClientHandler的UseCookies值
        /// </summary>
        bool? UseCookies { get; set; }
        /// <summary>
        /// 是否启用编码Url (如 : RequestQuery,PathVariable)
        /// </summary>
        bool UseUrlEncode { get; set; }
    }
}
