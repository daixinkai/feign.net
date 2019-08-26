using Feign.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    public interface ISendingRequestEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        FeignHttpRequestMessage RequestMessage { get; }
        bool IsTerminated { get; }
        void Terminate();
    }
}
