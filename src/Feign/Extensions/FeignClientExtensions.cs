using Feign.Fallback;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class FeignClientExtensions
    {
        public static IFallbackFeignClient<TService> AsFallback<TService>(this IFeignClient<TService> feignClient)
        {
            return feignClient as IFallbackFeignClient<TService>;
        }

        public static IFallbackFeignClient<TService> AsFallback<TService>(this IFeignClient<object> feignClient)
        {
            return feignClient as IFallbackFeignClient<TService>;
        }

        public static TService GetFallback<TService>(this IFeignClient<TService> feignClient) where TService : class
        {
            return feignClient.AsFallback()?.Fallback;
        }

        public static object GetFallback<TService>(this IFeignClient<object> feignClient) where TService : class
        {
            return feignClient.AsFallback()?.Fallback as TService;
        }

    }
}
