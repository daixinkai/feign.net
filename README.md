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
        /// <summary>
        /// async get一个请求
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Headers("Cache-Control:max-age=0","User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.122 Safari/537.36")]
        [RequestMapping("/{id}", Method = "GET")]
        //[GetMapping("/{id}")]
        [MethodId("GetAsync")]
        Task<string> GetAsync([PathVariable("id")]int id);
        /// <summary>
        /// 获取流
        /// </summary>
        /// <returns></returns>
        [GetMapping("/stream")]
        Task<Stream> GetStreamAsync();
        /// <summary>
        /// 获取buffer
        /// </summary>
        /// <returns></returns>
        [GetMapping("/stream")]
        Task<byte[]> GetBufferAsync();
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

## 代理类生成效果
```csharp
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Feign.Cache;
using Feign.Discovery;
using Feign.Logging;
using Feign.Proxy;
using Feign.Reflection;
using Feign.Request;
using Feign.Request.Headers;

namespace Feign.Tests
{
	// Token: 0x02000011 RID: 17
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	public class ITestControllerService_Proxy_85406ED1ACC54D4D99866BE1BC4F2549 : FeignClientHttpProxy<ITestControllerService>, ITestControllerService
	{
		// Token: 0x0600006F RID: 111 RVA: 0x000040E0 File Offset: 0x000022E0
		public ITestControllerService_Proxy_85406ED1ACC54D4D99866BE1BC4F2549(IFeignOptions feignOptions, IServiceDiscovery serviceDiscovery, ICacheProvider cacheProvider, ILoggerFactory loggerFactory) : base(feignOptions, serviceDiscovery, cacheProvider, loggerFactory)
		{
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000070 RID: 112 RVA: 0x00004108 File Offset: 0x00002308
		public override string ServiceId
		{
			get
			{
				return "yun-platform-service-provider";
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000071 RID: 113 RVA: 0x0000411C File Offset: 0x0000231C
		public override string BaseUri
		{
			get
			{
				return "/api/test";
			}
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004130 File Offset: 0x00002330
		[RequestMapping("/{id}", CompletionOption = HttpCompletionOption.ResponseHeadersRead, Method = "GET", ContentType = "application/json", Accept = "text/html")]
		[Headers(new string[]
		{
			"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
			"Cache-Control:max-age=0"
		})]
		[ResultType(typeof(QueryResult<>))]
		public Task<IQueryResult<TestServiceParam>> GetQueryResultValueAsync([PathVariable("id")] string id, [RequestQuery] TestServiceParam param)
		{
			string text = "/{id}";
			text = base.ReplaceStringPathVariable(text, "id", id);
			text = base.ReplaceRequestQuery<TestServiceParam>(text, "param", param);
			FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/{id}", text, "GET", "application/json")
			{
				CompletionOption = HttpCompletionOption.ResponseHeadersRead
			};
			FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
			feignClientMethodInfo.MethodId = "GetQueryResultValueAsync(System.String,Feign.Tests.TestServiceParam)";
			feignClientMethodInfo.ResultType = typeof(QueryResult<TestServiceParam>);
			if (base.FeignOptions.IncludeMethodMetadata)
			{
				feignClientMethodInfo.MethodMetadata = methodof(ITestControllerService.GetQueryResultValueAsync(string, TestServiceParam));
			}
			feignClientHttpRequest.Method = feignClientMethodInfo;
			feignClientHttpRequest.Accept = "text/html";
			feignClientHttpRequest.Headers = new string[]
			{
				"Cache-Control:max-age=0",
				"Accept-Encoding: gzip, deflate, br",
				"Accept-Language: zh-CN,zh;q=0.9,en;q=0.8",
				"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
				"Cache-Control:max-age=0"
			};
			return base.SendAsync<IQueryResult<TestServiceParam>>(feignClientHttpRequest);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004244 File Offset: 0x00002444
		[ResultType(typeof(QueryResult<>))]
		public Task<IQueryResult<TestServiceParam>> GetQueryResultValueAsync([PathVariable("id")] string id, [RequestQuery] int? value)
		{
			string text = "";
			text = base.ReplaceStringPathVariable(text, "id", id);
			text = base.ReplaceNullableRequestQuery<int>(text, "value", value);
			FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, null, text, "GET", null)
			{
				CompletionOption = HttpCompletionOption.ResponseContentRead
			};
			FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
			feignClientMethodInfo.MethodId = "GetQueryResultValueAsync(System.String,System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]])";
			feignClientMethodInfo.ResultType = typeof(QueryResult<TestServiceParam>);
			if (base.FeignOptions.IncludeMethodMetadata)
			{
				feignClientMethodInfo.MethodMetadata = methodof(ITestControllerService.GetQueryResultValueAsync(string, int?));
			}
			feignClientHttpRequest.Method = feignClientMethodInfo;
			feignClientHttpRequest.Headers = new string[]
			{
				"Cache-Control:max-age=0",
				"Accept-Encoding: gzip, deflate, br",
				"Accept-Language: zh-CN,zh;q=0.9,en;q=0.8"
			};
			return base.SendAsync<IQueryResult<TestServiceParam>>(feignClientHttpRequest);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000432C File Offset: 0x0000252C
		[ResultType(typeof(QueryResult<>))]
		public Task<IQueryResult<TestServiceParam>> GetQueryResultValueAsync([PathVariable("id")] string id, [RequestQuery] int[] values)
		{
			string text = "";
			text = base.ReplaceStringPathVariable(text, "id", id);
			text = base.ReplaceRequestQuery<int[]>(text, "values", values);
			FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, null, text, "GET", null)
			{
				CompletionOption = HttpCompletionOption.ResponseContentRead
			};
			FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
			feignClientMethodInfo.MethodId = "GetQueryResultValueAsync(System.String,System.Int32[])";
			feignClientMethodInfo.ResultType = typeof(QueryResult<TestServiceParam>);
			if (base.FeignOptions.IncludeMethodMetadata)
			{
				feignClientMethodInfo.MethodMetadata = methodof(ITestControllerService.GetQueryResultValueAsync(string, int[]));
			}
			feignClientHttpRequest.Method = feignClientMethodInfo;
			feignClientHttpRequest.Headers = new string[]
			{
				"Cache-Control:max-age=0",
				"Accept-Encoding: gzip, deflate, br",
				"Accept-Language: zh-CN,zh;q=0.9,en;q=0.8"
			};
			return base.SendAsync<IQueryResult<TestServiceParam>>(feignClientHttpRequest);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004414 File Offset: 0x00002614
		[ResultType(typeof(QueryResult<>))]
		[Headers(new string[]
		{
			"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
			"Cache-Control:max-age=0"
		})]
		[RequestMapping("/{id}", Method = "GET", Accept = "text/html")]
		public IQueryResult<TestServiceParam> GetQueryResultValue([PathVariable("id")] int? id, [RequestQuery] TestServiceParam param, [RequestHeader] string header, [RequestAuthorization] int? authorization)
		{
			string text = "/{id}";
			text = base.ReplaceNullablePathVariable<int>(text, "id", id);
			text = base.ReplaceRequestQuery<TestServiceParam>(text, "param", param);
			FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, "/{id}", text, "GET", null)
			{
				CompletionOption = HttpCompletionOption.ResponseContentRead
			};
			FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
			feignClientMethodInfo.MethodId = "GetQueryResultValue(System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]],Feign.Tests.TestServiceParam,System.String,System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]])";
			feignClientMethodInfo.ResultType = typeof(QueryResult<TestServiceParam>);
			if (base.FeignOptions.IncludeMethodMetadata)
			{
				feignClientMethodInfo.MethodMetadata = methodof(ITestControllerService.GetQueryResultValue(int?, TestServiceParam, string, int?));
			}
			feignClientHttpRequest.Method = feignClientMethodInfo;
			feignClientHttpRequest.Accept = "text/html";
			feignClientHttpRequest.Headers = new string[]
			{
				"Cache-Control:max-age=0",
				"Accept-Encoding: gzip, deflate, br",
				"Accept-Language: zh-CN,zh;q=0.9,en;q=0.8",
				"Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9",
				"Cache-Control:max-age=0"
			};
			feignClientHttpRequest.RequestHeaderHandlers = new List<IRequestHeaderHandler>();
			IRequestHeaderHandler item = new RequestHeaderHandler(null, header);
			feignClientHttpRequest.RequestHeaderHandlers.Add(item);
			string text2 = StringValueMethods.NullableToString<int>(authorization);
			IRequestHeaderHandler item2 = new RequestHeaderHandler(RequestAuthorizationAttribute.GetHeader(null, text2));
			feignClientHttpRequest.RequestHeaderHandlers.Add(item2);
			return base.Send<IQueryResult<TestServiceParam>>(feignClientHttpRequest);
		}
	}
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

