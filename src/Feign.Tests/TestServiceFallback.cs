using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Feign.Request;
using Newtonsoft.Json.Linq;

namespace Feign.Tests
{
    public class TestServiceFallback : ITestService
    {

        public TestServiceFallback() : base()
        {
        }


        //public FallbackTestService(ITestService testService1, FallbackTestService testService2) : base()
        //{
        //    _testService1 = testService1;
        //    _testService2 = testService2;
        //}

        ITestService _testService1;

        TestServiceFallback _testService2;

        public string Name => throw new NotImplementedException();

        public Task PostValueAsync()
        {
            return Task.FromResult<object>(null);
        }

        public Task<QueryResult<JObject>> GetQueryResultValueAsync([PathVariable("id")] string id, [RequestQuery] TestServiceParam param)
        {
            try
            {
                return Task.FromResult(new QueryResult<JObject>());
            }
            catch (Exception)
            {
                return _testService1.GetQueryResultValueAsync(id, param);
            }
        }

        public QueryResult<JObject> GetQueryResultValue([PathVariable("id")] string id, [RequestQuery] TestServiceParam param)
        {
            try
            {
                return new QueryResult<JObject>();
            }
            catch (Exception)
            {
                return _testService1.GetQueryResultValue(id, param);
            }
        }

        public Task<JObject> GetValueAsync([PathVariable("id")] string id)
        {
            Func<Task<JObject>> fallback = new Func<Task<JObject>>(delegate
            {
                return _testService2.GetValueAsync(id);
            });
            return GetValueAsync(id, fallback);
        }

        public Task<JObject> GetValueAsync([PathVariable("id")] string id, Func<Task<JObject>> fallback)
        {
            try
            {
                return Task.FromResult<JObject>(null);
            }
            catch (Exception)
            {
                return new Func<Task<JObject>>(() => fallback.Invoke()).Invoke();
                throw;
            }
        }

        public Task<JObject> GetValueAsync([PathVariable] int id, [RequestParam] string test)
        {
            throw new NotImplementedException();
        }

        public Task<JObject> GetValueAsync([PathVariable] int id, [RequestQuery] TestServiceParam param)
        {
            throw new NotImplementedException();
        }

        public Task<JObject> GetValueAsync([PathVariable] int id, [RequestParam] string test, [RequestQuery] TestServiceParam param)
        {
            throw new NotImplementedException();
        }

        public void GetValueVoid([PathVariable] int id, [RequestParam] string test, [RequestQuery] TestServiceParam param)
        {
            try
            {
                Task.FromResult(new QueryResult<JObject>());
            }
            catch (Exception)
            {
                _testService1.GetValueVoidAsync(id, test, param);
            }
        }
        public Task GetValueVoidAsync([PathVariable] int id, [RequestParam] string test, [RequestQuery] TestServiceParam param)
        {
            try
            {
                return Task.FromResult(new QueryResult<JObject>());
            }
            catch (Exception)
            {
                return _testService1.GetValueVoidAsync(id, test, param);
            }
        }
        public Task PostValueAsync([PathVariable] int id, [RequestParam] string test, [RequestBody] TestServiceParam param)
        {
            //return _testService1.PostValueAsync(id, test, param);
            return Task.FromResult<object>(null);
        }

        public void GetValueVoid([PathVariable] int id, [RequestParam] TestServiceParam queryParam, [RequestQuery] TestServiceParam param)
        {

        }

        public Task<string> UploadFileAsync(IHttpRequestFile file, TestServiceParam param)
        {
            throw new NotImplementedException();
        }

        public Task PostValueFormAsync([PathVariable] int id, [RequestParam] string test, [RequestForm] TestServiceParam param)
        {
            throw new NotImplementedException();
        }

        public Task PostValueForm2Async([PathVariable] int id, [RequestParam] string test, [RequestForm] TestServiceParam param1, [RequestForm] TestServiceParam param2)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadFilesAsync(IHttpRequestFile file1, IHttpRequestFile file2, IHttpRequestFile file3)
        {
            throw new NotImplementedException();
        }

        public Task<string> FormTestAsync([RequestForm] TestServiceParam param)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadFileAsync(IHttpRequestFile file, [RequestForm] string name)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadFileAsync(TestServiceUploadFileParam param)
        {
            throw new NotImplementedException();
        }

        public string Get()
        {
            throw new NotImplementedException();
        }
    }
}
