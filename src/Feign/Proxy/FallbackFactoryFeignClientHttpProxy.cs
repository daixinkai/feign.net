using Feign.Fallback;

namespace Feign.Proxy
{
    /// <summary>
    /// HttpProxy with service fallback factory support
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TFallbackFactory"></typeparam>
    public abstract class FallbackFactoryFeignClientHttpProxy<TService, TFallbackFactory> : FallbackFeignClientHttpProxy<TService, TService>, IFallbackFactoryFeignClient<TService>, IFeignClient<TService> where TService : class
        where TFallbackFactory : IFallbackFactory<TService>
    {
        public FallbackFactoryFeignClientHttpProxy(
            TFallbackFactory fallbackFactory,
            FeignClientHttpProxyOptions<TService> options
            ) : base(GetFallback(fallbackFactory), options)
        {
            FallbackFactory = fallbackFactory;
        }

        public TFallbackFactory FallbackFactory { get; }

        IFallbackFactory<TService> IFallbackFactoryFeignClient<TService>.FallbackFactory
        {
            get
            {
                return FallbackFactory;
            }
        }


        private static TService GetFallback(IFallbackFactory<TService> fallbackFactory)
        {
            //if (fallbackFactory != null)
            //{
            return fallbackFactory.GetFallback();
            //}
            //return default;
        }

        protected override void Dispose(bool disposing)
        {
            if (FallbackFactory != null && Fallback != null)
            {
                FallbackFactory.ReleaseFallback(Fallback);
            }
            base.Dispose(disposing);
        }

    }

}
