using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 表示发生错误时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public sealed class ErrorRequestEventArgs<TService> : FeignClientEventArgs<TService>, IErrorRequestEventArgs<TService>
    {
        internal ErrorRequestEventArgs(IFeignClient<TService> feignClient, Exception exception) : base(feignClient)
        {
            Exception = exception;
            if (exception is FeignHttpRequestException)
            {
                RequestMessage = ((FeignHttpRequestException)exception).RequestMessage;
            }
        }
        internal ErrorRequestEventArgs(IFeignClient<TService> feignClient, FeignHttpRequestMessage requestMessage, Exception exception) : base(feignClient)
        {
            RequestMessage = requestMessage;
            Exception = exception;
        }
        /// <summary>
        /// 获取请求的RequestMessage
        /// </summary>
        public FeignHttpRequestMessage RequestMessage { get; }
        /// <summary>
        /// 获取引发的错误
        /// </summary>
        public Exception Exception { get; }
        /// <summary>
        /// 获取或设置一个值,表示此错误是否已经处理
        /// </summary>
        public bool ExceptionHandled { get; set; }
    }
}
