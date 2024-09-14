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
        public static IFallbackFeignClient<TService>? AsFallback<TService>(this IFeignClient<TService> feignClient)
        {
            return feignClient as IFallbackFeignClient<TService>;
        }

        public static IFallbackFeignClient<TService>? AsFallback<TService>(this IFeignClient<object> feignClient)
        {
            return feignClient as IFallbackFeignClient<TService>;
        }

        public static TService? GetFallback<TService>(this IFeignClient<TService> feignClient) where TService : class
        {
            return feignClient.AsFallback()?.Fallback;
        }

        public static object? GetFallback(this IFeignClient<object> feignClient)
        {
            return feignClient.AsFallback()?.Fallback;
        }

        public static TService? GetFallback<TService>(this IFeignClient<object> feignClient) where TService : class
        {
            return feignClient.AsFallback()?.Fallback as TService;
        }

        public static void SetFeature<T>(this IFeignClient<object> feignClient, T? instance) where T : class
        {
            feignClient.Features[typeof(T)] = instance;
        }

        public static T? GetFeature<T>(this IFeignClient<object> feignClient) where T : class
        {
            feignClient.Features.TryGetValue(typeof(T), out var instance);
            return instance as T;
        }

    }
}
