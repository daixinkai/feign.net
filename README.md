# feign.net

*spring cloud feign for .net*

## feign.net是一个spring cloud feign组件的c#移植版

### 定义服务 : 

```csharp
    [FeignClient("test-service", Url = "http://testservice.xx.com")]
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
        [GetMapping("/stream")]
        Task<Stream> GetStreamAsync();
        /// <summary>
        /// 获取buffer
        /// </summary>
        /// <param name="authorization">sample: scheme parameter</param>
        /// <param name="header">sample: name:value</param>
        /// <returns></returns>
        [GetMapping("/stream")]
        Task<byte[]> GetBufferAsync([RequestAuthorization] string authorization, [RequestHeader] string header);
        /// <summary>
        /// 获取HttpResponseMessage
        /// </summary>
        /// <returns></returns>
        [GetMapping("/stream")]
        Task<HttpResponseMessage> GetHttpResponseMessageAsync();

        /// <summary>
        /// 以json的方式post一个请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [RequestMapping("/{id}", Method = "POST")]
        //[PostMapping("/{id}")]
        string PostJson([PathVariable] int id, [RequestBody] TestServiceParam param);

        /// <summary>
        /// 以form表单的方式post一个请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [RequestMapping("/{id}", Method = "POST")]
        //[PostMapping("/{id}")]
        string PostForm(int id, [RequestForm] TestServiceParam param);

        /// <summary>
        /// 上传2个文件
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        [PostMapping("/upload")]
        string UploadFile(IHttpRequestFile file1, IHttpRequestFile file2);

        /// <summary>
        /// 上传多个文件
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        [PostMapping("/upload")]
        string UploadFile(IHttpRequestFileForm files);

    }
```

