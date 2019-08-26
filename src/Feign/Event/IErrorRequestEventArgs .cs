using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    public interface IErrorRequestEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        FeignHttpRequestMessage RequestMessage { get; }
        Exception Exception { get; }
        bool ExceptionHandled { get; set; }
    }
}
