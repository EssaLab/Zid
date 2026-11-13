namespace EssaLab.Zid.Tests;

/// <summary>
/// <see cref="Zid"/> keeps process-wide static state, so every test class that touches it
/// joins this collection and runs serially (xUnit runs different collections in parallel).
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ZidGlobalState
{
    public const string Name = "Zid global state";
}
