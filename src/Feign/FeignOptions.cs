using Feign.Discovery.LoadBalancing;
using Feign.Formatting;
using Feign.Pipeline.Internal;
using Feign.Reflection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public class FeignOptions : IFeignOptions
    {
        public FeignOptions()
        {
            Assemblies = new List<Assembly>();
            Converters = new ConverterCollection();
            //Converters.AddConverter(new ClassToStringConverter<string>());
            //Converters.AddConverter(new StructToStringConverter<int>());
            //Converters.AddConverter(new StructToStringConverter<long>());
            Converters.AddConverter(new ObjectStringConverter());
            MediaTypeFormatters = new MediaTypeFormatterCollection();
            MediaTypeFormatters.AddFormatter(new JsonMediaTypeFormatter(this));
            MediaTypeFormatters.AddFormatter(new JsonMediaTypeFormatter(Constants.MediaTypes.TEXT_JSON, this));
            MediaTypeFormatters.AddFormatter(new XmlMediaTypeFormatter());
            MediaTypeFormatters.AddFormatter(new XmlMediaTypeFormatter(Constants.MediaTypes.TEXT_XML));
            MediaTypeFormatters.AddFormatter(new FormUrlEncodedMediaTypeFormatter());
            MediaTypeFormatters.AddFormatter(new MultipartFormDataMediaTypeFormatter());
            FeignClientPipeline = new GlobalFeignClientPipeline();
            Lifetime = FeignClientLifetime.Singleton;
            Types = new List<FeignClientTypeInfo>();
            DiscoverServiceCacheTime = TimeSpan.FromMinutes(10);
            PropertyNamingPolicy = NamingPolicy.CamelCase;
            JsonProvider = new JsonProviderType();
            LoadBalancingPolicy = LoadBalancingPolicy.Random;
        }
        public IList<Assembly> Assemblies { get; }
        public ConverterCollection Converters { get; }
        public MediaTypeFormatterCollection MediaTypeFormatters { get; }
        public IGlobalFeignClientPipeline FeignClientPipeline { get; }
        public FeignClientLifetime Lifetime { get; set; }
        public bool IncludeMethodMetadata { get; set; }

        public NamingPolicy PropertyNamingPolicy { get; set; }

        public IJsonProvider JsonProvider { get; set; }

        public DecompressionMethods? AutomaticDecompression { get; set; }

        public IList<FeignClientTypeInfo> Types { get; }
        public TimeSpan? DiscoverServiceCacheTime { get; set; }

        /// <inheritdoc/>
        public bool? UseCookies { get; set; }

        /// <inheritdoc/>
        public bool UseUrlEncode { get; set; }

        /// <inheritdoc/>
        public LoadBalancingPolicy LoadBalancingPolicy { get; set; }

    }
}
