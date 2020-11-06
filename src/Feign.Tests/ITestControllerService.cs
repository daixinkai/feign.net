using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    [CustomFeignClient("yun-platform-service-provider"
, Fallback = typeof(TestControllerServiceFallback)
//, Url = "http://localhost:55298/"
)]
    [RequestMapping("/api/test")]
    [Headers("Cache-Control:max-age=0", "Accept-Encoding: gzip, deflate, br", "Accept-Language: zh-CN,zh;q=0.9,en;q=0.8")]
    public interface ITestControllerService
    {
        [ResultType(typeof(QueryResult<>))]
        [Headers("Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9", "Cache-Control:max-age=0")]
        [RequestMapping("/{id}", Method = "GET", Accept = "text/html"
            , CompletionOption = System.Net.Http.HttpCompletionOption.ResponseHeadersRead
            )]
        Task<IQueryResult<TestServiceParam>> GetQueryResultValueAsync([PathVariable("id")] string id, [RequestQuery] TestServiceParam param);

        Task<IQueryResult<TestServiceParam>> GetQueryResultValueAsync([PathVariable("id")] string id, [RequestQuery] int? value);

        [ResultType(typeof(QueryResult<>))]
        [Headers("Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9", "Cache-Control:max-age=0")]
        [RequestMapping("/{id}", Method = "GET", Accept = "text/html")]
        IQueryResult<TestServiceParam> GetQueryResultValue([PathVariable("id")] string id, [RequestQuery] TestServiceParam param);

    }
}
