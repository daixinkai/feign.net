using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Autofac.Builder;
using Feign.Reflection;

namespace Feign.Autofac
{
    internal sealed class AutofacFeignBuilder : DefaultFeignBuilderBase, IAutofacFeignBuilder
    {

        public AutofacFeignBuilder(IFeignOptions options, ContainerBuilder containerBuilder) : base(options)
        {
            ContainerBuilder = containerBuilder;
        }

        public ContainerBuilder ContainerBuilder { get; set; }

        public override void AddService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            var registerBuilder = ContainerBuilder.RegisterType(implType).As(serviceType);
            SetLifetime(registerBuilder, lifetime);
        }
        public override void AddService(Type serviceType, FeignClientLifetime lifetime)
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
        public override void AddService<TService>(TService service)
        {
            ContainerBuilder.RegisterInstance(service).As<TService>();
        }

        public override void AddOrUpdateService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            AddService(serviceType, implType, lifetime);
        }
        public override void AddOrUpdateService(Type serviceType, FeignClientLifetime lifetime)
        {
            AddService(serviceType, lifetime);
        }
        public override void AddOrUpdateService<TService>(TService service)
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
