using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Pipeline.Internal
{
    /// <summary>
    /// 表示Disposing事件提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
//#if NET5_0_OR_GREATER
//    internal record DisposingPipelineContext<TService> : FeignClientPipelineContext<TService>, IDisposingPipelineContext<TService>
//#else
    internal class DisposingPipelineContext<TService> : FeignClientPipelineContext<TService>, IDisposingPipelineContext<TService>
//#endif
    {
        internal DisposingPipelineContext(IFeignClient<TService> feignClient, bool disposing) : base(feignClient)
        {
            Disposing = disposing;
        }
        public bool Disposing { get; }
    }
}
