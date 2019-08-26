using Feign.Cache;
using Feign.Discovery;
using Feign.Formatting;
using Feign.Logging;
using Feign.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class FeignBuilderExtensions
    {

        public static TFeignBuilder AddFeignClients<TFeignBuilder>(this TFeignBuilder feignBuilder, IFeignOptions options) where TFeignBuilder : IFeignBuilder
        {
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
            feignBuilder.AddLoggerFactory<DefaultLoggerFactory>();
            feignBuilder.AddCacheProvider<DefaultCacheProvider>();
            feignBuilder.AddServiceDiscovery<DefaultServiceDiscovery>();
            feignBuilder.AddService<IFeignOptions>(options);
            return feignBuilder;
        }

        public static TFeignBuilder AddFeignClients<TFeignBuilder>(this TFeignBuilder feignBuilder, Assembly assembly, FeignClientLifetime lifetime)
    where TFeignBuilder : IFeignBuilder
        {
            if (assembly == null)
            {
                return feignBuilder;
            }
            foreach (var serviceType in assembly.GetTypes())
            {
                FeignClientTypeInfo feignClientTypeInfo = feignBuilder.TypeBuilder.Build(serviceType);
                if (feignClientTypeInfo == null || feignClientTypeInfo.BuildType == null)
                {
                    continue;
                }
                FeignClientAttribute feignClientAttribute = serviceType.GetCustomAttribute<FeignClientAttribute>();
                feignBuilder.AddService(serviceType, feignClientTypeInfo.BuildType, feignClientAttribute.Lifetime ?? lifetime);
                // add fallback
                if (feignClientAttribute.Fallback != null)
                {
                    feignBuilder.AddService(feignClientAttribute.Fallback, feignClientAttribute.Lifetime ?? lifetime);
                }
                if (feignClientAttribute.FallbackFactory != null)
                {
                    feignBuilder.AddService(feignClientAttribute.FallbackFactory, feignClientAttribute.Lifetime ?? lifetime);
                }
            }
            return feignBuilder;
        }


        public static IFeignBuilder AddConverter<TSource, TResult>(this IFeignBuilder feignBuilder, IConverter<TSource, TResult> converter)
        {
            feignBuilder.Options.Converters.AddConverter(converter);
            return feignBuilder;
        }

        public static IFeignBuilder AddLoggerFactory<TLoggerFactory>(this IFeignBuilder feignBuilder) where TLoggerFactory : ILoggerFactory
        {
            feignBuilder.AddOrUpdateService(typeof(ILoggerFactory), typeof(TLoggerFactory), FeignClientLifetime.Singleton);
            return feignBuilder;
        }


        public static IFeignBuilder AddServiceDiscovery<TServiceDiscovery>(this IFeignBuilder feignBuilder) where TServiceDiscovery : IServiceDiscovery
        {
            feignBuilder.AddOrUpdateService(typeof(IServiceDiscovery), typeof(TServiceDiscovery), FeignClientLifetime.Singleton);
            return feignBuilder;
        }

        public static IFeignBuilder AddCacheProvider<TCacheProvider>(this IFeignBuilder feignBuilder) where TCacheProvider : ICacheProvider
        {
            feignBuilder.AddOrUpdateService(typeof(ICacheProvider), typeof(TCacheProvider), FeignClientLifetime.Singleton);
            return feignBuilder;
        }

    }
}
