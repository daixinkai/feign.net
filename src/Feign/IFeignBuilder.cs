using Feign.Formatting;
using Feign.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign
{
    public interface IFeignBuilder
    {
        IFeignOptions Options { get; }
        IFeignClientTypeBuilder TypeBuilder { get; }

        void AddService(Type serviceType, Type implType, FeignClientLifetime lifetime);
        void AddService(Type serviceType, FeignClientLifetime lifetime);
        void AddService<TService>(TService service) where TService : class;

        void AddOrUpdateService(Type serviceType, Type implType, FeignClientLifetime lifetime);
        void AddOrUpdateService(Type serviceType, FeignClientLifetime lifetime);
        void AddOrUpdateService<TService>(TService service) where TService : class;

    }
}
