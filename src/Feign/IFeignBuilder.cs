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

        /// <summary>
        /// Adds the specified <paramref name="serviceType"/> as a <see cref="FeignClientLifetime"/> service
        /// with the <paramref name="implType"/> implementation
        /// if the service type hasn't already been registered.
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implType">The implementation type of the service.</param>
        /// <param name="lifetime">Specifies the lifetime of a service</param>
        void AddService(Type serviceType, Type implType, FeignClientLifetime lifetime);
        /// <summary>
        /// Adds the specified <paramref name="serviceType"/> as a <see cref="FeignClientLifetime"/> service
        /// if the service type hasn't already been registered.
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="lifetime">Specifies the lifetime of a service</param>
        void AddService(Type serviceType, FeignClientLifetime lifetime);
        /// <summary>
        /// Adds the specified <typeparamref name="TService"/> as a <see cref="FeignClientLifetime.Singleton"/> service
        /// with an instance specified in <paramref name="service"/>
        /// if the service type hasn't already been registered.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        void AddService<TService>(TService service) where TService : class;

        /// <summary>
        /// Add or update the specified <paramref name="serviceType"/> as a <see cref="FeignClientLifetime"/> service
        /// with the <paramref name="implType"/> implementation
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="implType">The implementation type of the service.</param>
        /// <param name="lifetime">Specifies the lifetime of a service</param>
        void AddOrUpdateService(Type serviceType, Type implType, FeignClientLifetime lifetime);
        /// <summary>
        /// Add or update the specified <paramref name="serviceType"/> as a <see cref="FeignClientLifetime"/> service
        /// </summary>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="lifetime">Specifies the lifetime of a service</param>
        void AddOrUpdateService(Type serviceType, FeignClientLifetime lifetime);
        /// <summary>
        /// Add or update the specified <typeparamref name="TService"/> as a <see cref="FeignClientLifetime.Singleton"/> service
        /// with an instance specified in <paramref name="service"/>
        /// </summary>
        /// <typeparam name="TService">The type of the service to add or update.</typeparam>
        void AddOrUpdateService<TService>(TService service) where TService : class;

    }
}
