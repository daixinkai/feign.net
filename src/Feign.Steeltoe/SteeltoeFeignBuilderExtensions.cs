using Feign;
using Feign.Discovery;
using Feign.Formatting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Feign
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SteeltoeFeignBuilderExtensions
    {
        public static T AddSteeltoe<T>(this T feignBuilder) where T : IFeignBuilder
        {
            feignBuilder.AddServiceDiscovery<SteeltoeServiceDiscovery>();
            return feignBuilder;
        }

    }
}
