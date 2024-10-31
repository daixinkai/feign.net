using Feign.Tests;
using Feign.Proxy;
using Feign.Reflection;
using Feign.Request;

namespace Feign.TestWeb
{

    public class TestServiceClientFallback : FallbackFeignClientHttpProxy<ITestServiceClient, TestServiceClientFallback>, ITestServiceClient
    {
        public TestServiceClientFallback(TestServiceClientFallback fallback, FeignClientHttpProxyOptions<ITestServiceClient> options) : base(fallback, options)
        {
        }

        public override string ServiceId => throw new NotImplementedException();

        public IQueryResult<TestServiceParam> GetQueryResultValue([RequestQuery] string id, [RequestQuery] int[] values)
        {
            throw new NotImplementedException();
        }
        public ValueTask<IQueryResult<TestServiceParam>> GetQueryResultValueAsync([RequestQuery] string id, [RequestQuery] int[] values, Dictionary<string, object>? map = null, TestServiceParam? param = null)
        {
            string text = "";
            text = base.ReplaceStringPathVariable(text, "id", id);
            text = base.ReplaceRequestQuery<int[]>(text, "values", values);
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "GetQueryResultValueAsync(String,System.Int32[])";
            feignClientMethodInfo.ResultType = typeof(QueryResult<TestServiceParam>);
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, null, text, "GET", null)
            {
                Method = feignClientMethodInfo
            };
            return new ValueTask<IQueryResult<TestServiceParam>>(base.SendAsync<IQueryResult<TestServiceParam>>(feignClientHttpRequest)!);
        }

        public ValueTask<IQueryResult<TestServiceParam>> GetQueryResultValueFallbackAsync([RequestQuery] string id, [RequestQuery] int[] values)
        {
            string text = "";
            text = base.ReplaceStringPathVariable(text, "id", id);
            text = base.ReplaceRequestQuery<int[]>(text, "values", values);
            FeignClientMethodInfo feignClientMethodInfo = new FeignClientMethodInfo();
            feignClientMethodInfo.MethodId = "GetQueryResultValueAsync(String,System.Int32[])";
            feignClientMethodInfo.ResultType = typeof(QueryResult<TestServiceParam>);
            FeignClientHttpRequest feignClientHttpRequest = new FeignClientHttpRequest(this.BaseUrl, null, text, "GET", null)
            {
                Method = feignClientMethodInfo
            };
            return new ValueTask<IQueryResult<TestServiceParam>>(
                base.SendAsync<IQueryResult<TestServiceParam>>(feignClientHttpRequest, null!)!
                );
        }

    }
}
