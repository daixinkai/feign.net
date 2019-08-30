using BenchmarkDotNet.Attributes;
using Castle.Windsor;
using Feign.CastleWindsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.TestsConsole.NET45
{
    [MemoryDiagnoser]
    public class CastleWindsorBenchmarkTest : FeignClientBenchmarkTest
    {
        public CastleWindsorBenchmarkTest()
        {
            WindsorContainer windsorContainer = new WindsorContainer();
            CastleWindsorFeignBuilder = windsorContainer.AddFeignClients(options =>
            {
                options.Assemblies.Add(typeof(IBaiduTestService).Assembly);
            });
            AddTestFeignClients(CastleWindsorFeignBuilder);
            WindsorContainer = windsorContainer;
        }
        public ICastleWindsorFeignBuilder CastleWindsorFeignBuilder { get; private set; }

        public IWindsorContainer WindsorContainer { get; private set; }


        [Benchmark]
        public void TestService()
        {
            IBaiduTestService testService = WindsorContainer.Resolve<IBaiduTestService>();
            string html = testService.Get();
        }

    }
}
