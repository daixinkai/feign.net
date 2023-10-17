using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Feign.Pipeline
{
    /// <summary>
    /// An interface representing the sending request pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface ISendingRequestPipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        /// <summary>
        /// Gets the ResponseMessage
        /// </summary>
        FeignHttpRequestMessage RequestMessage { get; }

        /// <summary>
        /// Gets the <see cref="System.Threading.CancellationTokenSource"/>
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; }

        /// <summary>
        /// Gets a value indicating whether to terminate the request
        /// </summary>
        bool IsTerminated { get; }
        /// <summary>
        /// Terminate  request
        /// </summary>
        void Terminate();
    }
}
