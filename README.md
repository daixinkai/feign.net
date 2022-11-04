# feign.net

*spring cloud feign for .net*

## feign.net是一个spring cloud feign组件的c#移植版

### 定义服务 : 

```csharp
    [FeignClient("test-service", UriKind = UriKind.RelativeOrAbsolute)]
    [Headers("Cache-Control:max-age=0")]
    [RequestMapping("/api/test")]
    public interface ITestService
    {
        string Name { get; set; }
        /// <summary>
        /// async get一个请求
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Headers("Cache-Control:max-age=0", "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36")]
        [RequestMapping("/{id}", Method = "GET")]
        //[GetMapping("/{id}")]
        [MethodId("GetAsync")]
        Task<string> GetAsync([PathVariable("id")] int id, [RequestQuery] string name);
        /// <summary>
        /// 获取流
        /// </summary>
        /// <returns></returns>
        [GetMapping("stream")]
        Task<Stream> GetStreamAsync();
        /// <summary>
        /// 获取buffer
        /// </summary>
        /// <param name="authorization">sample: scheme parameter</param>
        /// <param name="header">sample: name:value</param>
        /// <returns></returns>
        [GetMapping("stream")]
        Task<byte[]> GetBufferAsync([RequestAuthorization] string authorization, [RequestHeader] string header);
        /// <summary>
        /// 获取HttpResponseMessage
        /// </summary>
        /// <returns></returns>
        [GetMapping("stream")]
        Task<HttpResponseMessage> GetHttpResponseMessageAsync();

        /// <summary>
        /// 以json的方式post一个请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [RequestMapping("{id}", Method = "POST")]
        //[PostMapping("/{id}")]
        string PostJson([PathVariable] int id, [RequestBody] TestServiceParam param);

        /// <summary>
        /// 以form表单的方式post一个请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [RequestMapping("{id}", Method = "POST")]
        //[PostMapping("/{id}")]
        string PostForm(int id, [RequestForm] TestServiceParam param);

        /// <summary>
        /// 上传2个文件
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        [PostMapping("/api/test/upload")]
        string UploadFile(IHttpRequestFile file1, IHttpRequestFile file2);

        /// <summary>
        /// 上传多个文件
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        [PostMapping("/api/test/upload")]
        string UploadFile(IHttpRequestFileForm files);

    }
```

## Emit代理类生成代码
<details>
<summary>TestService_Proxy</summary>

