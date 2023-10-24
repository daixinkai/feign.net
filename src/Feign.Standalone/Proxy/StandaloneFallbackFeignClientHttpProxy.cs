using Feign.Proxy;

namespace Feign.Standalone.Proxy
{
    public abstract class StandaloneFallbackFeignClientHttpProxy<TService, TFallback> : FallbackFeignClientHttpProxy<TService, TFallback> where TService : class
      where TFallback : TService
    {
        public StandaloneFallbackFeignClientHttpProxy() : base(FeignClients.Get<TFallback>(), FeignClients.CreateFeignClientConfigureOptions<TService>())
        {
        }

    }

}
