using Feign;
using Feign.Cache;
using Feign.Discovery;
using Feign.Formatting;
using Feign.Logging;
using Feign.Polly;
using Polly;
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
        /// <summary>
        /// 添加Polly支持
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T AddPolly<T>(this T feignBuilder, FeignPollyOptions options) where T : IFeignBuilder
        {
            if (options == null)
            {
                options = new FeignPollyOptions();
            }
            feignBuilder.Options.FeignClientPipeline.Initializing += (sender, e) =>
            {
                PollyDelegatingHandler pollyDelegatingHandler = new PollyDelegatingHandler(e.HttpClient.Handler.InnerHandler);
                options.SetupAllPolly(e.FeignClient, pollyDelegatingHandler);
                e.HttpClient.Handler.InnerHandler = pollyDelegatingHandler;
            };
            return feignBuilder;
        }
        /// <summary>
        /// 添加Polly支持
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        public static T AddPolly<T>(this T feignBuilder, Action<FeignPollyOptions> setup) where T : IFeignBuilder
        {
            FeignPollyOptions options = new FeignPollyOptions();
            setup?.Invoke(options);
            return feignBuilder.AddPolly(options);
        }

    }
}
