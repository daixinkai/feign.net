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
    class PollyFeignHttpClient : FeignHttpClient
    {
        public PollyFeignHttpClient(FeignHttpClient feignHttpClient, IAsyncPolicy asyncPolicy) : base(feignHttpClient.Handler)
        {
        }

        readonly IAsyncPolicy _asyncPolicy;

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return _asyncPolicy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
        }

    }
}
