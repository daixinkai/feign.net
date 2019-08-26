using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    public interface IReceivingResponseEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        HttpResponseMessage ResponseMessage { get; }

        Type ResultType { get; }

        object Result { get; set; }

    }
}
