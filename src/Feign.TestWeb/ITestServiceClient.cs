using Feign.Tests;

namespace Feign.TestWeb
{
    [FeignClient("yun-platform-service-provider")]
    [RequestMapping("/api/test")]
    //[Headers("Cache-Control:max-age=0", "Accept-Encoding: gzip, deflate, br", "Accept-Language: zh-CN,zh;q=0.9,en;q=0.8")]
    public interface ITestServiceClient
    {
        //[GetMapping("/")]
        [ResultType(typeof(QueryResult<>))]
        [Headers("methodHeader:true")]
        ValueTask<IQueryResult<TestServiceParam>> GetQueryResultValueAsync([RequestQuery] string id, [RequestQuery] int[] values);
        [ResultType(typeof(QueryResult<>))]
        [Headers("methodHeader:true")]
        IQueryResult<TestServiceParam> GetQueryResultValue([RequestQuery] string id, [RequestQuery] int[] values);
    }
}
