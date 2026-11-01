using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using EssaLab.Zid;

[MemoryDiagnoser] // لقياس استهلاك الذاكرة والـ GC (Garbage Collector)
[RankColumn]      // لترتيب النتائج من الأسرع للأبطأ
public class ZidBenchmarks
{
    private static readonly Zid SampleZid = Zid.Next();
    private static readonly string SampleBase62 = SampleZid.ToShort();

    [Benchmark(Baseline = true)]
    public Guid Standard_Guid_NewGuid()
    {
        return Guid.NewGuid();
    }

    [Benchmark]
    public Zid Zid_Next_Generation()
    {
        return Zid.Next();
    }

    [Benchmark]
    public string Zid_Base62_Encoding()
    {
        return SampleZid.ToShort();
    }

    [Benchmark]
    public bool Zid_Base62_Parsing()
    {
        return Zid.TryParse(SampleBase62, out _);
    }
}