using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign.Pipeline
{
    /// <summary>
    /// An interface representing the error request pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IErrorRequestPipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        /// <summary>
        /// Gets the RequestMessage
        /// </summary>
        FeignHttpRequestMessage RequestMessage { get; }
        /// <summary>
        /// Gets the Exception
        /// </summary>
        Exception Exception { get; }
        /// <summary>
        /// Gets or sets a value indicating whether this error has been handled
        /// </summary>
        bool ExceptionHandled { get; set; }
    }
}
