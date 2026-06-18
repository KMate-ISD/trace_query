using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;

namespace TraceQuery.Profiling;

public static class Program
{
    public static void Main(string[] args)
    {
        var config = ManualConfig.Create(DefaultConfig.Instance);
        
        BenchmarkRunner.Run<IngestionBenchmark>(config);
    }
}