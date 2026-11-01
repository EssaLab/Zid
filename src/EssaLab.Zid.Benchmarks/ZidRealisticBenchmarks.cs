using BenchmarkDotNet.Attributes;
using EssaLab.Zid;

[MemoryDiagnoser]
[RankColumn]
public class ZidRealisticBenchmarks
{
    // حجم دفعة طلبات متزامنة واقعية تمر في نفس النافذة الزمنية (أقل من الحد الأقصى 256)
    private const int RealisticBatchSize = 100;

    [Benchmark(Baseline = true)]
    public void Standard_Guid_Batch()
    {
        Parallel.For(0, RealisticBatchSize, _ =>
        {
            var id = Guid.NewGuid();
        });
    }

    [Benchmark]
    public void Zid_Next_Batch()
    {
        Parallel.For(0, RealisticBatchSize, _ =>
        {
            var id = Zid.Next();
        });
    }
}