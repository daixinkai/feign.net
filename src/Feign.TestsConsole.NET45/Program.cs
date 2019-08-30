using BenchmarkDotNet.Running;
using Feign.Standalone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feign.TestsConsole.NET45
{
    class Program
    {
        static void Main(string[] args)
        {
            //InitFeignClients();
            //Test1();
            //Console.WriteLine("Hello World!");

            BenchmarkRunner.Run<AutoFacBenchmarkTest>();

            BenchmarkRunner.Run<CastleWindsorBenchmarkTest>();

            Console.ReadKey();
        }


        static void Test1()
        {
            IBaiduTestService testService = FeignClients.Get<IBaiduTestService>();

            Console.WriteLine(testService.GetType().FullName);

        }

        static void InitFeignClients()
        {
            FeignClients.AddFeignClients(options =>
            {
                options.Assemblies.Add(typeof(IBaiduTestService).Assembly);
                //options.FeignClientPipeline.ReceivingQueryResult();
            });
        }

    }
}
