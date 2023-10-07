using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign.Pipeline.Internal
{
    /// <summary>
    /// 表示发生错误时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
//#if NET5_0_OR_GREATER
//    internal record ErrorRequestPipelineContext<TService> : FeignClientPipelineContext<TService>, IErrorRequestPipelineContext<TService>
//#else
    internal class ErrorRequestPipelineContext<TService> : FeignClientPipelineContext<TService>, IErrorRequestPipelineContext<TService>
//#endif
    {
        internal ErrorRequestPipelineContext(IFeignClient<TService> feignClient, FeignHttpRequestMessage requestMessage, Exception exception) : base(feignClient)
        {
            RequestMessage = requestMessage;
            Exception = exception;
        }
        /// <summary>
        /// 获取请求的RequestMessage
        /// </summary>
        public FeignHttpRequestMessage RequestMessage { get; }
        /// <summary>
        /// 获取引发的错误
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        /// 获取或设置一个值,表示此错误是否已经处理
        /// </summary>
        public bool ExceptionHandled { get; set; }
    }
}
