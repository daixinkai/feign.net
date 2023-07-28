using Feign.Internal;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Feign.Discovery.LoadBalancing
{
    /// <summary>
    /// RoundRobin
    /// </summary>
    public class RoundRobinServiceResolve : ServiceResolveBase
    {
        public RoundRobinServiceResolve(ILogger logger) : base(logger)
        {
        }
        private readonly AtomicCounter _counter = new AtomicCounter();
        public override Uri ResolveServiceCore(Uri uri, IList<IServiceInstance> services)
        {
            var offset = _counter.Increment() - 1;
            return services[(offset & 0x7FFFFFFF) % services.Count].Uri;
        }
    }
}