```csharp
    [StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
    public class ITestService_Proxy_5F7A08343A584555856613BF22ACB8CA : FeignClientHttpProxy<ITestService>, ITestService
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public ITestService_Proxy_5F7A08343A584555856613BF22ACB8CA(IFeignOptions feignOptions, IServiceDiscovery serviceDiscovery, ICacheProvider cacheProvider, ILoggerFactory loggerFactory) : base(feignOptions, serviceDiscovery, cacheProvider, loggerFactory)
        {
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000002 RID: 2 RVA: 0x0000206C File Offset: 0x0000026C
        public override string ServiceId
        {
            get
            {
                return "test-service";
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000003 RID: 3 RVA: 0x00002080 File Offset: 0x00000280
        public override string BaseUri
        {
            get
            {
                return "/api/test";
            }
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000004 RID: 4 RVA: 0x00002094 File Offset: 0x00000294
        protected override UriKind UriKind
        {
            get
            {
                return UriKind.RelativeOrAbsolute;
            }
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x06000006 RID: 6 RVA: 0x000020C4 File Offset: 0x000002C4
        protected override string[] DefaultHeaders
        {
            get
            {
                return ITestService_Proxy_5F7A08343A584555856613BF22ACB8CA.s_headers;
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000020D8 File Offset: 0x000002D8
        [RequestMapping("/{id}", Method = "GET")]
        [MethodId("GetAsync")]
        [Headers(new string[]
        {
            "Cache-Control:max-age=0",
            "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36"
        })]
        public Task<string> GetAsync([PathVariable("id")] int id, [RequestQuery] string name)
        {
            string uri = "/{id}";
            uri = base.ReplaceToStringPathVariable<int>(uri, "id", id);
            uri = base.ReplaceStringRequestQuery(uri, "name", name);
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "GetAsync";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.GetAsync(int, string));
            }
            FeignClientHttpRequest request = new FeignClientHttpRequest(this.BaseUrl, "/{id}", uri, "GET", null)
            {
                Method = feignClientMethodInfo,
                Headers = new string[]
                {
                    "Cache-Control:max-age=0",
                    "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36"
                },
                IsSpecialResult = true
            };
            return base.SendAsync<string>(request);
        }

        // Token: 0x06000008 RID: 8 RVA: 0x0000218C File Offset: 0x0000038C
        [GetMapping("stream")]
        public Task<Stream> GetStreamAsync()
        {
            string uri = "stream";
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "GetStreamAsync";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.GetStreamAsync());
            }
            FeignClientHttpRequest request = new FeignClientHttpRequest(this.BaseUrl, "stream", uri, "GET", null)
            {
                Method = feignClientMethodInfo,
                IsSpecialResult = true
            };
            return base.SendAsync<Stream>(request);
        }

        // Token: 0x06000009 RID: 9 RVA: 0x00002208 File Offset: 0x00000408
        [GetMapping("stream")]
        public Task<byte[]> GetBufferAsync([RequestAuthorization] string authorization, [RequestHeader] string header)
        {
            string uri = "stream";
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "GetBufferAsync";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.GetBufferAsync(string, string));
            }
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "stream", uri, "GET", null)
            {
                Method = feignClientMethodInfo,
                IsSpecialResult = true
            };
            feignClientHttpRequest.RequestHeaderHandlers = new List<IRequestHeaderHandler>();
            IRequestHeaderHandler item = new RequestHeaderHandler(RequestAuthorizationAttribute.GetHeader(null, authorization));
            feignClientHttpRequest.RequestHeaderHandlers.Add(item);
            IRequestHeaderHandler item2 = new RequestHeaderHandler(RequestHeaderAttribute.GetHeader(null, header));
            feignClientHttpRequest.RequestHeaderHandlers.Add(item2);
            return base.SendAsync<byte[]>(feignClientHttpRequest);
        }

        // Token: 0x0600000A RID: 10 RVA: 0x000022CC File Offset: 0x000004CC
        [GetMapping("stream")]
        public Task<HttpResponseMessage> GetHttpResponseMessageAsync()
        {
            string uri = "stream";
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "GetHttpResponseMessageAsync";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.GetHttpResponseMessageAsync());
            }
            FeignClientHttpRequest request = new FeignClientHttpRequest(this.BaseUrl, "stream", uri, "GET", null)
            {
                Method = feignClientMethodInfo,
                IsSpecialResult = true
            };
            return base.SendAsync<HttpResponseMessage>(request);
        }

        // Token: 0x0600000B RID: 11 RVA: 0x00002348 File Offset: 0x00000548
        [RequestMapping("{id}", Method = "POST")]
        public string PostJson([PathVariable] int id, [RequestBody] TestServiceParam param)
        {
            string uri = "{id}";
            uri = base.ReplaceToStringPathVariable<int>(uri, "id", id);
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "PostJson";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.PostJson(int, TestServiceParam));
            }
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "{id}", uri, "POST", null)
            {
                Method = feignClientMethodInfo,
                IsSpecialResult = true
            };
            feignClientHttpRequest.RequestContent = new FeignClientHttpJsonRequestContent<TestServiceParam>("param", param);
            return base.Send<string>(feignClientHttpRequest);
        }

        // Token: 0x0600000C RID: 12 RVA: 0x000023E4 File Offset: 0x000005E4
        [RequestMapping("{id}", Method = "POST")]
        public string PostForm(int id, [RequestForm] TestServiceParam param)
        {
            string uri = "{id}";
            uri = base.ReplaceToStringPathVariable<int>(uri, "id", id);
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "PostForm";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.PostForm(int, TestServiceParam));
            }
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "{id}", uri, "POST", null)
            {
                Method = feignClientMethodInfo,
                IsSpecialResult = true
            };
            feignClientHttpRequest.RequestContent = new FeignClientHttpFormRequestContent<TestServiceParam>("param", param);
            return base.Send<string>(feignClientHttpRequest);
        }

        // Token: 0x0600000D RID: 13 RVA: 0x00002480 File Offset: 0x00000680
        [PostMapping("/api/test/upload")]
        public string UploadFile(IHttpRequestFile file1, IHttpRequestFile file2)
        {
            string uri = "/api/test/upload";
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "UploadFile(Feign.Request.IHttpRequestFile,Feign.Request.IHttpRequestFile)";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.UploadFile(IHttpRequestFile, IHttpRequestFile));
            }
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/api/test/upload", uri, "POST", null)
            {
                Method = feignClientMethodInfo,
                IsSpecialResult = true
            };
            FeignClientHttpRequest feignClientHttpRequest2 = feignClientHttpRequest;
            FeignClientHttpMultipartFormRequestContent feignClientHttpMultipartFormRequestContent = new FeignClientHttpMultipartFormRequestContent();
            FeignClientHttpRequestContent content = new FeignClientHttpFileRequestContent("file1", file1);
            feignClientHttpMultipartFormRequestContent.AddContent("file1", content);
            FeignClientHttpRequestContent content2 = new FeignClientHttpFileRequestContent("file2", file2);
            feignClientHttpMultipartFormRequestContent.AddContent("file2", content2);
            feignClientHttpRequest2.RequestContent = feignClientHttpMultipartFormRequestContent;
            return base.Send<string>(feignClientHttpRequest);
        }

        // Token: 0x0600000E RID: 14 RVA: 0x0000253C File Offset: 0x0000073C
        [PostMapping("/api/test/upload")]
        public string UploadFile(IHttpRequestFileForm files)
        {
            string uri = "/api/test/upload";
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "UploadFile(Feign.Request.IHttpRequestFileForm)";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.UploadFile(IHttpRequestFileForm));
            }
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/api/test/upload", uri, "POST", null)
            {
                Method = feignClientMethodInfo,
                IsSpecialResult = true
            };
            feignClientHttpRequest.RequestContent = new FeignClientHttpFileFormRequestContent(files);
            return base.Send<string>(feignClientHttpRequest);
        }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x0600000F RID: 15 RVA: 0x000025C4 File Offset: 0x000007C4
        // (set) Token: 0x06000010 RID: 16 RVA: 0x000025D8 File Offset: 0x000007D8
        string ITestService.Name { get; set; }

        // Token: 0x04000001 RID: 1
        private static readonly string[] s_headers = new string[]
        {
            "Cache-Control:max-age=0"
        };
    }
```
</details>

