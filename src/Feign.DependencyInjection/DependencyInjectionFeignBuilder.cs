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
    {

        public DependencyInjectionFeignBuilder(IFeignOptions options, IServiceCollection services) : base(options)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }

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
            RemoveService(serviceType);
            AddService(serviceType, implType, lifetime);
        }
        public override void AddOrUpdateService(Type serviceType, FeignClientLifetime lifetime)
        {
            RemoveService(serviceType);
            AddService(serviceType, lifetime);
        }
        public override void AddOrUpdateService<TService>(TService service)
        {
            RemoveService(typeof(TService));
            AddService(service);
        }

        private void RemoveService(Type serviceType)
        {
            Services.RemoveAll(serviceType);
        }

    }
}
