using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// An interface representing the cancel request pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface ICancelRequestPipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        /// <summary>
        /// Gets the CancellationToken
        /// </summary>
        CancellationToken CancellationToken { get; }
        /// <summary>
        /// Gets the RequestMessage
        /// </summary>
        FeignHttpRequestMessage RequestMessage { get; }
    }
}
