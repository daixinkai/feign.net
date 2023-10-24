using Feign.Cache;
using Feign.Configuration;
using Feign.Discovery;
using Feign.Logging;
using Feign.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Standalone
{
    public static class FeignClients
    {
        static FeignClients()
        {
            s_standaloneFeignBuilder = new StandaloneFeignBuilder();
        }


        private static readonly StandaloneFeignBuilder s_standaloneFeignBuilder;

        internal static FeignClientConfigureOptions<TService> CreateFeignClientConfigureOptions<TService>()
        {
            return new FeignClientConfigureOptions<TService>(Get<IFeignOptions>(), Get<IServiceDiscovery>(), Get<ICacheProvider>(), Get<ILoggerFactory>(), Get<IFeignClientConfiguration>(), Get<IFeignClientConfiguration<TService>>());
        }


        public static IStandaloneFeignBuilder StandaloneFeignBuilder => s_standaloneFeignBuilder;

        public static TService Get<TService>()
        {
            return s_standaloneFeignBuilder.GetService<TService>();
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

            StandaloneFeignBuilder feignBuilder = s_standaloneFeignBuilder;

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
