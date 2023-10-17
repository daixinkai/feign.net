using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// An interface representing the building request pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IBuildingRequestPipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        /// <summary>
        /// Gets the http method
        /// </summary>
        string Method { get; }
        /// <summary>
        /// Gets or sets the RequestUri
        /// </summary>
        Uri? RequestUri { get; set; }
        /// <summary>
        /// Gets the headers
        /// </summary>
        IDictionary<string, string> Headers { get; }
        /// <summary>
        /// Gets the request
        /// </summary>
        FeignClientHttpRequest Request { get; }
    }
}
