using Feign.Middleware;
using Feign.Pipeline;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Polly
{
    internal class PollyInitializingMiddleware<TService> : IInitializingMiddleware<TService>
    {

        public PollyInitializingMiddleware(FeignPollyOptions options)
        {
            _options = options;
        }

        private readonly FeignPollyOptions _options;

        public void Invoke(IInitializingPipelineContext<TService> context)
        {
            IAsyncPolicy asyncPolicy = _options.GetAsyncPolicy(context.FeignClient.ServiceId, context.FeignClient.ServiceType);
            PollyDelegatingHandler pollyDelegatingHandler = new PollyDelegatingHandler(asyncPolicy, context.HttpClient.Handler);
            context.HttpClient.Handler = pollyDelegatingHandler;
        }
    }
}
