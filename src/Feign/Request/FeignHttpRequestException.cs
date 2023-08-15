using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// 请求过程中发生的错误
    /// </summary>
    public class FeignHttpRequestException : HttpRequestException
    {
        public FeignHttpRequestException(IFeignClient feignClient, FeignHttpRequestMessage requestMessage, Exception exception) : base(GetMessage(requestMessage, exception), exception)
        {
            FeignClient = feignClient;
            RequestMessage = requestMessage;
        }
        /// <summary>
        /// 获取服务对象
        /// </summary>
        public IFeignClient FeignClient { get; }
        /// <summary>
        /// 获取请求信息
        /// </summary>
        public FeignHttpRequestMessage RequestMessage { get; }

        private static string GetMessage(FeignHttpRequestMessage httpRequestMessage, Exception exception)
        {            
            //string url = BuildUrl(httpRequestMessage.FeignClientRequest.BaseUrl, httpRequestMessage.FeignClientRequest.MappingUri);
            string url = httpRequestMessage.RequestUri.ToString();
            return $"{httpRequestMessage.Method.Method} request error on {url} : {exception.Message}";
        }

        //private static string BuildUrl(string baseUrl, string uri)
        //{
        //    if (uri.StartsWith("/"))
        //    {
        //        return baseUrl + uri;
        //    }
        //    return baseUrl + "/" + uri;
        //}

    }
}
