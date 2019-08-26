using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Fallback
{
    public interface IFallbackFeignClient<out T> : IFeignClient<T>
    {
        T Fallback { get; }
    }
}
