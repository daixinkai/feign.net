using Feign.Request;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Feign.Pipeline.Internal
{
    /// <summary>
    /// 表示取消请求时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
#if NET5_0_OR_GREATER
    internal record CancelRequestPipelineContext<TService> : FeignClientPipelineContext<TService>, ICancelRequestPipelineContext<TService>
#else
    internal class CancelRequestPipelineContext<TService> : FeignClientPipelineContext<TService>, ICancelRequestPipelineContext<TService>
#endif
    {
        internal CancelRequestPipelineContext(IFeignClient<TService> feignClient, FeignHttpRequestMessage requestMessage, CancellationToken cancellationToken) : base(feignClient)
        {
            RequestMessage = requestMessage;
            CancellationToken = cancellationToken;
        }
        public CancellationToken CancellationToken { get; }

        public FeignHttpRequestMessage RequestMessage { get; }
    }
}
