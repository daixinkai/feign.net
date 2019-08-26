using Feign.Formatting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.DependencyInjection
{
    public interface IDependencyInjectionFeignBuilder : IFeignBuilder
    {
        IServiceCollection Services { get; }
    }
}
