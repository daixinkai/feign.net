using Feign.Cache;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#if NETCOREAPP2_1_OR_GREATER
using HttpHandlerType = System.Net.Http.SocketsHttpHandler;
#else
using HttpHandlerType = System.Net.Http.HttpClientHandler;
#endif

namespace Feign.Pipeline
{
    /// <summary>
    /// 一个接口,表示初始化时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IInitializingPipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        /// <summary>
        /// 获取HttpClient
        /// </summary>
        FeignHttpClient HttpClient { get; set; }
        HttpHandlerType HttpHandler { get; }
    }
}
