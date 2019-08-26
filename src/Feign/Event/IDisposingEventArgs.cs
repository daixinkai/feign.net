using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public interface IDisposingEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        bool Disposing { get; }
    }
}
