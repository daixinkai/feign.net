using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    /// <summary>
    /// 表示响应请求时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    public class ReceivingResponseEventArgs<TService> : FeignClientEventArgs<TService>, IReceivingResponseEventArgs<TService>
    {
        internal ReceivingResponseEventArgs(IFeignClient<TService> feignClient, FeignClientHttpRequest request, HttpResponseMessage responseMessage, Type resultType) : base(feignClient)
        {
            Request = request;
            ResponseMessage = responseMessage;
            ResultType = resultType;
        }

        /// <summary>
        /// 获取请求
        /// </summary>
        public FeignClientHttpRequest Request { get; }

        /// <summary>
        /// 获取响应信息
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; }

        /// <summary>
        /// 获取返回的类型
        /// </summary>
        public Type ResultType { get; }

        internal bool _isSetResult;

        object _result;
        /// <summary>
        /// 获取或设置返回对象
        /// </summary>
        public object Result
        {
            get
            {
                return _result;
            }
            set
            {
                _isSetResult = true;
                _result = value;
            }
        }

        internal T GetResult<T>()
        {
            return Result == null ? default(T) : (T)Result;
        }

    }
    /// <summary>
    /// 表示响应请求时提供的参数
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public sealed class ReceivingResponseEventArgs<TService, TResult> : ReceivingResponseEventArgs<TService>
    {
        internal ReceivingResponseEventArgs(IFeignClient<TService> feignClient, FeignClientHttpRequest request, HttpResponseMessage responseMessage) : base(feignClient, request, responseMessage, typeof(TResult))
        {
        }

        internal TResult GetResult()
        {
            return base.GetResult<TResult>();
        }

    }

}
