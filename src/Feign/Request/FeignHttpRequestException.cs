using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// An error occurred during the request
    /// </summary>
    public class FeignHttpRequestException : HttpRequestException
    {
        public FeignHttpRequestException(IFeignClient feignClient, FeignHttpRequestMessage requestMessage, Exception exception) : base(GetMessage(requestMessage, exception), exception)
        {
            FeignClient = feignClient;
            RequestMessage = requestMessage;
        }
        /// <summary>
        /// Gets the FeignClient
        /// </summary>
        public IFeignClient FeignClient { get; }
        /// <summary>
        /// Gets the RequestMessage
        /// </summary>
        public FeignHttpRequestMessage RequestMessage { get; }

        private static string GetMessage(FeignHttpRequestMessage httpRequestMessage, Exception exception)
        {            
            //string url = BuildUrl(httpRequestMessage.FeignClientRequest.BaseUrl, httpRequestMessage.FeignClientRequest.MappingUri);
            string url = httpRequestMessage.RequestUri!.ToString();
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
