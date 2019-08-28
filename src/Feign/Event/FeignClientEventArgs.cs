using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 一个接口,表示服务事件参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public abstract class FeignClientEventArgs<TService> : EventArgs, IFeignClientEventArgs<TService>
    {
        protected FeignClientEventArgs(IFeignClient<TService> feignClient)
        {
            FeignClient = feignClient;
        }
        /// <summary>
        /// 获取服务对象
        /// </summary>
        public IFeignClient<TService> FeignClient { get; }
    }

}
