using Feign.Request;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feign.Polly
{
    /// <summary>
    /// 支持Polly的DelegatingHandler
    /// </summary>
    public class PollyDelegatingHandler : DelegatingHandler
    {
        public PollyDelegatingHandler(IAsyncPolicy asyncPolicy) : base()
        {
            _asyncPolicy = asyncPolicy;
        }
        public PollyDelegatingHandler(IAsyncPolicy asyncPolicy, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            _asyncPolicy = asyncPolicy;
        }

        IAsyncPolicy _asyncPolicy;

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _asyncPolicy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
        }
    }
}
