using Feign.Fallback;
using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// An interface representing the fallback request pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IFallbackRequestPipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        /// <summary>
        /// Gets the Request
        /// </summary>
        FeignClientHttpRequest Request { get; }
        /// <summary>
        /// Gets the FallbackProxy
        /// </summary>
        IFallbackProxy? FallbackProxy { get; }
        /// <summary>
        /// Gets the Fallback object
        /// </summary>
        TService Fallback { get; }
        /// <summary>
        /// Gets the Fallback Method
        /// </summary>
        MethodInfo Method { get; }
        /// <summary>
        /// Gets the error that triggered the fallback
        /// </summary>
        Exception Exception { get; }
        /// <summary>
        /// Gets the Parameters
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetParameters();
        /// <summary>
        /// Gets the parameter type of the request
        /// </summary>
        /// <returns></returns>
        Type[] GetParameterTypes();
        /// <summary>
        /// Gets a value indicating whether to terminate the fallback
        /// </summary>
        bool IsTerminated { get; }
        /// <summary>
        /// Terminate fallback
        /// </summary>
        void Terminate();

    }
}
