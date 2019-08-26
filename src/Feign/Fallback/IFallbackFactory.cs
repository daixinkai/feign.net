using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Fallback
{
    public interface IFallbackFactory</*out*/ T>
    {
        T GetFallback();
        void ReleaseFallback(T fallback);
    }
}
