using Feign;
using Feign.Discovery;
using Feign.Formatting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class FeignBuilderExtensions
    {
        public static IFeignBuilder AddSteeltoeServiceDiscovery(this IFeignBuilder feignBuilder)
        {
            return feignBuilder.AddServiceDiscovery<SteeltoeServiceDiscovery>();
        }
    }
}
