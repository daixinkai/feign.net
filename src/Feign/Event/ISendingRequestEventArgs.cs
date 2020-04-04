using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Feign
{
    /// <summary>
    /// 一个接口,表示SendingRequest事件提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface ISendingRequestEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        /// <summary>
        /// 获取请求
        /// </summary>
        FeignHttpRequestMessage RequestMessage { get; }

        /// <summary>
        /// 获取 <see cref="System.Threading.CancellationTokenSource"/>
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// 获取一个值,指示是否终止请求
        /// </summary>
        bool IsTerminated { get; }
        /// <summary>
        /// 终止请求
        /// </summary>
        void Terminate();
    }
}
