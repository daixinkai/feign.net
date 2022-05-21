using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Feign.Pipeline.Internal
{
    /// <summary>
    /// 表示SendingRequest事件提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
#if NET5_0_OR_GREATER
    internal record SendingRequestPipelineContext<TService> : FeignClientPipelineContext<TService>, ISendingRequestPipelineContext<TService>
#else
    internal class SendingRequestPipelineContext<TService> : FeignClientPipelineContext<TService>, ISendingRequestPipelineContext<TService>
#endif        
    {
        internal SendingRequestPipelineContext(IFeignClient<TService> feignClient, FeignHttpRequestMessage requestMessage, CancellationToken cancellationToken) : base(feignClient)
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

        private CancellationToken _cancellationToken;

        private bool _isTerminated;
        /// <summary>
        /// 终止请求
        /// </summary>
        public void Terminate()
        {
            _isTerminated = true;
        }

    }
}
