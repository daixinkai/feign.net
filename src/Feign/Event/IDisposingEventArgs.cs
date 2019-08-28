using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 一个接口,表示Disposing事件提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IDisposingEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        bool Disposing { get; }
    }
}
