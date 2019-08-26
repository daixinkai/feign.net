using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    public class ReceivingResponseEventArgs<TService> : FeignClientEventArgs<TService>, IReceivingResponseEventArgs<TService>
    {
        internal ReceivingResponseEventArgs(IFeignClient<TService> feignClient, HttpResponseMessage responseMessage, Type resultType) : base(feignClient)
        {
            ResponseMessage = responseMessage;
            ResultType = resultType;
        }
        public HttpResponseMessage ResponseMessage { get; }

        public Type ResultType { get; }

        internal bool _isSetResult;

        object _result;

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

    public sealed class ReceivingResponseEventArgs<TService, TResult> : ReceivingResponseEventArgs<TService>
    {
        internal ReceivingResponseEventArgs(IFeignClient<TService> feignClient, HttpResponseMessage responseMessage) : base(feignClient, responseMessage, typeof(TResult))
        {
        }
    }

}
