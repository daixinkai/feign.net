﻿using Feign.Internal;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Feign.Discovery.LoadBalancing
{
    /// <summary>
    /// LeastRequests
    /// </summary>
    public class LeastRequestsServiceResolve : ServiceResolveBase
    {
        public LeastRequestsServiceResolve(ILogger? logger) : base(logger)
        {
        }

        private readonly ConditionalWeakTable<string, AtomicCounter> _counters = new ConditionalWeakTable<string, AtomicCounter>();

        public override Uri ResolveServiceCore(Uri uri, IList<IServiceInstance> services)
        {
            var serviceCount = services.Count;
            var leastRequestsService = services[0];
            var leastCounter = GetCounter(0);
            var leastRequestsCount = leastCounter.Value;
            for (var i = 1; i < serviceCount; i++)
            {
                var service = services[i];
                var endpointCounter= GetCounter(i);
                var endpointRequestCount = endpointCounter.Value;
                if (endpointRequestCount < leastRequestsCount)
                {
                    leastRequestsService = service;
                    leastCounter = endpointCounter;
                    leastRequestsCount = endpointRequestCount;
                }
            }
            leastCounter.Increment();
            return leastRequestsService.Uri;
        }

        private AtomicCounter GetCounter(int index)
            => _counters.GetOrCreateValue(index.ToString());

    }
}