## Emit代理类生成代码
```csharp
    // Token: 0x02000002 RID: 2
    [StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
    public class ITestService_Proxy_514A26D2BB864478AB253488510FBC97 : FeignClientHttpProxy<ITestService>, ITestService
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        public ITestService_Proxy_514A26D2BB864478AB253488510FBC97(IFeignOptions feignOptions, IServiceDiscovery serviceDiscovery, ICacheProvider cacheProvider, ILoggerFactory loggerFactory) : base(feignOptions, serviceDiscovery, cacheProvider, loggerFactory)
        {
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public override string ServiceId
        {
            get
            {
                return "test-service";
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000003 RID: 3 RVA: 0x0000208C File Offset: 0x0000028C
        public override string BaseUri
        {
            get
            {
                return "/api/test";
            }
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000004 RID: 4 RVA: 0x000020A0 File Offset: 0x000002A0
        public override string Url
        {
            get
            {
                return "http://testservice.xx.com";
            }
        }

        // Token: 0x06000005 RID: 5 RVA: 0x000020B4 File Offset: 0x000002B4
        [RequestMapping("/{id}", Method = "GET")]
        [Headers(new string[]
        {
            "Cache-Control:max-age=0",
            "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36"
        })]
        [MethodId("GetAsync")]
        public Task<string> GetAsync([PathVariable("id")] int id, [RequestQuery] string name)
        {
            string text = "/{id}";
            text = base.ReplaceToStringPathVariable<int>(text, "id", id);
            text = base.ReplaceStringRequestQuery(text, "name", name);
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/{id}", text, "GET", null)
            {
                CompletionOption = HttpCompletionOption.ResponseContentRead
            };
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "GetAsync";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.GetAsync(int, string));
            }
            feignClientHttpRequest.Method = feignClientMethodInfo;
            feignClientHttpRequest.Headers = new string[]
            {
                "Cache-Control:max-age=0",
                "Cache-Control:max-age=0",
                "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36"
            };
            return base.SendAsync<string>(feignClientHttpRequest);
        }

        // Token: 0x06000006 RID: 6 RVA: 0x00002190 File Offset: 0x00000390
        [GetMapping("/stream")]
        public Task<Stream> GetStreamAsync()
        {
            string text = "/stream";
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/stream", text, "GET", null)
            {
                CompletionOption = HttpCompletionOption.ResponseContentRead
            };
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "GetStreamAsync()";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.GetStreamAsync());
            }
            feignClientHttpRequest.Method = feignClientMethodInfo;
            feignClientHttpRequest.Headers = new string[]
            {
                "Cache-Control:max-age=0"
            };
            return base.SendAsync<Stream>(feignClientHttpRequest);
        }

        // Token: 0x06000007 RID: 7 RVA: 0x00002230 File Offset: 0x00000430
        [GetMapping("/stream")]
        public Task<byte[]> GetBufferAsync([RequestAuthorization] string authorization, [RequestHeader] string header)
        {
            string text = "/stream";
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/stream", text, "GET", null)
            {
                CompletionOption = HttpCompletionOption.ResponseContentRead
            };
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "GetBufferAsync(System.String,System.String)";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.GetBufferAsync(string, string));
            }
            feignClientHttpRequest.Method = feignClientMethodInfo;
            feignClientHttpRequest.Headers = new string[]
            {
                "Cache-Control:max-age=0"
            };
            feignClientHttpRequest.RequestHeaderHandlers = new List<IRequestHeaderHandler>();
            IRequestHeaderHandler item = new RequestHeaderHandler(RequestAuthorizationAttribute.GetHeader(null, authorization));
            feignClientHttpRequest.RequestHeaderHandlers.Add(item);
            IRequestHeaderHandler item2 = new RequestHeaderHandler(null, header);
            feignClientHttpRequest.RequestHeaderHandlers.Add(item2);
            return base.SendAsync<byte[]>(feignClientHttpRequest);
        }

        // Token: 0x06000008 RID: 8 RVA: 0x0000231C File Offset: 0x0000051C
        [GetMapping("/stream")]
        public Task<HttpResponseMessage> GetHttpResponseMessageAsync()
        {
            string text = "/stream";
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/stream", text, "GET", null)
            {
                CompletionOption = HttpCompletionOption.ResponseContentRead
            };
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "GetHttpResponseMessageAsync()";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.GetHttpResponseMessageAsync());
            }
            feignClientHttpRequest.Method = feignClientMethodInfo;
            feignClientHttpRequest.Headers = new string[]
            {
                "Cache-Control:max-age=0"
            };
            return base.SendAsync<HttpResponseMessage>(feignClientHttpRequest);
        }

        // Token: 0x06000009 RID: 9 RVA: 0x000023BC File Offset: 0x000005BC
        [RequestMapping("/{id}", Method = "POST")]
        public string PostJson([PathVariable] int id, [RequestBody] TestServiceParam param)
        {
            string text = "/{id}";
            text = base.ReplaceToStringPathVariable<int>(text, "id", id);
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/{id}", text, "POST", null)
            {
                CompletionOption = HttpCompletionOption.ResponseContentRead
            };
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "PostJson(System.Int32,Feign.Tests.TestServiceParam)";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.PostJson(int, TestServiceParam));
            }
            feignClientHttpRequest.Method = feignClientMethodInfo;
            feignClientHttpRequest.Headers = new string[]
            {
                "Cache-Control:max-age=0"
            };
            feignClientHttpRequest.RequestContent = new FeignClientHttpJsonRequestContent<TestServiceParam>("param", param);
            return base.Send<string>(feignClientHttpRequest);
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002484 File Offset: 0x00000684
        [RequestMapping("/{id}", Method = "POST")]
        public string PostForm(int id, [RequestForm] TestServiceParam param)
        {
            string text = "/{id}";
            text = base.ReplaceToStringPathVariable<int>(text, "id", id);
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/{id}", text, "POST", null)
            {
                CompletionOption = HttpCompletionOption.ResponseContentRead
            };
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "PostForm(System.Int32,Feign.Tests.TestServiceParam)";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.PostForm(int, TestServiceParam));
            }
            feignClientHttpRequest.Method = feignClientMethodInfo;
            feignClientHttpRequest.Headers = new string[]
            {
                "Cache-Control:max-age=0"
            };
            feignClientHttpRequest.RequestContent = new FeignClientHttpFormRequestContent<TestServiceParam>("param", param);
            return base.Send<string>(feignClientHttpRequest);
        }

        // Token: 0x0600000B RID: 11 RVA: 0x0000254C File Offset: 0x0000074C
        [PostMapping("/upload")]
        public string UploadFile(IHttpRequestFile file1, IHttpRequestFile file2)
        {
            string text = "/upload";
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/upload", text, "POST", null)
            {
                CompletionOption = HttpCompletionOption.ResponseContentRead
            };
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "UploadFile(Feign.Request.IHttpRequestFile,Feign.Request.IHttpRequestFile)";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.UploadFile(IHttpRequestFile, IHttpRequestFile));
            }
            feignClientHttpRequest.Method = feignClientMethodInfo;
            feignClientHttpRequest.Headers = new string[]
            {
                "Cache-Control:max-age=0"
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

        // Token: 0x0600000C RID: 12 RVA: 0x00002634 File Offset: 0x00000834
        [PostMapping("/upload")]
        public string UploadFile(IHttpRequestFileForm files)
        {
            string text = "/upload";
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/upload", text, "POST", null)
            {
                CompletionOption = HttpCompletionOption.ResponseContentRead
            };
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "UploadFile(Feign.Request.IHttpRequestFileForm)";
            if (base.FeignOptions.IncludeMethodMetadata)
            {
                feignClientMethodInfo.MethodMetadata = methodof(ITestService.UploadFile(IHttpRequestFileForm));
            }
            feignClientHttpRequest.Method = feignClientMethodInfo;
            feignClientHttpRequest.Headers = new string[]
            {
                "Cache-Control:max-age=0"
            };
            feignClientHttpRequest.RequestContent = new FeignClientHttpFileFormRequestContent(files);
            return base.Send<string>(feignClientHttpRequest);
        }

        // Token: 0x17000004 RID: 4
        // (get) Token: 0x0600000D RID: 13 RVA: 0x000026E4 File Offset: 0x000008E4
        // (set) Token: 0x0600000E RID: 14 RVA: 0x000026F8 File Offset: 0x000008F8
        string ITestService.Name
        {
            [CompilerGenerated]
            get
            {
                return this.< Feign.Tests.ITestService.Name > k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                this.< Feign.Tests.ITestService.Name > k__BackingField = value;
            }
        }

        // Token: 0x04000001 RID: 1
        [CompilerGenerated]
        private string <Feign.Tests.ITestService.Name>k__BackingField;
	}
```
##

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

