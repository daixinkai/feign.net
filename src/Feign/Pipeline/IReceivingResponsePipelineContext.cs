using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign.Pipeline
{
    /// <summary>
    /// An interface representing the receiving response pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IReceivingResponsePipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        /// <summary>
        /// Gets the Request
        /// </summary>
        FeignClientHttpRequest Request { get; }
        /// <summary>
        /// Gets the ResponseMessage
        /// </summary>
        HttpResponseMessage ResponseMessage { get; }
        /// <summary>
        /// Gets the ResultType
        /// </summary>
        Type ResultType { get; }
        /// <summary>
        /// Gets or sets the Result
        /// </summary>
        object? Result { get; set; }

    }
}
