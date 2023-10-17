using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// An interface representing the disposing pipeline context
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IDisposingPipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        bool Disposing { get; }
    }
}
