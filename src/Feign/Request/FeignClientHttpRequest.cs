using Feign.Internal;
using Feign.Reflection;
using Feign.Request.Headers;
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
    /// <summary>
    /// 一个FeignClientHttp请求
    /// </summary>
    public class FeignClientHttpRequest
    {
        public FeignClientHttpRequest(string baseUrl, string mappingUri, string uri, string httpMethod, string contentType)
        {
            BaseUrl = baseUrl;
            MappingUri = mappingUri;
            Uri = uri;
            HttpMethod = httpMethod;
            if (!string.IsNullOrWhiteSpace(contentType))
            {
                MediaTypeHeaderValue mediaTypeHeaderValue;
                if (!MediaTypeHeaderValue.TryParse(contentType, out mediaTypeHeaderValue))
                {
                    throw new ArgumentException("ContentType error");
                }
                MediaType = mediaTypeHeaderValue.MediaType;
                ContentType = mediaTypeHeaderValue;
            }
        }
        /// <summary>
        /// 获取BaseUrl
        /// </summary>
        public string BaseUrl { get; }
        /// <summary>
        /// 获取映射的Uri
        /// </summary>
        public string MappingUri { get; }
        /// <summary>
        /// 获取真实Uri
        /// </summary>
        public string Uri { get; }
        /// <summary>
        /// 获取或设置HttpMethod
        /// </summary>
        public string HttpMethod { get; set; }
        /// <summary>
        /// 获取媒体类型
        /// </summary>
        public MediaTypeHeaderValue ContentType { get; }
        /// <summary>
        /// 获取媒体类型
        /// </summary>
        public string MediaType { get; }
        /// <summary>
        /// 获取或设置Headers
        /// </summary>
        public string[] Headers { get; set; }
        /// <summary>
        /// 获取或设置Accept
        /// </summary>
        public string Accept { get; set; }
        /// <summary>
        /// 获取或设置HttpCompletionOption
        /// </summary>
        public HttpCompletionOption CompletionOption { get; set; }
        /// <summary>
        /// 获取或设置RequestContent
        /// </summary>
        public FeignClientHttpRequestContent RequestContent { get; set; }
        /// <summary>
        /// 是否特殊结果
        /// </summary>
        public bool IsSpecialResult { get; set; }

        /// <summary>
        /// 获取或设置方法元数据
        /// </summary>
        public FeignClientMethodInfo Method { get; set; }

        /// <summary>
        /// 获取或设置处理请求头
        /// </summary>
        public List<IRequestHeaderHandler> RequestHeaderHandlers { get; set; }

        /// <summary>
        /// 获取随请求一起发送的HttpContent
        /// </summary>
        /// <returns></returns>
        public HttpContent GetHttpContent(IFeignOptions options)
        {
            return RequestContent?.GetHttpContent(ContentType, options);
        }

    }
}
