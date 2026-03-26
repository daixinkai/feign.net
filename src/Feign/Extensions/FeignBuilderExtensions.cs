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
                var feignClientTypeInfo = GetFeignClientTypeInfo(feignBuilder, serviceType, lifetime);
                if (feignClientTypeInfo == null || feignClientTypeInfo.BuildType == null)
                {
                    continue;
                }
                var feignClientAttribute = feignClientTypeInfo.FeignClient;
                feignBuilder.AddService(serviceType, feignClientTypeInfo.BuildType, feignClientTypeInfo.Lifetime);
                // add fallback
                if (feignClientAttribute.Fallback != null)
                {
                    feignBuilder.AddService(feignClientAttribute.Fallback, feignClientTypeInfo.Lifetime);
                }
                if (feignClientAttribute.FallbackFactory != null)
                {
                    feignBuilder.AddService(feignClientAttribute.FallbackFactory, feignClientTypeInfo.Lifetime);
                }
                // add feignClient proxy options
                if (feignClientTypeInfo.ProxyOptionsType != null)
                {
                    feignBuilder.AddService(feignClientTypeInfo.ProxyOptionsType.Type, feignClientTypeInfo.Lifetime);
                    feignBuilder.AddService(feignClientTypeInfo.ProxyOptionsType.ConfigurationType, feignClientTypeInfo.Lifetime);
                }
            }
            return feignBuilder;
        }

        /// <summary>
        /// Add keyd FeignClients
        /// </summary>
        /// <typeparam name="TFeignBuilder"></typeparam>
        /// <param name="key"></param>
        /// <param name="feignBuilder"></param>
        /// <param name="assembly">Assemblies to scan</param>
        /// <param name="lifetime">Service life cycle</param>
        /// <returns></returns>
        public static TFeignBuilder AddKeyedFeignClients<TFeignBuilder>(this TFeignBuilder feignBuilder, string key, Assembly? assembly, FeignClientLifetime lifetime)
            where TFeignBuilder : IKeyedFeignBuilder
        {
            if (assembly == null)
            {
                return feignBuilder;
            }
            foreach (var serviceType in assembly.GetTypes())
            {
                var feignClientTypeInfo = GetFeignClientTypeInfo(feignBuilder, serviceType, lifetime);
                if (feignClientTypeInfo == null || feignClientTypeInfo.BuildType == null)
                {
                    continue;
                }
                var feignClientAttribute = feignClientTypeInfo.FeignClient;
                var keydType = feignBuilder.TypeBuilder.BuildKeyedType(key, feignClientTypeInfo);
                feignBuilder.AddKeyedService(key, serviceType, keydType, feignClientTypeInfo.Lifetime);
                // add fallback
                if (feignClientAttribute.Fallback != null)
                {
                    feignBuilder.AddKeyedService(key, feignClientAttribute.Fallback, feignClientTypeInfo.Lifetime);
                }
                if (feignClientAttribute.FallbackFactory != null)
                {
                    feignBuilder.AddKeyedService(key, feignClientAttribute.FallbackFactory, feignClientTypeInfo.Lifetime);
                }
                // add feignClient proxy options
                if (feignClientTypeInfo.ProxyOptionsType != null)
                {
                    feignBuilder.AddKeyedService(key, feignClientTypeInfo.ProxyOptionsType.Type, feignClientTypeInfo.Lifetime);
                    feignBuilder.AddKeyedService(key, feignClientTypeInfo.ProxyOptionsType.ConfigurationType, feignClientTypeInfo.Lifetime);
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
        public static TFeignBuilder AddConverter<TFeignBuilder>(this TFeignBuilder feignBuilder, IConverter converter) where TFeignBuilder : IFeignBuilder
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

        private static FeignClientTypeInfo? GetFeignClientTypeInfo(IFeignBuilder feignBuilder, Type serviceType, FeignClientLifetime lifetime)
        {
            if (!feignBuilder.TypeBuilder.IsServiceType(serviceType))
            {
                return null;
            }
            var feignClientTypeInfo = feignBuilder.Options.Types.FirstOrDefault(s => s.ServiceType == serviceType);
            if (feignClientTypeInfo != null)
            {
                return feignClientTypeInfo;
            }
            feignClientTypeInfo = feignBuilder.TypeBuilder.Build(serviceType, lifetime);
            if (feignClientTypeInfo == null)
            {
                return null;
            }
            feignBuilder.Options.Types.Add(feignClientTypeInfo);
            return feignClientTypeInfo;
        }

    }
}
