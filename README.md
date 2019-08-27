# feign.net
spring cloud feign for .net


```
            services.AddFeignClients(options =>
            {
                options.Assemblies.Add(typeof(ITestService).Assembly);
                options.Lifetime = FeignClientLifetime.Singleton;
                options.Lifetime = FeignClientLifetime.Scoped;
                options.Lifetime = FeignClientLifetime.Transient;
                options.FeignClientPipeline.Authorization(proxy =>
                {
                    return ("global", "asdasd");
                });
                options.FeignClientPipeline.Service("serviceId").BuildingRequest += (sender, e) =>
                {
                    IFallbackFeignClient<object> fallbackFeignClient = e.FeignClient as IFallbackFeignClient<object>;
                    object fallback = fallbackFeignClient?.Fallback;
                };

                options.FeignClientPipeline.Service("yun-platform-service-provider").Authorization(proxy =>
                {
                    return ("service", "asdasd");
                });

                options.FeignClientPipeline.Service("yun-platform-service-provider").ReceivingResponse += (sender, e) =>
                {

                };

                options.FeignClientPipeline.ReceivingResponse += (sender, e) =>  
                {
                    if (!typeof(QueryResult).IsAssignableFrom(e.ResultType))
                    {
                        return;
                    }
                    if (e.ResultType == typeof(QueryResult))
                    {
                        e.Result = new QueryResult()
                        {
                            StatusCode = e.ResponseMessage.StatusCode
                        };
                        return;
                    }

                    if (e.ResultType.IsGenericType && e.ResultType.GetGenericTypeDefinition() == typeof(QueryResult<>))
                    {
                        QueryResult queryResult;
                        if (e.ResponseMessage.IsSuccessStatusCode)
                        {
                            string json = e.ResponseMessage.Content.ReadAsStringAsync().Result;
                            object data = Newtonsoft.Json.JsonConvert.DeserializeObject(json, e.ResultType.GetGenericArguments()[0]);
                            var constructor = e.ResultType.GetConstructor(new Type[] { typeof(object) });
                            queryResult = (QueryResult)constructor.Invoke(new object[] { data });
                        }
                        else
                        {
                            queryResult = (QueryResult)e.ResultType.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                        }
                        queryResult.StatusCode = e.ResponseMessage.StatusCode;
                        e.Result = queryResult;
                    }

                };

                options.FeignClientPipeline.CancelRequest += (sender, e) =>
                {
                    e.CancellationToken.Register((state) =>
                    {

                    }, sender);
                };
                options.FeignClientPipeline.ErrorRequest += (sender, e) =>
                {
                    Exception exception = e.Exception;
                    //e.ExceptionHandled = true;
                };
            });
``` 


```

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> Get(int id, [FromServices] ITestService testService)
        {
            await testService.PostValueAsync(id, "", new TestServiceParam());
			testService.GetValueVoidAsync(id, "", null);
            return testService.GetQueryResultValue(id.ToString(), new TestServiceParam
            {
                Name = "asasdsad"
            });
        }
    }

```