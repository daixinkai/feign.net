using Feign.Cache;
using Feign.Configuration;
using Feign.Discovery;
using Feign.Formatting;
using Feign.Logging;
using Feign.Proxy;
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
        /// <summary>
        /// Add default FeignClients
        /// </summary>
        /// <typeparam name="TFeignBuilder"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <returns></returns>
        public static TFeignBuilder AddDefaultFeignClients<TFeignBuilder>(this TFeignBuilder feignBuilder) where TFeignBuilder : IFeignBuilder
        {
            if (feignBuilder.Options.Assemblies.Count == 0)
            {
                feignBuilder.AddFeignClients(Assembly.GetEntryAssembly(), feignBuilder.Options.Lifetime);
            }
            else
            {
                foreach (var assembly in feignBuilder.Options.Assemblies)
                {
                    feignBuilder.AddFeignClients(assembly, feignBuilder.Options.Lifetime);
                }
            }
            feignBuilder.AddLoggerFactory<DefaultLoggerFactory>();
            feignBuilder.AddCacheProvider<DefaultCacheProvider>();
            feignBuilder.AddServiceDiscovery<DefaultServiceDiscovery>();
            feignBuilder.AddService(feignBuilder.Options);
            //if (feignBuilder.Options.Lifetime != FeignClientLifetime.Singleton
            //    || feignBuilder.Options.Types.Any(static s => s.FeignClient.Lifetime.HasValue && s.FeignClient.Lifetime.Value != FeignClientLifetime.Singleton))
            //{

            //}
            //else
            //{

            //}
            //foreach (var type in feignBuilder.Options.Types)
            //{
            //    var lifetime = type.FeignClient.Lifetime ?? feignBuilder.Options.Lifetime;
            //}
            feignBuilder.AddService(typeof(FeignClientHttpProxyOptions), FeignClientLifetime.Singleton);
            return feignBuilder;
        }
        /// <summary>
        /// Add FeignClients
        /// </summary>
        /// <typeparam name="TFeignBuilder"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <param name="assembly">Assemblies to scan</param>
        /// <param name="lifetime">Service life cycle</param>
        /// <returns></returns>
        public static TFeignBuilder AddFeignClients<TFeignBuilder>(this TFeignBuilder feignBuilder, Assembly? assembly, FeignClientLifetime lifetime)
            where TFeignBuilder : IFeignBuilder
        {
            if (assembly == null)
            {
                return feignBuilder;
            }
            foreach (var serviceType in assembly.GetTypes())
            {
                FeignClientTypeInfo? feignClientTypeInfo = feignBuilder.TypeBuilder.Build(serviceType);
                if (feignClientTypeInfo == null || feignClientTypeInfo.BuildType == null)
                {
                    continue;
                }
                feignBuilder.Options.Types.Add(feignClientTypeInfo);
                FeignClientAttribute feignClientAttribute = feignClientTypeInfo.FeignClient;
                var feignClientLifetime = feignClientAttribute.Lifetime ?? lifetime;
                feignBuilder.AddService(serviceType, feignClientTypeInfo.BuildType, feignClientLifetime);
                // add fallback
                if (feignClientAttribute.Fallback != null)
                {
                    feignBuilder.AddService(feignClientAttribute.Fallback, feignClientLifetime);
                }
                if (feignClientAttribute.FallbackFactory != null)
                {
                    feignBuilder.AddService(feignClientAttribute.FallbackFactory, feignClientLifetime);
                }
                // add feignClient proxy options
                if (feignClientTypeInfo.ProxyOptionsType != null)
                {
                    feignBuilder.AddService(feignClientTypeInfo.ProxyOptionsType.Type, feignClientLifetime);
                    feignBuilder.AddService(feignClientTypeInfo.ProxyOptionsType.ConfigurationType, feignClientLifetime);
                }
            }
            return feignBuilder;
        }

        /// <summary>
        /// Add a converter
        /// </summary>
        /// <param name="feignBuilder"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static IFeignBuilder AddConverter(this IFeignBuilder feignBuilder, IConverter converter)
        {
            feignBuilder.Options.Converters.AddConverter(converter);
            return feignBuilder;
        }

        /// <summary>
        /// Add <see cref="ILoggerFactory"/>
        /// </summary>
        /// <typeparam name="TLoggerFactory"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <returns></returns>
        public static IFeignBuilder AddLoggerFactory<TLoggerFactory>(this IFeignBuilder feignBuilder) where TLoggerFactory : ILoggerFactory
        {
            feignBuilder.AddOrUpdateService(typeof(ILoggerFactory), typeof(TLoggerFactory), FeignClientLifetime.Singleton);
            return feignBuilder;
        }

        /// <summary>
        /// Add <see cref="IServiceDiscovery"/>
        /// </summary>
        /// <typeparam name="TServiceDiscovery"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <returns></returns>
        public static IFeignBuilder AddServiceDiscovery<TServiceDiscovery>(this IFeignBuilder feignBuilder) where TServiceDiscovery : IServiceDiscovery
        {
            feignBuilder.AddOrUpdateService(typeof(IServiceDiscovery), typeof(TServiceDiscovery), FeignClientLifetime.Singleton);
            return feignBuilder;
        }
        /// <summary>
        /// Add <see cref="ICacheProvider"/>
        /// </summary>
        /// <typeparam name="TCacheProvider"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <returns></returns>
        public static IFeignBuilder AddCacheProvider<TCacheProvider>(this IFeignBuilder feignBuilder) where TCacheProvider : ICacheProvider
        {
            feignBuilder.AddOrUpdateService(typeof(ICacheProvider), typeof(TCacheProvider), FeignClientLifetime.Singleton);
            return feignBuilder;
        }
        /// <summary>
        /// Configure JsonSettings
        /// </summary>
        /// <typeparam name="TFeignBuilder"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static TFeignBuilder ConfigureJsonSettings<TFeignBuilder>(this TFeignBuilder feignBuilder, Action<JsonSerializerOptions> configure) where TFeignBuilder : IFeignBuilder
        {
            feignBuilder.Options.ConfigureJsonSettings(configure);
            return feignBuilder;
        }

    }
}
