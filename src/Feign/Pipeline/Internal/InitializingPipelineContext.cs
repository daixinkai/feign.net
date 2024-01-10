using Feign.Cache;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
    /// <summary>
    /// Representing the initializing pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    //#if NET5_0_OR_GREATER
    //    internal record InitializingPipelineContext<TService> : FeignClientPipelineContext<TService>, IInitializingPipelineContext<TService>
    //#else
    internal class InitializingPipelineContext<TService> : FeignClientPipelineContext<TService>, IInitializingPipelineContext<TService>
    //#endif
    {
        internal InitializingPipelineContext(IFeignClient<TService> feignClient, FeignHttpClient httpClient, HttpHandlerType httpHandler) : base(feignClient)
        {
            HttpClient = httpClient;
            HttpHandler = httpHandler;
        }
        /// <inheritdoc/>
        public FeignHttpClient HttpClient { get; set; }

        public HttpHandlerType HttpHandler { get; }

    }
}
