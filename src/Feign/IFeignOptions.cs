using Feign.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public interface IFeignOptions
    {
        IList<Assembly> Assemblies { get; }
        ConverterCollection Converters { get; }
        MediaTypeFormatterCollection MediaTypeFormatters { get; }
        IGlobalFeignClientPipeline FeignClientPipeline { get; }
        FeignClientLifetime Lifetime { get; set; }
        bool IncludeMethodMetadata { get; set; }
    }
}
