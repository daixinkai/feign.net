using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline
{
    /// <summary>
    /// 一个接口,表示Disposing事件提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IDisposingPipelineContext<out TService> : IFeignClientPipelineContext<TService>
    {
        bool Disposing { get; }
    }
}
