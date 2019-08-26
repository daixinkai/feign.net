using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Feign
{
    public interface IFeignClient
    {
        /// <summary>
        /// Gets the serviceId
        /// </summary>
        string ServiceId { get; }
    }

    public interface IFeignClient<out TService> : IFeignClient
    {
        TService Service { get; }
    }
}
