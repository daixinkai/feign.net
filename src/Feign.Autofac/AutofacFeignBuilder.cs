using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Builder;
using Feign.Formatting;
using Feign.Reflection;

namespace Feign.Autofac
{
    sealed class AutofacFeignBuilder : IAutofacFeignBuilder
    {

        public AutofacFeignBuilder()
        {
            TypeBuilder = new FeignClientHttpProxyTypeBuilder();
        }

        public IFeignOptions Options { get; set; }

        public ContainerBuilder ContainerBuilder { get; set; }

        public IFeignClientTypeBuilder TypeBuilder { get; }

        public void AddService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            var registerBuilder = ContainerBuilder.RegisterType(implType).As(serviceType);
            switch (lifetime)
            {
                case FeignClientLifetime.Singleton:
                    registerBuilder.SingleInstance();
                    break;
                case FeignClientLifetime.Scoped:
                    registerBuilder.InstancePerLifetimeScope();
                    break;
                case FeignClientLifetime.Transient:
                    registerBuilder.InstancePerDependency();
                    break;
                default:
                    break;
            }
        }
        public void AddService(Type serviceType, FeignClientLifetime lifetime)
        {
            var registerBuilder = ContainerBuilder.RegisterType(serviceType);
            switch (lifetime)
            {
                case FeignClientLifetime.Singleton:
                    registerBuilder.SingleInstance();
                    break;
                case FeignClientLifetime.Scoped:
                    registerBuilder.InstancePerLifetimeScope();
                    break;
                case FeignClientLifetime.Transient:
                    registerBuilder.InstancePerDependency();
                    break;
                default:
                    break;
            }
        }
        public void AddService<TService>(TService service) where TService : class
        {
            ContainerBuilder.RegisterInstance(service).As<TService>();
        }

        public void AddOrUpdateService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            AddService(serviceType, implType, lifetime);
        }
        public void AddOrUpdateService(Type serviceType, FeignClientLifetime lifetime)
        {
            AddService(serviceType, lifetime);
        }
        public void AddOrUpdateService<TService>(TService service) where TService : class
        {
            AddService(service);
        }

    }
}
