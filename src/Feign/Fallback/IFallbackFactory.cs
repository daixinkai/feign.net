using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Fallback
{
    /// <summary>
    /// An interface that represents the service fallback factory
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFallbackFactory</*out*/ T>
    {
        T GetFallback();
        void ReleaseFallback(T fallback);
    }
}
