using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// FeignHttpRequestMessage
    /// </summary>
    public class FeignHttpRequestMessage : HttpRequestMessage
    {
        public FeignHttpRequestMessage(FeignClientHttpRequest feignClientRequest)
        {
            FeignClientRequest = feignClientRequest;
        }

        public FeignHttpRequestMessage(FeignClientHttpRequest feignClientRequest, HttpMethod method, string? requestUri) : base(method, requestUri)
        {
            FeignClientRequest = feignClientRequest;
        }

        public FeignHttpRequestMessage(FeignClientHttpRequest feignClientRequest, HttpMethod method, Uri? requestUri) : base(method, requestUri)
        {
            FeignClientRequest = feignClientRequest;
        }
        /// <summary>
        /// Gets the FeignClientRequest
        /// </summary>
        public FeignClientHttpRequest FeignClientRequest { get; }

    }
}
