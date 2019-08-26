using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feign
{
    public interface ICancelRequestEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        CancellationToken CancellationToken { get; }
    }
}
