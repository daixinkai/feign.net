using BenchmarkDotNet.Attributes;
using Castle.Windsor;
using Feign.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.TestsConsole
{
    [MemoryDiagnoser]
    public class DependencyInjectionBenchmarkTest : FeignClientBenchmarkTest
    {
        public DependencyInjectionBenchmarkTest()
        {
            IServiceCollection services = new ServiceCollection();
            DependencyInjectionFeignBuilder = services.AddFeignClients(options =>
            {
                options.Assemblies.Add(typeof(ITestService).Assembly);
            });
            AddTestFeignClients(DependencyInjectionFeignBuilder);
            Services = services.BuildServiceProvider();
        }
        public IDependencyInjectionFeignBuilder DependencyInjectionFeignBuilder { get; private set; }

        public IServiceProvider Services { get; private set; }


        [Benchmark]
        public async Task TestService()
        {
            ITestService testService = Services.GetRequiredService<ITestService>();
            string html = await testService.GetHtmlAsync();
        }
    }
}
