using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    public abstract class FeignClientEventArgs<TService> : EventArgs, IFeignClientEventArgs<TService>
    {
        protected FeignClientEventArgs(IFeignClient<TService> feignClient)
        {
            FeignClient = feignClient;
        }
        public IFeignClient<TService> FeignClient { get; }
    }

}
