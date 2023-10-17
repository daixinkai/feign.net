using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign.Pipeline.Internal
{
    /// <summary>
    /// Representing the error request pipeline context
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
        /// <inheritdoc/>
        public FeignHttpRequestMessage RequestMessage { get; }
        /// <inheritdoc/>
        public Exception Exception { get; }
        /// <inheritdoc/>
        public bool ExceptionHandled { get; set; }
    }
}
