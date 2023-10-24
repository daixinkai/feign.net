using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Configuration
{
    public class FeignClientConfigurationContext
    {
        public FeignClientConfigurationContext(
            IFeignClient<object> feignClient,
            IFeignClientPipeline<object> pipeline,
            FeignHttpClient httpClient,
            HttpHandlerType? httpHandler)
        {
            FeignClient = feignClient;
            Pipeline = pipeline;
            HttpClient = httpClient;
            HttpHandler = httpHandler;
        }
        public IFeignClient<object> FeignClient { get; }
        public IFeignClientPipeline<object> Pipeline { get; }
        /// <summary>
        /// Gets the HttpClient
        /// </summary>
        public FeignHttpClient HttpClient { get; }
        /// <summary>
        /// Gets the Handler for HttpClient
        /// </summary>
        public HttpHandlerType? HttpHandler { get; }
    }

    public class FeignClientConfigurationContext<TService>
    {
        public FeignClientConfigurationContext(
            IFeignClient<TService> feignClient,
            IFeignClientPipeline<TService> pipeline,
            FeignHttpClient httpClient,
            HttpHandlerType? httpHandler)
        {
            FeignClient = feignClient;
            Pipeline = pipeline;
            HttpClient = httpClient;
            HttpHandler = httpHandler;
        }
        public IFeignClient<TService> FeignClient { get; set; }
        public IFeignClientPipeline<TService> Pipeline { get; set; }
        /// <summary>
        /// Gets the HttpClient
        /// </summary>
        public FeignHttpClient HttpClient { get; }
        /// <summary>
        /// Gets the Handler for HttpClient
        /// </summary>
        public HttpHandlerType? HttpHandler { get; }
    }
}
