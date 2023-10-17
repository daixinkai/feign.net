using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Request
{
    /// <summary>
    /// Support content type conversion HttpContent
    /// </summary>
    public abstract class FeignClientHttpRequestContent
    {
        /// <summary>
        /// Gets the corresponding HttpContent according to the specified content type
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract HttpContent? GetHttpContent(MediaTypeHeaderValue? contentType, IFeignOptions options);
    }
}
