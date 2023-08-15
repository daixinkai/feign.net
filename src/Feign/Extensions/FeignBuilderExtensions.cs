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
        /// <summary>
        /// 添加默认FeignClients
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
            feignBuilder.AddService<IFeignOptions>(feignBuilder.Options);
            return feignBuilder;
        }
        /// <summary>
        /// 添加FeignClients
        /// </summary>
        /// <typeparam name="TFeignBuilder"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <param name="assembly">要扫描的程序集</param>
        /// <param name="lifetime">服务的生命周期</param>
        /// <returns></returns>
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
                feignBuilder.Options.Types.Add(feignClientTypeInfo);
                //FeignClientAttribute feignClientAttribute = serviceType.GetCustomAttribute<FeignClientAttribute>();
                FeignClientAttribute feignClientAttribute = serviceType.GetCustomAttributeIncludingBaseInterfaces<FeignClientAttribute>();
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

        /// <summary>
        /// 添加一个转换器
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="feignBuilder"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static IFeignBuilder AddConverter<TSource, TResult>(this IFeignBuilder feignBuilder, IConverter<TSource, TResult> converter)
        {
            feignBuilder.Options.Converters.AddConverter(converter);
            return feignBuilder;
        }
        /// <summary>
        /// 添加<see cref="ILoggerFactory"/>
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
        /// 添加<see cref="IServiceDiscovery"/>
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
        /// 添加<see cref="ICacheProvider"/>
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
