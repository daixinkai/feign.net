using Autofac;
using Feign.Formatting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Autofac
{
    public interface IAutofacFeignBuilder : IFeignBuilder
    {
        ContainerBuilder ContainerBuilder { get; }
    }
}
