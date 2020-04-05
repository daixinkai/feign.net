using Feign.Request;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Feign.Tests
{
    //[CustomFeignClient("asdasdasd")]
    //[RequestMapping("/organizations")]
    [Headers("Cache-Control:max-age=0000", "Accept-Encoding: gzip, deflate, br", "Accept-Language: zh-CN,zh;q=0.9,en;q=0.8")]
    public interface ITestService : ITestParentService<string>
    {

        [Headers("Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9", "Cache-Control:max-age=0")]
        [RequestMapping(CompletionOption = HttpCompletionOption.ResponseHeadersRead)]
        Task<string> Get();
        [RequestMapping("/{id}/asdasdsad", Method = "POST")]
        [MethodId("PostValueAsync")]
        Task PostValueAsync();

        [RequestMapping("/Values/uploadFile", Method = "POST")]
        Task<string> UploadFileAsync(IHttpRequestFile file, [RequestForm] TestServiceParam param);

        [RequestMapping("/Values/uploadFile", Method = "POST")]
        Task<string> UploadFileAsync(IHttpRequestFile file, [RequestForm] string name);

        [RequestMapping("/Values/uploadFile", Method = "POST")]
        Task<string> UploadFileAsync(TestServiceUploadFileParam param);

        [RequestMapping("/Values/formTest", Method = "POST")]
        Task<string> FormTestAsync([RequestForm] TestServiceParam param);

        [RequestMapping("/Values/uploadFiles", Method = "POST")]
        Task<string> UploadFilesAsync(IHttpRequestFile file1, IHttpRequestFile file2, IHttpRequestFile file3);

        //[ResultType(typeof(QueryResult<>))]
        //[Headers("Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9", "Cache-Control:max-age=0")]
        //[RequestMapping("/{id}", Method = "GET", Accept = "text/html")]
        //Task<IQueryResult<JObject>> GetQueryResultValueAsync([PathVariable("id")]string id, [RequestQuery] TestServiceParam param);

        //[ResultType(typeof(QueryResult<>))]
        //[RequestMapping("/{id}", Method = "GET")]
        //IQueryResult<JObject> GetQueryResultValue([PathVariable("id")]string id, [RequestQuery] TestServiceParam param);

        //[RequestMapping("/{id}", Method = "GET")]
        //Task<JObject> GetValueAsync([PathVariable("id")]string id);
        //[RequestMapping("/{id}", Method = "GET")]
        //Task<JObject> GetValueAsync([PathVariable]int id, [RequestParam] string test);
        //[GetMapping("/{id}")]
        //Task<JObject> GetValueAsync([PathVariable]int id, [RequestQuery] TestServiceParam param);
        [RequestMapping("/{id}")]
        //[RequestMapping("http://www.baidu.com")]
        void GetValueVoid([PathVariable]int id, [RequestParam] string test, [RequestParam] TestServiceParam param);

        [RequestMapping("/{id}")]
        Task GetValueVoidAsync([PathVariable]int id, [RequestParam] string test, [RequestQuery] TestServiceParam param);

        [RequestMapping("/{id}", Method = "POST")]
        Task PostValueAsync([PathVariable]int id, [RequestQuery] string test, [RequestBody] TestServiceParam param);

        [RequestMapping("/{id}", Method = "POST")]
        Task PostValueFormAsync([PathVariable]int id, [RequestParam] string test, [RequestForm] TestServiceParam param);

        [RequestMapping("/{id}", Method = "POST")]
        Task PostValueForm2Async([PathVariable]int id, [RequestParam] string test, [RequestForm] TestServiceParam param1, [RequestForm] TestServiceParam param2);

        [RequestMapping("/{id}")]
        void GetValueVoid([PathVariable]int id, [RequestParam] TestServiceParam queryParam, [RequestQuery] TestServiceParam param);

        //[GetMapping("/{id}")]
        //Task<JObject> GetValueAsync([PathVariable]int id, [RequestParam] string test, [RequestQuery] TestServiceParam param);

        [DeleteMapping]
        Task<string> DeleteAsync([RequestBody]int[] ids);

    }



}
