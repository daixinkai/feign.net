using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Tests
{
    public class TestControllerServiceFallback : ITestControllerService
    {
        public IQueryResult<TestServiceParam> GetQueryResultValue([PathVariable("id")] string id, [RequestQuery] TestServiceParam param)
        {
            return new QueryResult<TestServiceParam>()
            {
                Data = param,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
        }

        public Task<IQueryResult<TestServiceParam>> GetQueryResultValueAsync([PathVariable("id")] string id, [RequestQuery] TestServiceParam param)
        {
            return Task.FromResult<IQueryResult<TestServiceParam>>(new QueryResult<TestServiceParam>()
            {
                Data = param,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }

        public Task<IQueryResult<TestServiceParam>> GetQueryResultValueAsync([PathVariable("id")] string id, [RequestQuery] int? value)
        {
            throw new NotImplementedException();
        }
    }
}
