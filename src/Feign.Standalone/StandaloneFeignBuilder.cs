using Feign.Formatting;
using Feign.Reflection;
using Feign.Standalone.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Standalone
{
    class StandaloneFeignBuilder : IStandaloneFeignBuilder
    {
        public StandaloneFeignBuilder()
        {
            TypeBuilder = new StandaloneFeignClientHttpProxyTypeBuilder();
            Services = new ServiceCollection();
        }

        public FeignOptions Options { get; set; }

        public IFeignClientTypeBuilder TypeBuilder { get; }

        public ServiceCollection Services { get; }


        public object GetService(Type type)
        {
            ServiceDescriptor serviceDescriptor = Services.Get(type);
            if (serviceDescriptor == null)
            {
                return null;
            }
            return serviceDescriptor.GetService();
        }

        public TService GetService<TService>()
        {
            object service = GetService(typeof(TService));
            if (service == null)
            {
                return default(TService);
            }
            return (TService)service;
        }

        IFeignOptions IFeignBuilder.Options => Options;

        public void AddService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            Services.AddOrUpdate(new ServiceDescriptor(serviceType, implType, lifetime));
        }
        public void AddService(Type serviceType, FeignClientLifetime lifetime)
        {
            Services.AddOrUpdate(new ServiceDescriptor(serviceType, serviceType, lifetime));
        }
        public void AddService<TService>(TService service) where TService : class
        {
            Services.AddOrUpdate(new ServiceDescriptor(typeof(TService), service));
        }

        public void AddOrUpdateService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            Services.AddOrUpdate(new ServiceDescriptor(serviceType, implType, lifetime));
        }
        public void AddOrUpdateService(Type serviceType, FeignClientLifetime lifetime)
        {
            Services.AddOrUpdate(new ServiceDescriptor(serviceType, serviceType, lifetime));
        }
        public void AddOrUpdateService<TService>(TService service) where TService : class
        {
            Services.AddOrUpdate(new ServiceDescriptor(typeof(TService), service));
        }

    }
}
