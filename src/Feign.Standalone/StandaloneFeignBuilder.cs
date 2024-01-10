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
    internal class StandaloneFeignBuilder : DefaultFeignBuilderBase, IStandaloneFeignBuilder
    {
        public StandaloneFeignBuilder(IFeignOptions options) : base(options)
        {
            Services = new ServiceCollection();
        }

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

        public override void AddService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            Services.AddOrUpdate(new ServiceDescriptor(serviceType, implType, lifetime));
        }
        public override void AddService(Type serviceType, FeignClientLifetime lifetime)
        {
            Services.AddOrUpdate(new ServiceDescriptor(serviceType, serviceType, lifetime));
        }
        public override void AddService<TService>(TService service)
        {
            Services.AddOrUpdate(new ServiceDescriptor(typeof(TService), service));
        }

        public override void AddOrUpdateService(Type serviceType, Type implType, FeignClientLifetime lifetime)
        {
            Services.AddOrUpdate(new ServiceDescriptor(serviceType, implType, lifetime));
        }
        public override void AddOrUpdateService(Type serviceType, FeignClientLifetime lifetime)
        {
            Services.AddOrUpdate(new ServiceDescriptor(serviceType, serviceType, lifetime));
        }
        public override void AddOrUpdateService<TService>(TService service)
        {
            Services.AddOrUpdate(new ServiceDescriptor(typeof(TService), service));
        }

    }
}
