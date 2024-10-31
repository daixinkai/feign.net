using Feign.Tests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Feign.TestWeb.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet("{id}")]
        [HttpGet("")]
        public Task<IQueryResult<TestServiceParam>> GetQueryResultValueAsync(string? id, [FromQuery] TestServiceParam param)
        {
            param.Name = param.Name + "_" + id + "_" + Guid.NewGuid().ToString();
            return Task.FromResult<IQueryResult<TestServiceParam>>(new QueryResult<TestServiceParam>()
            {
                Data = param,
                StatusCode = System.Net.HttpStatusCode.OK
            });
        }

        [HttpGet("stopwatch")]
        public async Task<ActionResult<object>> Index([FromServices] ITestServiceClient testServiceClient)
        {

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 1000; i++)
            {
                var t = await testServiceClient.GetQueryResultValueAsync("1", new int[] { 1, 2, 3 }, null);
            }

            stopwatch.Stop();

            return new JsonResult(stopwatch);

        }


    }
}
