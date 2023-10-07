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
    /// 支持媒体类型转换HttpContent
    /// </summary>
    public abstract class FeignClientHttpRequestContent
    {
        /// <summary>
        /// 根据指定的媒体类型获取对应的HttpContent
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract HttpContent? GetHttpContent(MediaTypeHeaderValue? contentType, IFeignOptions options);
    }
}
