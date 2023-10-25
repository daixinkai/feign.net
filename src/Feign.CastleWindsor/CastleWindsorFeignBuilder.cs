using System;
using System.Collections.Generic;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Feign.Formatting;
using Feign.Reflection;

namespace Feign.CastleWindsor
{
    internal sealed class CastleWindsorFeignBuilder : ICastleWindsorFeignBuilder
    {

        public CastleWindsorFeignBuilder()
        {
            TypeBuilder = new FeignClientHttpProxyTypeBuilder();
        }

        public IFeignOptions Options { get; set; }

        public IWindsorContainer WindsorContainer { get; set; }

        public IFeignClientTypeBuilder TypeBuilder { get; }

        public void AddService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            var registration = Component.For(serviceType).ImplementedBy(implType);
            WindsorContainer.Register(Lifestyle(registration, lifetime));
        }
        public void AddService(Type serviceType, FeignClientLifetime lifetime)
        {
            var registration = Component.For(serviceType);
            WindsorContainer.Register(Lifestyle(registration, lifetime));
        }
        public void AddService<TService>(TService service) where TService : class
        {
            var registration = Component.For<TService>().Instance(service);
            WindsorContainer.Register(registration);
        }

        public void AddOrUpdateService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            RemoveService(serviceType);
            AddService(serviceType, implType, lifetime);
        }
        public void AddOrUpdateService(Type serviceType, FeignClientLifetime lifetime)
        {
            RemoveService(serviceType);
            AddService(serviceType, lifetime);
        }
        public void AddOrUpdateService<TService>(TService service) where TService : class
        {
            RemoveService(typeof(TService));
            AddService(service);
        }


        private void RemoveService(Type serviceType)
        {
            if (!WindsorContainer.Kernel.HasComponent(serviceType))
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
