using Feign.Cache;
using Feign.Logging;
using System;
using System.Reflection;

namespace Feign.Standalone
{
    public static class FeignClients
    {
        static FeignClients()
        {
            _standaloneFeignBuilder = new StandaloneFeignBuilder();
        }
        internal static readonly StandaloneFeignBuilder _standaloneFeignBuilder;

        public static IStandaloneFeignBuilder StandaloneFeignBuilder { get { return _standaloneFeignBuilder; } }

        public static TService Get<TService>()
        {
            return _standaloneFeignBuilder.GetService<TService>();
        }

        public static IStandaloneFeignBuilder AddFeignClients()
        {
            return AddFeignClients((FeignOptions)null);
        }

        public static IStandaloneFeignBuilder AddFeignClients(Action<FeignOptions> setupAction)
        {
            FeignOptions options = new FeignOptions();
            setupAction?.Invoke(options);
            return AddFeignClients(options);
        }

        public static IStandaloneFeignBuilder AddFeignClients(FeignOptions options)
        {
            if (options == null)
            {
                options = new FeignOptions();
            }

            if (options.Lifetime == FeignClientLifetime.Scoped)
            {
                throw new NotSupportedException(nameof(options.Lifetime) + " can not be FeignClientLifetime.Scoped!");
            }

            StandaloneFeignBuilder feignBuilder = _standaloneFeignBuilder;

            feignBuilder.Options = options;
            if (options.Assemblies.Count == 0)
            {
                feignBuilder.AddFeignClients(Assembly.GetEntryAssembly(), options.Lifetime);
            }
            else
            {
                foreach (var assembly in options.Assemblies)
                {
                    feignBuilder.AddFeignClients(assembly, options.Lifetime);
                }
            }
            feignBuilder.AddService(typeof(ILoggerFactory), typeof(DefaultLoggerFactory), FeignClientLifetime.Singleton);
            feignBuilder.AddService(typeof(ICacheProvider), typeof(DefaultCacheProvider), FeignClientLifetime.Singleton);
            feignBuilder.AddService<IFeignOptions>(options);
            return feignBuilder;
        }


    }
}
