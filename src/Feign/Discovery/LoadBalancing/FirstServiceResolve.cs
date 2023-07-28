using Feign.Internal;
using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Feign.Discovery.LoadBalancing
{
    /// <summary>
    /// Select the alphabetically first available destination without considering load. This is useful for dual destination fail-over systems.
    /// </summary>
    public class FirstServiceResolve : ServiceResolveBase
    {
        public FirstServiceResolve(ILogger logger) : base(logger)
        {
        }
        public override Uri ResolveServiceCore(Uri uri, IList<IServiceInstance> services)
        {
            var selectedService = services[0];
            for (var i = 1; i < services.Count; i++)
            {
                var service = services[i];
                if (string.Compare(selectedService.Host, service.Host, StringComparison.OrdinalIgnoreCase) > 0)
                {
                    selectedService = service;
                }
            }
            return selectedService.Uri;
        }
    }
}
