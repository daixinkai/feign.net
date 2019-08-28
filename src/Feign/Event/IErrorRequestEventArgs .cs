using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 一个接口,表示发生错误时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IErrorRequestEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        /// <summary>
        /// 获取请求的RequestMessage
        /// </summary>
        FeignHttpRequestMessage RequestMessage { get; }
        /// <summary>
        /// 获取引发的错误
        /// </summary>
        Exception Exception { get; }
        /// <summary>
        /// 获取或设置一个值,表示此错误是否已经处理
        /// </summary>
        bool ExceptionHandled { get; set; }
    }
}
