using Feign.Cache;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// An interface representing the initializing pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IInitializingPipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        /// <summary>
        /// Gets the HttpClient
        /// </summary>
        FeignHttpClient HttpClient { get; set; }
        /// <summary>
        /// Gets the Handler for HttpClient
        /// </summary>
        HttpHandlerType? HttpHandler { get; }
    }
}
