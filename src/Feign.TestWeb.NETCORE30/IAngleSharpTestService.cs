using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feign.TestWeb.NETCORE30
{
    [FeignClient("angle-sharp-test-service", Url = "")]
    public interface IAngleSharpTestService
    {
        [RequestMapping("{url}")]
        Task<IHtmlDocument> GetHtmlDocumentAsync(string url);
        [RequestMapping("{url}")]
        Task<string> GetHtmlAsync(string url);

        [RequestMapping("{url}")]
        IHtmlDocument GetHtmlDocument(string url);
        [RequestMapping("{url}")]
        string GetHtml(string url);

    }
}
