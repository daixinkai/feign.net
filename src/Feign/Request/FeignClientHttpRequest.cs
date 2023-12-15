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
    /// A FeignClientHttp request
    /// </summary>
    public class FeignClientHttpRequest
    {
        public FeignClientHttpRequest(string baseUrl, string? mappingUri, string uri, string httpMethod, string? contentType)
        {
            BaseUrl = baseUrl;
            MappingUri = mappingUri;
            Uri = uri;
            HttpMethod = httpMethod;
            if (!string.IsNullOrWhiteSpace(contentType))
            {
                if (!MediaTypeHeaderValue.TryParse(contentType, out var mediaTypeHeaderValue))
                {
                    throw new ArgumentException("ContentType error");
                }
                MediaType = mediaTypeHeaderValue.MediaType;
                ContentType = mediaTypeHeaderValue;
            }
        }
        /// <summary>
        /// Gets the BaseUrl
        /// </summary>
        public string BaseUrl { get; }
        /// <summary>
        /// Gets the MappingUri
        /// </summary>
        public string? MappingUri { get; }
        /// <summary>
        /// Gets the real Uri
        /// </summary>
        public string Uri { get; }
        /// <summary>
        /// Gets or sets the HttpMethod
        /// </summary>
        public string HttpMethod { get; set; }
        /// <summary>
        /// Gets the ContentType
        /// </summary>
        public MediaTypeHeaderValue? ContentType { get; }
        /// <summary>
        /// Gets the MediaType
        /// </summary>
        public string? MediaType { get; }
        /// <summary>
        /// Gets or sets the Headers
        /// </summary>
        public string[]? Headers { get; set; }
        /// <summary>
        /// Gets or sets the Accept
        /// </summary>
        public string? Accept { get; set; }
        /// <summary>
        /// Gets or sets the HttpCompletionOption
        /// </summary>
        public HttpCompletionOption CompletionOption { get; set; }
        /// <summary>
        /// whether 404s should be decoded instead of throwing FeignExceptions
        /// </summary>
        public bool Dismiss404 { get; set; }
        /// <summary>
        /// Gets or sets the RequestContent
        /// </summary>
        public FeignClientHttpRequestContent? RequestContent { get; set; }
        /// <summary>
        /// Is it a special result?
        /// </summary>
        public bool IsSpecialResult { get; set; }

        /// <summary>
        /// Gets or sets the Method metadata
        /// </summary>
        public FeignClientMethodInfo? Method { get; set; }

        /// <summary>
        /// Gets or sets the RequestHeaderHandlers
        /// </summary>
        public List<IRequestHeaderHandler>? RequestHeaderHandlers { get; set; }

        /// <summary>
        /// Gets the HttpContent sent with the request
        /// </summary>
        /// <returns></returns>
        public HttpContent? GetHttpContent(IFeignOptions options)
        {
            return RequestContent?.GetHttpContent(ContentType, options);
        }

    }
}
