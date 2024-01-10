using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Feign.Formatting;
using Feign.Reflection;

namespace Feign.CastleWindsor
{
    internal sealed class CastleWindsorFeignBuilder : DefaultFeignBuilderBase, ICastleWindsorFeignBuilder
    {

        public CastleWindsorFeignBuilder(IFeignOptions options, IWindsorContainer windsorContainer) : base(options)
        {
            WindsorContainer = windsorContainer;
        }

        public IWindsorContainer WindsorContainer { get; }

        public override void AddService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            var registration = Component.For(serviceType).ImplementedBy(implType);
            WindsorContainer.Register(Lifestyle(registration, lifetime));
        }
        public override void AddService(Type serviceType, FeignClientLifetime lifetime)
        {
            var registration = Component.For(serviceType);
            WindsorContainer.Register(Lifestyle(registration, lifetime));
        }
        public override void AddService<TService>(TService service)
        {
            var registration = Component.For<TService>().Instance(service);
            WindsorContainer.Register(registration);
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


        private bool HasService(Type serviceType) => WindsorContainer.Kernel.HasComponent(serviceType);

        private void RemoveService(Type serviceType)
        {
            if (!HasService(serviceType))
            {
                return;
            }
            WindsorContainer.Kernel.RemoveComponent(serviceType);
        }

        private ComponentRegistration<T> Lifestyle<T>(ComponentRegistration<T> registration, FeignClientLifetime lifetime)
              where T : class
        {
            switch (lifetime)
            {
                case FeignClientLifetime.Transient:
                    return registration.LifestyleTransient();
                case FeignClientLifetime.Singleton:
                    return registration.LifestyleSingleton();
                case FeignClientLifetime.Scoped:
                    return registration.LifestyleScoped();
                default:
                    return registration;
            }
        }


    }
}
