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
            AddDefaultConverters();
            MediaTypeFormatters = new MediaTypeFormatterCollection();
            AddDefaultFormatters();
            FeignClientPipeline = new GlobalFeignClientPipeline();
            Lifetime = FeignClientLifetime.Singleton;
            Types = new List<FeignClientTypeInfo>();
            DiscoverServiceCacheTime = TimeSpan.FromMinutes(10);
            PropertyNamingPolicy = NamingPolicy.CamelCase;
            JsonProvider = new JsonProviderType();
            LoadBalancingPolicy = LoadBalancingPolicy.Random;
        }

        private void AddDefaultConverters()
        {
            Converters.AddConverter(new StringToStringConverter(), false);
            Converters.AddConverter(new BooleanToStringConverter(), false);
            Converters.AddConverter(new StructToStringConverter<byte>(), false);
            Converters.AddConverter(new StructToStringConverter<char>(), false);
            Converters.AddConverter(new StructToStringConverter<decimal>(), false);
            Converters.AddConverter(new StructToStringConverter<double>(), false);
            Converters.AddConverter(new StructToStringConverter<float>(), false);
            Converters.AddConverter(new StructToStringConverter<int>(), false);
            Converters.AddConverter(new StructToStringConverter<long>(), false);
            Converters.AddConverter(new StructToStringConverter<sbyte>(), false);
            Converters.AddConverter(new StructToStringConverter<short>(), false);
            Converters.AddConverter(new StructToStringConverter<uint>(), false);
            Converters.AddConverter(new StructToStringConverter<ulong>(), false);
            Converters.AddConverter(new StructToStringConverter<ushort>(), false);
            Converters.AddConverter(new DateTimeToStringConverter(), false);
            Converters.AddConverter(new DateTimeOffsetToStringConverter(), false);
            Converters.AddConverter(new ObjectStringConverter());
        }

        private void AddDefaultFormatters()
        {
            MediaTypeFormatters.AddFormatter(new JsonMediaTypeFormatter(this));
            MediaTypeFormatters.AddFormatter(new JsonMediaTypeFormatter(Constants.MediaTypes.TEXT_JSON, this));
            MediaTypeFormatters.AddFormatter(new XmlMediaTypeFormatter());
            MediaTypeFormatters.AddFormatter(new XmlMediaTypeFormatter(Constants.MediaTypes.TEXT_XML));
            MediaTypeFormatters.AddFormatter(new FormUrlEncodedMediaTypeFormatter());
            MediaTypeFormatters.AddFormatter(new MultipartFormDataMediaTypeFormatter());
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
