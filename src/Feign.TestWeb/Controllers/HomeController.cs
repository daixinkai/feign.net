using Feign.Tests;
using Microsoft.AspNetCore.Mvc;

namespace Feign.TestWeb.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("/")]
        public async Task<ActionResult<object>> Index(
            string? id,
            [FromServices] ITestService testService,
            [FromServices] ITestControllerService testControllerService,
            [FromServices] ITestServiceClient testServiceClient
            )
        {
            if (id != null)
            {

                return new QueryResult<object>
                {
                    Data = new TestServiceParam
                    {
                        Name = "return_" + id
                    }
                };
            }
            var noBaseUrlTestService = HttpContext.RequestServices.GetRequiredService<INoBaseUrlTestService>();

            var responseMessage = await testControllerService.GetHttpResponseMessage();

            string? ss = responseMessage.RequestMessage?.RequestUri?.ToString();

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

            //var t = await testControllerService.GetQueryResultValueAsync("1", new int[] { 1, 2, 3 });       
            var t = await testServiceClient.GetQueryResultValueAsync("1", new int[] { 1, 2, 3 });
            return new JsonResult(t);


            //var r = await testService.Get();


            ////var r = testService.GetById(1).Result;
            ////var r = await testService.GetQueryResultValueAsync("", new TestServiceParam { });
            ////await testService.PostValueAsync();
            //HttpContext.Response.ContentType = "text/html;charset=utf-8";
            //await HttpContext.Response.WriteAsync(r);
            //return new OkResult();
        }

        [HttpGet("/html")]
        public IActionResult Html()
        {
            return Content("<html><head><title>html</title></head></html>", "text/html");
        }

    }
}
