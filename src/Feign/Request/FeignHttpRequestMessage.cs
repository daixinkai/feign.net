using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{

    //public class FeignHttpRequestMessage<TService> : FeignHttpRequestMessage
    //{
    //    public FeignHttpRequestMessage(FeignClientHttpRequest feignClientRequest) : base(feignClientRequest)
    //    {
    //    }

    //    public FeignHttpRequestMessage(FeignClientHttpRequest feignClientRequest, HttpMethod method, string? requestUri) : base(feignClientRequest, method, requestUri)
    //    {
    //    }

    //    public FeignHttpRequestMessage(FeignClientHttpRequest feignClientRequest, HttpMethod method, Uri? requestUri) : base(feignClientRequest, method, requestUri)
    //    {
    //    }
    //}

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

        /// <summary>
        /// Gets the ResponseMessage
        /// </summary>
        public HttpResponseMessage? ResponseMessage { get; internal set; }
        /// <summary>
        /// Gets or sets the ServiceId
        /// </summary>
        public string? ServiceId { get; set; }

        private IDictionary<string, object?>? _items;
        public IDictionary<string, object?> Items => _items ??= FeignClientUtils.CreateDictionary<string>();

    }
}
