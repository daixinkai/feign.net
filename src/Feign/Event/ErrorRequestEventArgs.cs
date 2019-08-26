using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
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
        public FeignHttpRequestMessage RequestMessage { get; }
        public Exception Exception { get; }
        public bool ExceptionHandled { get; set; }
    }
}
