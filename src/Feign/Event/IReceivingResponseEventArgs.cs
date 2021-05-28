using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 一个接口,表示响应请求时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public interface IReceivingResponseEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        /// <summary>
        /// 获取请求
        /// </summary>
        FeignClientHttpRequest Request { get; }
        /// <summary>
        /// 获取响应信息
        /// </summary>
        HttpResponseMessage ResponseMessage { get; }
        /// <summary>
        /// 获取返回的类型
        /// </summary>
        Type ResultType { get; }
        /// <summary>
        /// 获取或设置返回对象
        /// </summary>
        object Result { get; set; }

    }
}
