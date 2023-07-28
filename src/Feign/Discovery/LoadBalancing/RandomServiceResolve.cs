using Feign.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Feign.Discovery.LoadBalancing
{
    /// <summary>
    /// 随机服务决定
    /// </summary>
    public class RandomServiceResolve : ServiceResolveBase
    {
        public RandomServiceResolve(ILogger logger) : base(logger)
        {
        }
        private static readonly Random _random = new Random();
        public override Uri ResolveServiceCore(Uri uri, IList<IServiceInstance> services)
        {
            return services[_random.Next(services.Count)].Uri;
        }
    }
}
