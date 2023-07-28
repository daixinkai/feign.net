using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.Discovery.LoadBalancing
{
    public abstract class ServiceResolveBase : IServiceResolve
    {
        protected ServiceResolveBase(ILogger logger)
        {
            _logger = logger;
        }
        private readonly ILogger _logger;
        public Uri ResolveService(Uri uri, IList<IServiceInstance> services)
        {
            _logger?.LogTrace($"ResolveService: {uri.Host}");
            if (services == null || services.Count == 0)
            {
                _logger?.LogWarning($"Attempted to resolve service for {uri.Host} but found 0 instances");
                return uri;
            }

            var resolvedUri = services.Count == 1 ? services[0].Uri : ResolveServiceCore(uri, services);

            //return new Uri(resolvedUri, uri.PathAndQuery);

            // http://xx.xx.com/child    <== ?

            if (resolvedUri.AbsolutePath == "/" || string.IsNullOrWhiteSpace(resolvedUri.AbsolutePath))
            {
                return new Uri(resolvedUri, uri.PathAndQuery);
            }
            if (string.IsNullOrWhiteSpace(uri.PathAndQuery))
            {
                return resolvedUri;
            }
            return new Uri(resolvedUri.ToString().TrimEnd('/') + "/" + uri.PathAndQuery.TrimStart('/'));
        }

        public abstract Uri ResolveServiceCore(Uri uri, IList<IServiceInstance> services);

    }
}
