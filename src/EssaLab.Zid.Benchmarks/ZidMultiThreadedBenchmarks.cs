using BenchmarkDotNet.Attributes;
using EssaLab.Zid;

[MemoryDiagnoser]
[RankColumn]
public class ZidMultiThreadedBenchmarks
{
    private const int OperationsCount = 10_000;

    [Benchmark(Baseline = true)]
    public void Standard_Guid_MultiThreaded()
    {
        Parallel.For(0, OperationsCount, _ =>
        {
            var id = Guid.NewGuid();
        });
    }

    [Benchmark]
    public void Zid_Next_MultiThreaded()
    {
        Parallel.For(0, OperationsCount, _ =>
        {
            var id = Zid.Next();
        });
    }
}