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
    public interface IInitializingEventArgs<out TService> : IFeignClientEventArgs<TService>
    {
        HttpClient HttpClient { get; set; }
    }
}
