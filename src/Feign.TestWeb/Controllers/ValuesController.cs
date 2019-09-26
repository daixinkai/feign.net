using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Feign.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Feign.TestWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get([FromServices] IDistributedCache distributedCache)
        {
            return new string[] { "value1", "value2", distributedCache?.GetHashCode().ToString() };
        }

        [HttpPost("uploadFile")]
        [HttpPost("/organizations/values/uploadFile")]
        public ActionResult<object> UploadFile(IFormFile file, [FromForm]TestServiceParam param)
        {
            return file?.Length;
        }

        [HttpPost("FormTest")]
        [HttpPost("/organizations/values/formTest")]
        public ActionResult<object> FormTest([FromForm]TestServiceParam param)
        {
            return param?.Name;
        }

        [HttpPost("uploadFiles")]
        public ActionResult<object> UploadFiles(List<IFormFile> files)
        {
            return files?.Count + " : " + Request.Form.Files.Count;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(int id, [FromServices] ITestService testService/*, [FromServices] ITestService1 testService1*/)
        {
            //await testService.PostValueAsync();
            await testService.GetById(id);
            testService.GetValueVoid(id, "", new TestServiceParam
            {

            });

            //string html = await testService1.GetHtml();
            //return html;

            //var rrr = typeof(Func<Task>).GetConstructors(System.Reflection.BindingFlags.Default);

            //IServiceCollection serviceCollection = HttpContext.RequestServices.GetService(typeof(IServiceCollection)) as IServiceCollection;

            //testService.GetValueVoidAsync(id, null, new TestServiceParam
            //{
            //    Name = "asasdsad"
            //});
            //return await testService.GetValueAsync(id, "asdasd");
            //await testService.PostValueForm2Async(id, "", new TestServiceParam
            //{
            //    Name = "testName"
            //}, new TestServiceParam
            //{
            //    Name = "name"
            //});
            //testService.GetValueVoid(id, new TestServiceParam
            //{
            //    Name = "testName"
            //}, new TestServiceParam
            //{
            //    Name = "name"
            //});
            //await testService.PostValueAsync();
            //await testService.PostValueAsync(id, "", new TestServiceParam());
            //return testService.GetQueryResultValue(id.ToString(), new TestServiceParam
            //{
            //    Name = "asasdsad"
            //});
            //return await testService.GetQueryResultValueAsync(id.ToString(), new TestServiceParam
            //{
            //    Name = "asasdsad"
            //});
            testService.GetValueVoidAsync(id, "", null);
            return "ok";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
