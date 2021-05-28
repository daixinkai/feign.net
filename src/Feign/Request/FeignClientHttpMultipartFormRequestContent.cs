using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// 处理multipart/form-data
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FeignClientHttpMultipartFormRequestContent : FeignClientHttpRequestContent
    {
        public FeignClientHttpMultipartFormRequestContent()
        {
            _map = new Dictionary<string, object>();
        }

        Dictionary<string, object> _map;

        public void AddContent(string name, object content)
        {
            _map.Add(name, content);
        }

        public override HttpContent GetHttpContent(MediaTypeHeaderValue contentType, IFeignOptions options)
        {
            string boundary = contentType?.Parameters.FirstOrDefault(s => s.Name == "boundary")?.Value;
            if (string.IsNullOrWhiteSpace(boundary))
            {
                boundary = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N")));
            }
            MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent(boundary);
            foreach (var item in _map)
            {
                object content = item.Value;
                HttpContent httpContentPart = null;
                if (content is HttpContent)
                {
                    httpContentPart = (HttpContent)content;
                }
                else if (content is FeignClientHttpRequestContent)
                {
                    httpContentPart = ((FeignClientHttpRequestContent)content).GetHttpContent(contentType, options);
                }
                if (httpContentPart != null)
                {
                    multipartFormDataContent.Add(httpContentPart, item.Key);
                    //multipartFormDataContent.Add(httpContentPart);
                }
            }
            return multipartFormDataContent;
        }
    }
}
