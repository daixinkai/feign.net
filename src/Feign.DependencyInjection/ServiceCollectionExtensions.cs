using Feign;
using Feign.Cache;
using Feign.DependencyInjection;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ServiceCollectionExtensions
    {

        public static IDependencyInjectionFeignBuilder AddFeignClients(this IServiceCollection services)
        {
            return AddFeignClients(services, default(FeignOptions));
        }

        public static IDependencyInjectionFeignBuilder AddFeignClients(this IServiceCollection services, Action<FeignOptions> setupAction)
        {
            var options = new FeignOptions();
            setupAction?.Invoke(options);
            return AddFeignClients(services, options);
        }

        public static IDependencyInjectionFeignBuilder AddFeignClients(this IServiceCollection services, FeignOptions? options)
        {
            options ??= new FeignOptions();
            var feignBuilder = new DependencyInjectionFeignBuilder(options, services);
            feignBuilder.AddDefaultFeignClients()
            .AddLoggerFactory<LoggerFactory>()
            .AddCacheProvider<JsonCacheProvider>()
            ;
            return feignBuilder;
        }


    }
}
