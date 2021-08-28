using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feign.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Feign.TestWeb.NETCORE30.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnGet([FromServices] ITestService testService, [FromServices] ITestControllerService testControllerService)
        {

            var noBaseUrlTestService = HttpContext.RequestServices.GetRequiredService<INoBaseUrlTestService>();

            var baiduHtml = await noBaseUrlTestService.GetBaidu();

            //return Content(baiduHtml);

            string name = testService.Name;
            string serviceId = testService.ServiceId;
            Type serviceType = testService.ServiceType;

            //testService.GetValueVoid(1, "", new TestServiceParam
            //{
            //    State = 1
            //});

            //var t = await testControllerService.GetQueryResultValueAsync("1", new TestServiceParam
            //{
            //    Age = 11,
            //    Name = "OnGet"
            //});

            //var t = await testControllerService.GetQueryResultValueAsync("1", 1);

            var t = await testControllerService.GetQueryResultValueAsync("1", new int[] { 1, 2, 3 });
            return new JsonResult(t);


            //var r = await testService.Get();


            ////var r = testService.GetById(1).Result;
            ////var r = await testService.GetQueryResultValueAsync("", new TestServiceParam { });
            ////await testService.PostValueAsync();
            //HttpContext.Response.ContentType = "text/html;charset=utf-8";
            //await HttpContext.Response.WriteAsync(r);
            //return new OkResult();
        }
    }
}
