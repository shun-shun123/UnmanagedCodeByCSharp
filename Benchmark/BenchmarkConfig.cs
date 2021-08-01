using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace LinqBenchmark.Benchmark
{
    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            AddExporter(HtmlExporter.Default);
            AddDiagnoser(MemoryDiagnoser.Default);
            AddJob(Job.Default);
        }
    }
}
