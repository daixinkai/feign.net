using BenchmarkDotNet.Running;
using Feign.TestsConsole;

//BenchmarkRunner.Run<AutoFacBenchmarkTest>();

//BenchmarkRunner.Run<CastleWindsorBenchmarkTest>();

BenchmarkRunner.Run<DependencyInjectionBenchmarkTest>();

Console.ReadKey();