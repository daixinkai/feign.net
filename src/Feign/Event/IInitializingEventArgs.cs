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
    /// 一个接口,表示初始化时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IInitializingEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        /// <summary>
        /// 获取HttpClient
        /// </summary>
        FeignHttpClient HttpClient { get; set; }
    }
}
