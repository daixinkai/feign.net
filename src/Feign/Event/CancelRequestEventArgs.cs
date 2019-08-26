using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Feign
{
    public sealed class CancelRequestEventArgs<TService> : FeignClientEventArgs<TService>, ICancelRequestEventArgs<TService>
    {
        internal CancelRequestEventArgs(IFeignClient<TService> feignClient, CancellationToken cancellationToken) : base(feignClient)
        {
            CancellationToken = cancellationToken;
        }
        public CancellationToken CancellationToken { get; }
    }
}
