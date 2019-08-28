using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Feign
{
    /// <summary>
    /// 表示取消请求时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public sealed class CancelRequestEventArgs<TService> : FeignClientEventArgs<TService>, ICancelRequestEventArgs<TService>
    {
        internal CancelRequestEventArgs(IFeignClient<TService> feignClient, CancellationToken cancellationToken) : base(feignClient)
        {
            CancellationToken = cancellationToken;
        }
        public CancellationToken CancellationToken { get; }
    }
}
