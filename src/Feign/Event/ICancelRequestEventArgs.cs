using Feign.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Feign
{
    /// <summary>
    /// 一个接口,表示取消请求时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface ICancelRequestEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        /// <summary>
        /// 获取CancellationToken
        /// </summary>
        CancellationToken CancellationToken { get; }
        /// <summary>
        /// 获取请求
        /// </summary>
        FeignHttpRequestMessage RequestMessage { get; }
    }
}
