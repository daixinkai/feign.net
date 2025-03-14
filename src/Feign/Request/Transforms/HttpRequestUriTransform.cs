using Feign.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request.Transforms
{
    public sealed class HttpRequestUriTransform : IHttpRequestTransform
    {
        public HttpRequestUriTransform(string? requestUri)
        {
            RequestUri = requestUri;
        }

        public string? RequestUri { get; }

        public ValueTask ApplyAsync(FeignHttpRequestMessage request)
        {
            if (!string.IsNullOrWhiteSpace(RequestUri))
            {
                if (Uri.TryCreate(RequestUri, UriKind.RelativeOrAbsolute, out var uri))
                {
                    if (uri.IsAbsoluteUri)
                    {
                        if (request.RequestUri != null)
                        {
                            throw new NotSupportedException("RequestMapping Url is not null,but RequestUri is absolute uri");
                        }
                        request.RequestUri = uri;
                    }
                    else
                    {
                        if (request.RequestUri == null)
                        {
                            throw new NotSupportedException("RequestMapping Url is null,but RequestUri is relative uri");
                        }
                        string url = $"{request.RequestUri.Scheme}://{request.RequestUri.Authority}{request.RequestUri.AbsolutePath}";
                        url = url.TrimEnd('/') + "/" + uri.OriginalString.TrimStart('/') + request.RequestUri.Query;
                        request.RequestUri = new Uri(url);
                    }
                }
            }
            return TaskEx.CompletedValueTask;
        }
    }
}
