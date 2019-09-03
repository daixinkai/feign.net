using Feign.Request;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Feign.Tests
{
    [CustomFeignClient("yun-platform-service-provider"
        , Fallback = typeof(TestServiceFallback)
         //, FallbackFactory = typeof(TestServiceFallbackFactory)
         //, Url = "http://localhost:8802/"
         //, Url = "http://10.1.5.90:8802/"
         //, Url = "http://localhost:62088/"
         //, Url = ""
        )]
    //[RequestMapping("/organizations")]
    public interface ITestService
    {



        //string Name { get; }

        //string Get();

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

        [RequestMapping("/{id}", Method = "GET")]
        Task<QueryResult<JObject>> GetQueryResultValueAsync([PathVariable("id")]string id, [RequestQuery] TestServiceParam param);

        [RequestMapping("/{id}", Method = "GET")]
        QueryResult<JObject> GetQueryResultValue([PathVariable("id")]string id, [RequestQuery] TestServiceParam param);

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
        Task PostValueAsync([PathVariable]int id, [RequestParam] string test, [RequestBody] TestServiceParam param);

        [RequestMapping("/{id}", Method = "POST")]
        Task PostValueFormAsync([PathVariable]int id, [RequestParam] string test, [RequestForm] TestServiceParam param);

        [RequestMapping("/{id}", Method = "POST")]
        Task PostValueForm2Async([PathVariable]int id, [RequestParam] string test, [RequestForm] TestServiceParam param1, [RequestForm] TestServiceParam param2);

        [RequestMapping("/{id}")]
        void GetValueVoid([PathVariable]int id, [RequestParam] TestServiceParam queryParam, [RequestQuery] TestServiceParam param);

        //[GetMapping("/{id}")]
        //Task<JObject> GetValueAsync([PathVariable]int id, [RequestParam] string test, [RequestQuery] TestServiceParam param);

    }



}
