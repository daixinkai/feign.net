using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Feign
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class AngleSharpExtensions
    {
        public static void AddAngleSharp<TService>(this IFeignClientPipeline<TService> feignClientPipeline)
        {
            feignClientPipeline.ReceivingResponse += async (sender, e) =>
            {
                if (e.ResultType == typeof(IHtmlDocument))
                {
                    var stream = await e.ResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                    e.Result = new HtmlParser().ParseDocument(stream);
                }
            };
        }

        public static TFeignBuilder AddAngleSharp<TFeignBuilder>(this TFeignBuilder feignBuilder) where TFeignBuilder : IFeignBuilder
        {
            feignBuilder.Options.FeignClientPipeline.AddAngleSharp();
            return feignBuilder;
        }

    }
}
