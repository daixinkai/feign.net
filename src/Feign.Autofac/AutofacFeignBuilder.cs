using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Builder;
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
            SetLifetime(registerBuilder, lifetime);
        }
        public void AddService(Type serviceType, FeignClientLifetime lifetime)
        {
            if (serviceType.IsGenericType && serviceType.IsGenericTypeDefinition)
            {
                var registerBuilder = ContainerBuilder.RegisterGeneric(serviceType).AsSelf();
                SetLifetime(registerBuilder, lifetime);
            }
            else
            {
                var registerBuilder = ContainerBuilder.RegisterType(serviceType).AsSelf();
                SetLifetime(registerBuilder, lifetime);
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

        private void SetLifetime<TLimit, TActivatorData, TRegistrationStyle>(IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registration, FeignClientLifetime lifetime)
        {
            switch (lifetime)
            {
                case FeignClientLifetime.Singleton:
                    registration.SingleInstance();
                    break;
                case FeignClientLifetime.Scoped:
                    registration.InstancePerLifetimeScope();
                    break;
                case FeignClientLifetime.Transient:
                    registration.InstancePerDependency();
                    break;
                default:
                    break;
            }
        }

    }
}
