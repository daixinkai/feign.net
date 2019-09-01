using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 提供基本类，用于发送 HTTP 请求和接收来自通过 URI 确认的资源的 HTTP 响应。
    /// </summary>
    public class FeignHttpClient : HttpClient
    {
        public FeignHttpClient(FeignDelegatingHandler handler) : base(handler)
        {
            Handler = handler;
        }
        /// <summary>
        /// 获取用于处理请求的Handler
        /// </summary>
        public FeignDelegatingHandler Handler { get; }

    }
}
