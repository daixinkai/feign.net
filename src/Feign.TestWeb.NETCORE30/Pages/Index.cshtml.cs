using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feign.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public async Task<IActionResult> OnGet([FromServices]ITestService testService, [FromServices] ITestControllerService testControllerService)
        {

            var t = await testControllerService.GetQueryResultValueAsync("1", new TestServiceParam
            {
                Age = 11,
                Name = "OnGet"
            });

            string name = testService.Name;
            string serviceId = testService.ServiceId;
            Type serviceType = testService.ServiceType;
            //var r = await testService.Get();


            ////var r = testService.GetById(1).Result;
            ////var r = await testService.GetQueryResultValueAsync("", new TestServiceParam { });
            ////await testService.PostValueAsync();
            //HttpContext.Response.ContentType = "text/html;charset=utf-8";
            //await HttpContext.Response.WriteAsync(r);
            return new JsonResult(t);
        }
    }
}
