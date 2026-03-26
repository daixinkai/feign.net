using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Internal
{
    /// <summary>
    ///  A type for HTTP handlers that delegate the processing of HTTP response messages to another handler, called the inner handler.
    /// </summary>
    internal class FeignDelegatingHandler : DelegatingHandler
    {
        public FeignDelegatingHandler() : base() { }
        public FeignDelegatingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }
    }
}
