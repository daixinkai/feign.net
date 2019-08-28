using Feign.Cache;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 表示初始化时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public sealed class InitializingEventArgs<TService> : FeignClientEventArgs<TService>, IInitializingEventArgs<TService>
    {
        internal InitializingEventArgs(IFeignClient<TService> feignClient) : base(feignClient)
        {
        }
        /// <summary>
        /// 获取HttpClient
        /// </summary>
        public HttpClient HttpClient { get; set; }

    }
}
