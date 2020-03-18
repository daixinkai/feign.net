using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feign.TestWeb.NETCORE30.Controllers
{
    [Route("[controller]")]
    [Controller]
    public class AngleSharpController : Controller
    {
        public AngleSharpController(IAngleSharpTestService angleSharpTestService)
        {
            _angleSharpTestService = angleSharpTestService;
        }

        IAngleSharpTestService _angleSharpTestService;

        [HttpGet("index")]
        public async Task<IActionResult> Index()
        {
            string url = "https://www.jd.com";
            string html = await _angleSharpTestService.GetHtmlAsync(url);
            var document = await _angleSharpTestService.GetHtmlDocumentAsync(url);
            return Content(document.Head.GetElementsByTagName("title").FirstOrDefault()?.TextContent);
        }


        //[HttpGet("index")]
        //public IActionResult Index()
        //{
        //    string url = "https://www.jd.com";
        //    string html = _angleSharpTestService.GetHtml(url);
        //    var document = _angleSharpTestService.GetHtmlDocument(url);
        //    return Content(document.Head.GetElementsByTagName("title").FirstOrDefault()?.TextContent);
        //}

    }
}
