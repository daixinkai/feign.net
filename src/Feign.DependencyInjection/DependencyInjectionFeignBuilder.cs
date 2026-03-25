using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Feign.Formatting;
using Feign.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Feign.DependencyInjection
{
    internal sealed class DependencyInjectionFeignBuilder : DefaultFeignBuilderBase, IDependencyInjectionFeignBuilder
#if NET8_0_OR_GREATER
        , IKeyedFeignBuilder
#endif
    {

        public DependencyInjectionFeignBuilder(FeignOptions options, IServiceCollection services) : base(options)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

        public override bool SupportGenericService => true;

        public override void AddService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            switch (lifetime)
            {
                case FeignClientLifetime.Singleton:
                    Services.TryAddSingleton(serviceType, implType);
                    break;
                case FeignClientLifetime.Scoped:
                    Services.TryAddScoped(serviceType, implType);
                    break;
                case FeignClientLifetime.Transient:
                    Services.TryAddTransient(serviceType, implType);
                    break;
                default:
                    break;
            }
        }
        public override void AddService(Type serviceType, FeignClientLifetime lifetime)
        {
            switch (lifetime)
            {
                case FeignClientLifetime.Singleton:
                    Services.TryAddSingleton(serviceType);
                    break;
                case FeignClientLifetime.Scoped:
                    Services.TryAddScoped(serviceType);
                    break;
                case FeignClientLifetime.Transient:
                    Services.TryAddTransient(serviceType);
                    break;
                default:
                    break;
            }
        }

        public override void AddService<TService>(TService service)
        {
            Services.TryAddSingleton(service);
        }

        public override void AddOrUpdateService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            Services.RemoveAll(serviceType);
            AddService(serviceType, implType, lifetime);
        }
        public override void AddOrUpdateService(Type serviceType, FeignClientLifetime lifetime)
        {
            Services.RemoveAll(serviceType);
            AddService(serviceType, lifetime);
        }
        public override void AddOrUpdateService<TService>(TService service)
        {
            Services.RemoveAll(typeof(TService));
            AddService(service);
        }



#if NET8_0_OR_GREATER
        public void AddKeyedService(string key, Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            object? serviceKey = key;
            switch (lifetime)
            {
                case FeignClientLifetime.Singleton:
                    Services.TryAddKeyedSingleton(serviceType, serviceKey, implType);
                    break;
                case FeignClientLifetime.Scoped:
                    Services.TryAddKeyedScoped(serviceType, serviceKey, implType);
                    break;
                case FeignClientLifetime.Transient:
                    Services.TryAddKeyedTransient(serviceType, serviceKey, implType);
                    break;
                default:
                    break;
            }
        }

        public void AddKeyedService(string key, Type serviceType, FeignClientLifetime lifetime)
        {
            object? serviceKey = key;
            switch (lifetime)
            {
                case FeignClientLifetime.Singleton:
                    Services.TryAddKeyedSingleton(serviceType, serviceKey);
                    break;
                case FeignClientLifetime.Scoped:
                    Services.TryAddKeyedScoped(serviceType, serviceKey);
                    break;
                case FeignClientLifetime.Transient:
                    Services.TryAddKeyedTransient(serviceType, serviceKey);
                    break;
                default:
                    break;
            }
        }

        public void AddOrUpdateKeyedService(string key, Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            object? serviceKey = key;
            Services.RemoveAllKeyed(serviceType, serviceKey);
            AddKeyedService(key, serviceType, implType, lifetime);
        }
#endif
    }
}
