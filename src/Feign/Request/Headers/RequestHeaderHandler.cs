using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request.Headers
{
    public sealed class RequestHeaderHandler : IRequestHeaderHandler
    {
        public RequestHeaderHandler(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public RequestHeaderHandler(KeyValuePair<string, string> header)
        {
            Name = header.Key;
            Value = header.Value;
        }

        public string Name { get; }
        public string Value { get; }

        public void SetHeader(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Headers.TryAddWithoutValidation(Name, Value);
        }
    }
}
