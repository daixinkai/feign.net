using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public class DisposingEventArgs<TService> : FeignClientEventArgs<TService>, IDisposingEventArgs<TService>
    {
        public DisposingEventArgs(IFeignClient<TService> feignClient, bool disposing) : base(feignClient)
        {
            Disposing = disposing;
        }
        public bool Disposing { get; }
    }
}
