using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Feign
{
    /// <summary>
    /// 表示SendingRequest事件提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public sealed class SendingRequestEventArgs<TService> : FeignClientEventArgs<TService>, ISendingRequestEventArgs<TService>
    {
        internal SendingRequestEventArgs(IFeignClient<TService> feignClient, FeignHttpRequestMessage requestMessage, CancellationToken cancellationToken) : base(feignClient)
        {
            RequestMessage = requestMessage;
            _cancellationToken = cancellationToken;
        }
        /// <summary>
        /// 获取请求
        /// </summary>
        public FeignHttpRequestMessage RequestMessage { get; }



        /// <summary>
        /// 获取一个值,指示是否终止请求
        /// </summary>
        public bool IsTerminated => _isTerminated;

        public CancellationTokenSource CancellationTokenSource
        {
            get
            {
                if (_cancellationTokenSource == null)
                {
                    _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken);
                }
                return _cancellationTokenSource;
            }
        }

        internal CancellationTokenSource _cancellationTokenSource;

        CancellationToken _cancellationToken;

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
