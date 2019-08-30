using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.TestsConsole.NET45
{
    public abstract class FeignClientBenchmarkTest
    {
        protected static void AddTestFeignClients(IFeignBuilder feignBuilder)
        {
            //feignBuilder.Options.Lifetime = FeignClientLifetime.Transient;
            //feignBuilder.AddFeignClients(this.GetType().Assembly, feignBuilder.Options.Lifetime);
        }
    }
}
