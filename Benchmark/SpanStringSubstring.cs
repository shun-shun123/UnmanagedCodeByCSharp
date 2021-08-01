using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace LinqBenchmark.Benchmark
{
    [Config(typeof(BenchmarkConfig))]
    public class SpanStringSubstring
    {
        private const int TRY_COUNT = 100_000;

        private string longStrs;

        private Random rand;
        
        [GlobalSetup]
        public void Setup()
        {
            var strs = Enumerable.Range(0, 100).Select(i => i.ToString()).ToArray();
            foreach (var s in strs)
            {
                longStrs += s;
            }

            rand = new Random();
        }

        [Benchmark]
        public void SubstringByString()
        {
            for (var i = 0; i < TRY_COUNT; i++)
            {
                var _ = longStrs.Substring(rand.Next(longStrs.Length));
            }
        }

        [Benchmark]
        public void SubstringBySpan()
        {
            for (var i = 0; i < TRY_COUNT; i++)
            {
                var _ = longStrs.AsSpan()[..rand.Next(longStrs.Length)];
            }
        }
    }
}