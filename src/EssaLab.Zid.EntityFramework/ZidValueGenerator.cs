using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace EssaLab.Zid.EntityFramework;

/// <summary>
/// Generates new <see cref="Zid"/> primary keys on the client-side seamlessly during entity insertion.
/// </summary>
public class ZidValueGenerator : ValueGenerator<Zid>
{
    /// <summary>
    /// Gets a value indicating whether the values generated are temporary. 
    /// Returns <c>false</c> to instruct EF Core that this value is final and stable in memory.
    /// </summary>
    public override bool GeneratesTemporaryValues => false;

    /// <summary>
    /// Generates a new <see cref="Zid"/> value for the specified entry.
    /// </summary>
    /// <param name="entry">The change tracking entry for the entity.</param>
    /// .<returns>A newly generated <see cref="Zid"/>.</returns>
    public override Zid Next(EntityEntry entry) => Zid.Next();
}