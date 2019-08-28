using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 表示SendingRequest事件提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public sealed class SendingRequestEventArgs<TService> : FeignClientEventArgs<TService>, ISendingRequestEventArgs<TService>
    {
        internal SendingRequestEventArgs(IFeignClient<TService> feignClient, FeignHttpRequestMessage requestMessage) : base(feignClient)
        {
            RequestMessage = requestMessage;
        }
        /// <summary>
        /// 获取请求
        /// </summary>
        public FeignHttpRequestMessage RequestMessage { get; }
        /// <summary>
        /// 获取一个值,指示是否终止请求
        /// </summary>
        public bool IsTerminated => _isTerminated;

        bool _isTerminated;
        /// <summary>
        /// 终止请求
        /// </summary>
        public void Terminate()
        {
            _isTerminated = true;
        }

    }
}
