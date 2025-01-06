using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request.Transforms
{
    public sealed class HttpRequestHeaderTransform : IHttpRequestTransform
    {
        public HttpRequestHeaderTransform(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public HttpRequestHeaderTransform(KeyValuePair<string, string> header)
        {
            Name = header.Key;
            Value = header.Value;
        }

        public string Name { get; }
        public string Value { get; }

        public ValueTask ApplyAsync(FeignHttpRequestMessage request)
        {
            request.Headers.TryAddWithoutValidation(Name, Value);
            return TaskEx.CompletedValueTask;
        }
    }
}
