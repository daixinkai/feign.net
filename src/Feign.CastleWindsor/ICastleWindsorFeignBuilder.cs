using Castle.Windsor;
using Feign.Formatting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.CastleWindsor
{
    public interface ICastleWindsorFeignBuilder : IFeignBuilder
    {
        IWindsorContainer WindsorContainer { get; }
    }
}
