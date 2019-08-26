using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    public class FeignHttpRequestException : HttpRequestException
    {
        public FeignHttpRequestException(IFeignClient feignClient, FeignHttpRequestMessage requestMessage, HttpRequestException httpRequestException) : base(GetMessage(feignClient, requestMessage, httpRequestException))
        {
            FeignClient = feignClient;
            RequestMessage = requestMessage;
        }

        public IFeignClient FeignClient { get; }
        public FeignHttpRequestMessage RequestMessage { get; }

        static string GetMessage(IFeignClient feignClient, FeignHttpRequestMessage httpRequestMessage, HttpRequestException httpRequestException)
        {
            //string url = BuildUrl(httpRequestMessage.FeignClientRequest.BaseUrl, httpRequestMessage.FeignClientRequest.MappingUri);
            string url = httpRequestMessage.RequestUri.ToString();
            return $"{httpRequestMessage.Method.Method} request error on {url} : {httpRequestException.Message}";
        }

        static string BuildUrl(string baseUrl, string uri)
        {
            if (uri.StartsWith("/"))
            {
                return baseUrl + uri;
            }
            return baseUrl + "/" + uri;
        }

    }
}
