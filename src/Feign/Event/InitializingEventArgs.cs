using Feign.Cache;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public class InitializingEventArgs<TService> : FeignClientEventArgs<TService>, IInitializingEventArgs<TService>
    {
        public InitializingEventArgs(IFeignClient<TService> feignClient) : base(feignClient)
        {
        }

        public HttpClient HttpClient { get; set; }

    }
}
