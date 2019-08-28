using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 表示Disposing事件提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public sealed class DisposingEventArgs<TService> : FeignClientEventArgs<TService>, IDisposingEventArgs<TService>
    {
        internal DisposingEventArgs(IFeignClient<TService> feignClient, bool disposing) : base(feignClient)
        {
            Disposing = disposing;
        }
        public bool Disposing { get; }
    }
}
