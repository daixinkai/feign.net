using Feign.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign
{
    public abstract class DefaultFeignBuilderBase : IFeignBuilder
    {
        protected DefaultFeignBuilderBase(FeignOptions options)
        {
            Options = options;
            TypeBuilder = new FeignClientHttpProxyTypeBuilder();
        }

        public FeignOptions Options { get; }

        public IFeignClientTypeBuilder TypeBuilder { get; set; }

        public virtual bool SupportGenericService => false;

        public abstract void AddOrUpdateService(Type serviceType, Type implType, FeignClientLifetime lifetime);
        public abstract void AddOrUpdateService(Type serviceType, FeignClientLifetime lifetime);
        public abstract void AddOrUpdateService<TService>(TService service) where TService : class;
        public abstract void AddService(Type serviceType, Type implType, FeignClientLifetime lifetime);
        public abstract void AddService(Type serviceType, FeignClientLifetime lifetime);
        public abstract void AddService<TService>(TService service) where TService : class;
    }
}
