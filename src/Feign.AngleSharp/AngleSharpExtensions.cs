using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AngleSharpExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="feignClientPipeline"></param>
        public static void AddAngleSharp<TService>(this IFeignClientPipeline<TService> feignClientPipeline)
        {
            feignClientPipeline.UseReceivingResponse(async context =>
            {
                if (context.ResultType == typeof(IHtmlDocument))
                {
                    var stream = await context.ResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    context.Result = new HtmlParser().ParseDocument(stream);
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TFeignBuilder"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <returns></returns>
        public static TFeignBuilder AddAngleSharp<TFeignBuilder>(this TFeignBuilder feignBuilder) where TFeignBuilder : IFeignBuilder
        {
            feignBuilder.Options.FeignClientPipeline.AddAngleSharp();
            return feignBuilder;
        }

    }
}
