# feign.net

*spring cloud feign for .net*

## feign.net是一个spring cloud feign组件的c#移植版


### 定义服务 : 

```csharp
    [FeignClient("test-service", Url = "http://testservice.xx.com")]
    [RequestMapping("/api/test")]
    public interface ITestService
    {
        /// <summary>
        /// async get一个请求
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [RequestMapping("/{id}", Method = "GET")]
        //[GetMapping("/{id}")]
		[MethodId("GetAsync")]
        Task<string> GetAsync([PathVariable("id")]int id);

        /// <summary>
        /// 以json的方式post一个请求
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [RequestMapping("/{id}", Method = "POST")]
        //[PostMapping("/{id}")]
        string PostJson([PathVariable]int id, [RequestBody] TestServiceParam param);

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
    /// 工作Pipeline
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IFeignClientPipeline<TService>
    {
        bool Enabled { get; set; }
        event EventHandler<IBuildingRequestEventArgs<TService>> BuildingRequest;
        event EventHandler<ISendingRequestEventArgs<TService>> SendingRequest;
        event EventHandler<ICancelRequestEventArgs<TService>> CancelRequest;
        event EventHandler<IErrorRequestEventArgs<TService>> ErrorRequest;
        event EventHandler<IReceivingResponseEventArgs<TService>> ReceivingResponse;
        event EventHandler<IInitializingEventArgs<TService>> Initializing;
        event EventHandler<IDisposingEventArgs<TService>> Disposing;
        event EventHandler<IFallbackRequestEventArgs<TService>> FallbackRequest;
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

