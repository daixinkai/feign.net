using Autofac;
using BenchmarkDotNet.Attributes;
using Feign.Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.TestsConsole
{
    [MemoryDiagnoser]
    public class AutoFacBenchmarkTest : FeignClientBenchmarkTest
    {
        public AutoFacBenchmarkTest()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            AutofacFeignBuilder = containerBuilder.AddFeignClients(options =>
            {
                options.Assemblies.Add(typeof(ITestService).Assembly);
            });
            AddTestFeignClients(AutofacFeignBuilder);
            Container = AutofacFeignBuilder.ContainerBuilder.Build();
        }
        public IAutofacFeignBuilder AutofacFeignBuilder { get; private set; }

        public IContainer Container { get; private set; }


        [Benchmark]
        public async Task TestService()
        {
            ITestService testService = Container.Resolve<ITestService>();
            string html = await testService.GetHtmlAsync();
        }

    }
}
