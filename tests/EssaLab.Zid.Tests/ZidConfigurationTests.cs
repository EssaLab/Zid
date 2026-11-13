namespace EssaLab.Zid.Tests;

[Collection(ZidGlobalState.Name)]
public class ZidConfigurationTests : IDisposable
{
    public ZidConfigurationTests() => Zid.ResetForTests();
    public void Dispose()
    {
        Environment.SetEnvironmentVariable("ZID_MACHINE_ID", null);
        Environment.SetEnvironmentVariable("ZID_ALLOW_REINIT", null);
        Zid.ResetForTests();
    }

    [Fact]
    public void Initialize_WithCustomMachineId_IsHonoured()
    {
        Zid.Initialize(new ZidOptions { MachineId = 7 });

        Assert.Equal(7, Zid.Next().MachineId);
    }

    [Fact]
    public void Initialize_WithCustomEpoch_IsReflectedInTimestamp()
    {
        var epoch = DateTimeOffset.UtcNow.AddDays(-30);
        Zid.Initialize(new ZidOptions { Epoch = epoch, MachineId = 1 });

        var zid = Zid.Next();

        Assert.InRange((zid.Timestamp - epoch).TotalDays, 29.99, 30.01);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1024)]
    [InlineData(2000)]
    public void Initialize_WithOutOfRangeMachineId_Throws(long machineId)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Zid.Initialize(new ZidOptions { MachineId = machineId }));
    }

    [Fact]
    public void Initialize_WithFutureEpoch_ThrowsInsteadOfHanging()
    {
        var options = new ZidOptions { Epoch = DateTimeOffset.UtcNow.AddDays(1) };

        Assert.Throws<ArgumentOutOfRangeException>(() => Zid.Initialize(options));
    }

    [Fact]
    public void Initialize_WithEpochBefore1970_Throws()
    {
        var options = new ZidOptions { Epoch = new DateTimeOffset(1969, 1, 1, 0, 0, 0, TimeSpan.Zero) };

        Assert.Throws<ArgumentOutOfRangeException>(() => Zid.Initialize(options));
    }

    [Fact]
    public void Initialize_WithNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => Zid.Initialize(null!));
    }

    [Fact]
    public void Initialize_AfterUse_WithConflictingOptions_Throws()
    {
        Zid.Initialize(new ZidOptions { MachineId = 3 });
        Zid.Next();

        Assert.Throws<InvalidOperationException>(() => Zid.Initialize(new ZidOptions { MachineId = 4 }));
        Assert.Equal(3, Zid.Next().MachineId); // configuration unchanged
    }

    [Fact]
    public void Initialize_AfterUse_WithIdenticalOptions_IsNoOp()
    {
        Zid.Initialize(new ZidOptions { MachineId = 3 });
        Zid.Next();

        Zid.Initialize(new ZidOptions { MachineId = 3 });
        Zid.Initialize(new ZidOptions()); // MachineId unspecified => compatible

        Assert.Equal(3, Zid.Next().MachineId);
    }

    [Fact]
    public void Initialize_AfterUse_WithReinitFlag_AppliesNewOptions()
    {
        Zid.Initialize(new ZidOptions { MachineId = 3 });
        Zid.Next();
        Environment.SetEnvironmentVariable("ZID_ALLOW_REINIT", "true");

        Zid.Initialize(new ZidOptions { MachineId = 9 });

        Assert.Equal(9, Zid.Next().MachineId);
    }

    [Fact]
    public void MachineId_FromEnvironmentVariable_IsUsed()
    {
        Environment.SetEnvironmentVariable("ZID_MACHINE_ID", "42");

        Assert.Equal(42, Zid.Next().MachineId);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("-5")]
    [InlineData("1024")]
    public void MachineId_FromInvalidEnvironmentVariable_ThrowsInsteadOfFallingBackToRandom(string value)
    {
        Environment.SetEnvironmentVariable("ZID_MACHINE_ID", value);

        Assert.Throws<InvalidOperationException>(() => Zid.Next());
    }

    [Fact]
    public void ExplicitMachineId_WinsOverEnvironmentVariable()
    {
        Environment.SetEnvironmentVariable("ZID_MACHINE_ID", "42");
        Zid.Initialize(new ZidOptions { MachineId = 5 });

        Assert.Equal(5, Zid.Next().MachineId);
    }

    [Fact]
    public void Next_FromManyThreads_ProducesNoDuplicates()
    {
        Zid.Initialize(new ZidOptions { MachineId = 1 });
        const int threads = 8, perThread = 20_000;
        var results = new long[threads][];

        Parallel.For(0, threads, t =>
        {
            var local = new long[perThread];
            for (int i = 0; i < perThread; i++) local[i] = Zid.Next().Value;
            results[t] = local;
        });

        var all = results.SelectMany(r => r).ToList();
        Assert.Equal(threads * perThread, all.Distinct().Count());
        Assert.All(results, r => Assert.Equal(r.OrderBy(x => x), r)); // strictly increasing per thread
    }
}
