using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// Provides a base class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI.
    /// </summary>
    public class FeignHttpClient : HttpClient
    {
        public FeignHttpClient(FeignDelegatingHandler handler) : base(handler)
        {
            Handler = handler;
        }
        /// <summary>
        /// Gets or sets the inner handler which processes the HTTP response messages.
        /// </summary>
        public FeignDelegatingHandler Handler { get; }

    }
}
