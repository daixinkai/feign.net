using Feign.Fallback;
using Feign.Proxy;
using Feign.Reflection;
using Feign.Standalone.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Standalone.Reflection
{
    internal class StandaloneFeignClientHttpProxyTypeBuilder : FeignClientHttpProxyTypeBuilder
    {
        protected override Type GetParentType(Type parentType)
        {
            if (typeof(FallbackFactoryFeignClientHttpProxy<,>) == parentType.GetGenericTypeDefinition())
            {
                return typeof(StandaloneFallbackFactoryFeignClientHttpProxy<,>).MakeGenericType(parentType.GetGenericArguments());
            }
            if (typeof(FallbackFeignClientHttpProxy<,>) == parentType.GetGenericTypeDefinition())
            {
                return typeof(StandaloneFallbackFeignClientHttpProxy<,>).MakeGenericType(parentType.GetGenericArguments());
            }
            return typeof(StandaloneFeignClientHttpProxy<>).MakeGenericType(parentType.GetGenericArguments());
        }
    }
}
