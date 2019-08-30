using Autofac;
using BenchmarkDotNet.Attributes;
using Feign.Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.TestsConsole.NET45
{    
    [MemoryDiagnoser]
    public class AutoFacBenchmarkTest : FeignClientBenchmarkTest
    {
        public AutoFacBenchmarkTest()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            AutofacFeignBuilder = containerBuilder.AddFeignClients(options =>
            {
                options.Assemblies.Add(typeof(IBaiduTestService).Assembly);
            });
            AddTestFeignClients(AutofacFeignBuilder);
            Container = AutofacFeignBuilder.ContainerBuilder.Build();
        }
        public IAutofacFeignBuilder AutofacFeignBuilder { get; private set; }

        public IContainer Container { get; private set; }


        [Benchmark]
        public void TestService()
        {
            IBaiduTestService testService = Container.Resolve<IBaiduTestService>();
            string html = testService.Get();
        }

    }
}
