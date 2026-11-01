using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EssaLab.Zid.EntityFramework;

/// <summary>
/// Provides value comparison logic for <see cref="Zid"/> to assist EF Core change tracking.
/// </summary>
public class ZidValueComparer : ValueComparer<Zid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZidValueComparer"/> class.
    /// </summary>
    public ZidValueComparer() : base(
        (z1, z2) => z1.Value == z2.Value,
        z => z.Value.GetHashCode(),
        z => z.Value == 0 ? default : new Zid(z.Value))
    { }
}