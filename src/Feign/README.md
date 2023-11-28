# Usage

1. Install the NuGet package

    `PM> Install-Package Feign`

2. Define services : 

```csharp
    [FeignClient("test-service", UriKind = UriKind.RelativeOrAbsolute)]
    [Headers("Cache-Control:max-age=0")]
    [RequestMapping("/api/test")]
    public interface ITestService
    {
        string Name { get; set; }
        /// <summary>
        /// async get
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Headers("Cache-Control:max-age=0", "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36")]
        [RequestMapping("/{id}", Method = "GET")]
        //[GetMapping("/{id}")]
        [MethodId("GetAsync")]
        Task<string> GetAsync([PathVariable("id")] int id, [RequestQuery] string name);
        /// <summary>
        /// get Stream
        /// </summary>
        /// <returns></returns>
        [GetMapping("stream")]
        Task<Stream> GetStreamAsync();
        /// <summary>
        /// get buffer
        /// </summary>
        /// <param name="authorization">sample: scheme parameter</param>
        /// <param name="header">sample: name:value</param>
        /// <returns></returns>
        [GetMapping("stream")]
        Task<byte[]> GetBufferAsync([RequestAuthorization] string authorization, [RequestHeader] string header);
        /// <summary>
        /// get HttpResponseMessage
        /// </summary>
        /// <returns></returns>
        [GetMapping("stream")]
        Task<HttpResponseMessage> GetHttpResponseMessageAsync();

        /// <summary>
        /// post a json request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [RequestMapping("{id}", Method = "POST")]
        //[PostMapping("/{id}")]
        string PostJson([PathVariable] int id, [RequestBody] TestServiceParam param);

        /// <summary>
        /// post a form request
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [RequestMapping("{id}", Method = "POST")]
        //[PostMapping("/{id}")]
        string PostForm(int id, [RequestForm] TestServiceParam param);

        /// <summary>
        /// upload 2 files
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        [PostMapping("/api/test/upload")]
        string UploadFile(IHttpRequestFile file1, IHttpRequestFile file2);

        /// <summary>
        /// upload multiple files
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        [PostMapping("/api/test/upload")]
        string UploadFile(IHttpRequestFileForm files);

    }
```

3. Install the NuGet package on .net core application

    `PM> Install-Package Feign.DependencyInjection`

    ```c#
        var feignBuilder = builder.Services.AddFeignClients(options =>
        {
            //options.DiscoverServiceCacheTime = TimeSpan.FromSeconds(10);
        }).AddFeignClients(typeof(ITestService).Assembly, FeignClientLifetime.Singleton)
        .ConfigureJsonSettings(options =>
        {
            options.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        });
    ```

4. Use service discovery, here is Steeltoe as an example

    `PM> Install-Package Feign.Steeltoe`

    ```c#
        feignBuilder.AddSteeltoe();
    ```

5. Using services, here we take asp.net core as an example

```csharp

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(int id, [FromServices] ITestService testService)
        {
            await testService.GetAsync(id);
	        testService.PostJson(id, new TestServiceParam());
            testService.UploadFile(
                new FilePathHttpRequestFile(@"E:\asdasdasd.txt"),
                new FilePathHttpRequestFile(@"E:\asdasdasd.txt")
            );
            return "ok";
        }
    }

```


