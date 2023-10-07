using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// 处理IHttpRequestFile
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FeignClientHttpFileRequestContent : FeignClientHttpRequestContent
    {
        public FeignClientHttpFileRequestContent(string name, IHttpRequestFile requestFile)
        {
            Name = name;
            RequestFile = requestFile;
        }
        public string Name { get; }
        public IHttpRequestFile RequestFile { get; }

        public override HttpContent? GetHttpContent(MediaTypeHeaderValue? contentType, IFeignOptions options)
        {
            HttpContent? httpContent = RequestFile?.GetHttpContent();
            if (httpContent != null && string.IsNullOrWhiteSpace(httpContent.Headers.ContentDisposition!.Name))
            {
                httpContent.Headers.ContentDisposition.Name = Name;
            }
            return httpContent;
        }
    }
}
