using Feign.Polly;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Feign
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PollyFeignBuilderExtensions
    {
        /// <summary>
        /// 添加Polly支持
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static T AddPolly<T>(this T feignBuilder, FeignPollyOptions? options) where T : IFeignBuilder
        {
            feignBuilder.Options.Pipeline.AddPolly(options);
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
            feignBuilder.Options.Pipeline.AddPolly(setup);
            return feignBuilder;
        }

        public static IFeignClientPipeline<TService> AddPolly<TService>(this IFeignClientPipeline<TService> pipeline, FeignPollyOptions? options)
        {
            options ??= new FeignPollyOptions();
            var middleware = new PollyInitializingMiddleware<TService>(options);
            pipeline.UseMiddleware(middleware);
            return pipeline;
        }

        public static IFeignClientPipeline<TService> AddPolly<TService>(this IFeignClientPipeline<TService> pipeline, Action<FeignPollyOptions> setup)
        {
            var options = new FeignPollyOptions();
            setup?.Invoke(options);
            return pipeline.AddPolly(options);
        }

    }
}
