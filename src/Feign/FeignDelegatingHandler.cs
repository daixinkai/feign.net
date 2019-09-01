using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    ///  一种典型的 HTTP 处理程序委托给另一个处理程序，HTTP 响应消息的处理称为内部处理程序。
    /// </summary>
    public class FeignDelegatingHandler : DelegatingHandler
    {
        public FeignDelegatingHandler() : base() { }
        public FeignDelegatingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }
    }
}
