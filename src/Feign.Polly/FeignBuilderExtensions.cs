using Feign;
using Feign.Cache;
using Feign.Discovery;
using Feign.Formatting;
using Feign.Logging;
using Feign.Polly;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;

namespace Feign
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class FeignBuilderExtensions
    {

        [Obsolete]
        public static T AddPolly<T>(this T feignBuilder, FeignPollyOptions options) where T : IFeignBuilder
        {

            if (options == null)
            {
                options = new FeignPollyOptions();
            }

            feignBuilder.Options.FeignClientPipeline.Initializing += (sender, e) =>
            {                
                // var feignClient = e.FeignClient as Feign.Proxy.FeignClientHttpProxy<object>;
                // HttpClient httpClient = e.HttpClient;
                //  ServiceDiscoveryHttpClientPollyHandler<object> serviceDiscoveryHttpClientPollyHandler = new ServiceDiscoveryHttpClientPollyHandler<object>(e.FeignClient);
            };
            return feignBuilder;
        }
    }
}
