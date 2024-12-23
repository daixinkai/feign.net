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
        protected override Type GetParentType(Type serviceType, FeignClientAttribute feignClientAttribute)
        {
            if (feignClientAttribute.Fallback != null)
            {
                return typeof(StandaloneFallbackFeignClientHttpProxy<,>).MakeGenericType(serviceType, feignClientAttribute.Fallback);
            }
            if (feignClientAttribute.FallbackFactory != null)
            {
                return typeof(StandaloneFallbackFactoryFeignClientHttpProxy<,>).MakeGenericType(serviceType, feignClientAttribute.FallbackFactory);
            }
            return typeof(StandaloneFeignClientHttpProxy<>).MakeGenericType(serviceType);
        }
    }
}
