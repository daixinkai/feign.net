using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Fallback
{
    /// <summary>
    /// 一个接口,表示服务降级提供者
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFallbackFactory</*out*/ T>
    {
        T GetFallback();
        void ReleaseFallback(T fallback);
    }
}
