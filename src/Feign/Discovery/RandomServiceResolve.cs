using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery
{
    /// <summary>
    /// 随机服务决定
    /// </summary>
    public class RandomServiceResolve : IServiceResolve
    {
        public RandomServiceResolve(ILogger logger)
        {
            _logger = logger;
        }
        private static readonly Random _random = new Random();
        private readonly ILogger _logger;
        public Uri ResolveService(Uri uri, IList<IServiceInstance> services)
        {
            _logger?.LogTrace($"ResolveService: {uri.Host}");
            if (services == null || services.Count == 0)
            {
                _logger?.LogWarning($"Attempted to resolve service for {uri.Host} but found 0 instances");
                return uri;
            }
            Uri resolvedUri;
            if (services.Count == 1)
            {
                resolvedUri = services[0].Uri;
            }
            else
            {
                resolvedUri = services[_random.Next(services.Count)].Uri;
            }
            _logger?.LogWarning($"Attempted to resolve service for {uri.Host} but found 0 instances");

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
    }
}
