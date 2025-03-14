﻿using Feign.Tests;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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

            testControllerService.GetQueryResultValue(null, new TestServiceParam(), "api/test", "Key:Value", 1);

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

            //responseMessage = await testControllerService.GetDefaultHttpResponseMessage();

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

            var tt = testServiceClient.GetQueryResultValueAsync("1", new int[] { 1, 2, 3 }, new Dictionary<string, object>
            {
                ["mapTest"] = 1
            }, new TestServiceParam
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Time = TimeOnly.FromDateTime(DateTime.Now),
                Age = 1,
                Name = "root",
                Ids = ["1", "2"],
                DateTimeRange = new DateTimeOffsetRange
                {
                    StartTime = DateTimeOffset.Now,
                    EndTime = DateTimeOffset.Now
                },
                Properties = new Dictionary<string, string>
                {
                    { "key1","value1"},
                    { "key2","value2"}
                },
                SubParam = new TestServiceParam
                {
                    Age = 2,
                    Name = "sub1",
                    SubParam = new TestServiceParam
                    {
                        Age = 3,
                        Name = "sub2",
                    },
                    Properties = new Dictionary<string, string>
                    {
                        { "key1","value1"},
                        { "key2","value2"}
                    },
                }
            });
            var t = await tt;
            var ttt = testServiceClient.GetQueryResultValue("1", new int[] { 1, 2, 3 });
            //return new JsonResult(t);

            var tttt = await testService.PostOfTAsync<HttpResponseMessage>(1);

            return Ok(t);


            //var r = await testService.Get();


            ////var r = testService.GetById(1).Result;
            ////var r = await testService.GetQueryResultValueAsync("", new TestServiceParam { });
            ////await testService.PostValueAsync();
            //HttpContext.Response.ContentType = "text/html;charset=utf-8";
            //await HttpContext.Response.WriteAsync(r);
            //return new OkResult();
        }

        [HttpGet("/test")]
        public async Task<ActionResult<object>> Test(string? id, [FromServices] ITestControllerService testControllerService)
        {
            //var s = testControllerService.GetQueryResultValue(null, null, "sessionId:12345", 1);
            var ss = await testControllerService.GetHttpResponseMessage(new TestServiceParam
            {
            });
            return Ok();
        }

        [HttpGet("/html")]
        public IActionResult Html()
        {
            return Content("<html><head><title>html</title></head></html>", "text/html");
        }

    }
}