## 支持继承父接口服务 : 

```csharp
    [FeignClient("test-service", Url = "http://testservice.xx.com")]
    [NonFeignClient]
    public interface ITestParentService<TModel>
    {
        /// <summary>
        /// async get一个请求
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RequestMapping("/{id}", Method = "GET")]
        [MethodId("GetAsync")]
        Task<TModel> GetByIdAsync([PathVariable("id")]object id);
    }
    [RequestMapping("/api/test")]
    public interface ITestService:ITestParentService<string>
    {
    }
```

### 使用服务,这里以asp.net core为例

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


### 为了方便扩展,feign.net配置了一个管道事件,可以方便的自定义请求


```csharp
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IFeignClientPipeline<TService>
    {
        bool Enabled { get; set; }
        IFeignClientPipeline<TService> UseBuildingRequest(BuildingRequestDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseCancelRequest(CancelRequestDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseDisposing(DisposingDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseErrorRequest(ErrorRequestDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseFallbackRequest(FallbackRequestDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseInitializing(InitializingDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseReceivingResponse(ReceivingResponseDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseReceivedResponse(ReceivedResponseDelegate<TService> middleware);
        IFeignClientPipeline<TService> UseSendingRequest(SendingRequestDelegate<TService> middleware);
    }
```

### Options中包含一个全局的管道事件,可以根据参数获取指定服务的管道事件

```csharp

    /// <summary>
    /// 全局Pipeline
    /// </summary>
    public interface IGlobalFeignClientPipeline : IFeignClientPipeline<object>
    {
        /// <summary>
        /// 获取指定的服务Pipeline
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        IFeignClientPipeline<object> GetServicePipeline(string serviceId);
        /// <summary>
        /// 获取指定的服务Pipeline
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        IFeignClientPipeline<object> GetOrAddServicePipeline(string serviceId);
        /// <summary>
        /// 获取指定的服务Pipeline
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        IFeignClientPipeline<TService> GetServicePipeline<TService>();
        /// <summary>
        /// 获取指定的服务Pipeline
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        IFeignClientPipeline<TService> GetOrAddServicePipeline<TService>();
    }

```

关于管道的详细说明请参考文档 https://github.com/daixinkai/feign.net/wiki/Pipeline


# Usage

## DependencyInjection

```csharp

IDependencyInjectionFeignBuilder feignBuilder = services.AddFeignClients(options=>
{
    options.Assemblies.Add(typeof(ITestService).Assembly);
    options.Lifetime = FeignClientLifetime.Singleton;
    options.IncludeMethodMetadata = true;
    //````
});

```

## Autofac

```csharp

ContainerBuilder containerBuilder = new ContainerBuilder();
IAutofacFeignBuilder feignBuilder = containerBuilder.AddFeignClients(options=>
{
    options.Assemblies.Add(typeof(ITestService).Assembly);
    options.Lifetime = FeignClientLifetime.Singleton;
    options.IncludeMethodMetadata = true;
    //````
});

```

## CastleWindsor

```csharp
IWindsorContainer windsorContainer = new WindsorContainer();
ICastleWindsorFeignBuilder feignBuilder = services.AddFeignClients(options=>
{
    options.Assemblies.Add(typeof(ITestService).Assembly);
    options.Lifetime = FeignClientLifetime.Singleton;
    options.IncludeMethodMetadata = true;
    //````
});

```





## 扩展组件

[Steeltoe](https://github.com/daixinkai/feign.net/tree/master/src/Feign.Steeltoe) -- enable .NET Core and .NET Framework apps to easily leverage Netflix Eureka, Hystrix, Spring Cloud Config Server, and Cloud Foundry services:https://github.com/SteeltoeOSS/Samples

[Polly](https://github.com/daixinkai/feign.net/tree/master/src/Feign.Polly) -- Polly是一个被.NET基金会认可的弹性和瞬态故障处理库，允许我们以非常顺畅和线程安全的方式来执诸如行重试，断路，超时，故障恢复等策略 https://github.com/App-vNext/Polly


# 未完待续

