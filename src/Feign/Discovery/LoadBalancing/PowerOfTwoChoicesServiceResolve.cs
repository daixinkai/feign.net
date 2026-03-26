using Feign.Internal;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Feign.Discovery.LoadBalancing
{
    /// <summary>
    /// PowerOfTwoChoices
    /// </summary>
    public class PowerOfTwoChoicesServiceResolve : ServiceResolveBase
    {
        public PowerOfTwoChoicesServiceResolve(ILogger logger) : base(logger)
        {
        }
        private static readonly Random _random = new Random();
        private readonly ConditionalWeakTable<string, AtomicCounter> _counters = new ConditionalWeakTable<string, AtomicCounter>();
        public override Uri ResolveServiceCore(Uri uri, IList<IServiceInstance> services)
        {
            var serviceCount = services.Count;
            // Pick two, and then return the least busy. This avoids the effort of searching the whole list, but
            // still avoids overloading a single destination.
            var firstIndex = _random.Next(serviceCount);
            int secondIndex;
            do
            {
                secondIndex = _random.Next(serviceCount);
            } while (firstIndex == secondIndex);
            var first = services[firstIndex];
            var second = services[secondIndex];
            var firstCounter = GetCounter(firstIndex);
            var secondCounter = GetCounter(secondIndex);
            if (firstCounter.Value <= secondCounter.Value)
            {
                firstCounter.Increment();
                return first.Uri;
            }
            secondCounter.Increment();
            return second.Uri;
        }

        private AtomicCounter GetCounter(int index)
            => _counters.GetOrCreateValue(index.ToString());

    }
}
