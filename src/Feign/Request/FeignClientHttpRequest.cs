using Feign.Internal;
using Feign.Reflection;
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
        public FeignClientHttpRequest(string baseUrl, string mappingUri, string uri, string httpMethod, string contentType, FeignClientHttpRequestContent requestContent, FeignClientMethodInfo method)
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
        /// 获取HttpMethod
        /// </summary>
        public string HttpMethod { get; }
        /// <summary>
        /// 获取媒体类型
        /// </summary>
        public MediaTypeHeaderValue ContentType { get; }
        /// <summary>
        /// 获取媒体类型
        /// </summary>
        public string MediaType { get; }
        /// <summary>
        /// 获取RequestContent
        /// </summary>
        public FeignClientHttpRequestContent RequestContent { get; }
        ///// <summary>
        ///// 获取方法元数据
        ///// </summary>
        //public MethodInfo Method { get; }

        public FeignClientMethodInfo Method { get; set; }

        /// <summary>
        /// 获取随请求一起发送的HttpContent
        /// </summary>
        /// <returns></returns>
        public HttpContent GetHttpContent()
        {
            return RequestContent?.GetHttpContent(ContentType);
        }

    }
}
