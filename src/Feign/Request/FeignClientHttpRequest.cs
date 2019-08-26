using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    public class FeignClientHttpRequest
    {
        public FeignClientHttpRequest(string baseUrl, string mappingUri, string uri, string httpMethod, string contentType, FeignClientHttpRequestContent requestContent, MethodInfo method)
        {
            BaseUrl = baseUrl;
            MappingUri = mappingUri;
            Uri = uri;
            HttpMethod = httpMethod;
            RequestContent = requestContent;
            Method = method;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                contentType = "application/json; charset=utf-8";
            }
            MediaTypeHeaderValue mediaTypeHeaderValue;
            if (!MediaTypeHeaderValue.TryParse(contentType, out mediaTypeHeaderValue))
            {
                throw new ArgumentException("ContentType error");
            }
            MediaType = mediaTypeHeaderValue.MediaType;
            ContentType = mediaTypeHeaderValue;
        }
        public string BaseUrl { get; }
        public string MappingUri { get; }
        public string Uri { get; }
        public string HttpMethod { get; }
        public MediaTypeHeaderValue ContentType { get; }
        public string MediaType { get; }
        public FeignClientHttpRequestContent RequestContent { get; }
        public MethodInfo Method { get; }
        public HttpContent GetHttpContent()
        {
            return RequestContent?.GetHttpContent(ContentType);
        }

    }
}
