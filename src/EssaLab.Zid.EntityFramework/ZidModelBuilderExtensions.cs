using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EssaLab.Zid.EntityFramework;

/// <summary>
/// Provides extension methods for <see cref="ModelBuilder"/> to automatically apply <see cref="Zid"/> configurations.
/// </summary>
public static class ZidModelBuilderExtensions
{
    /// <summary>
    /// Automatically applies the <see cref="ZidValueConverter"/>, sets column types to <c>bigint</c>, 
    /// and configures automated client-side ID generation for primary keys across all entities in the <see cref="DbContext"/>.
    /// </summary>
    /// <param name="modelBuilder">The EF Core <see cref="ModelBuilder"/> instance.</param>
    /// <returns>The same <see cref="ModelBuilder"/> instance for method chaining.</returns>
    public static ModelBuilder ApplyZidConventions(this ModelBuilder modelBuilder)
    {
        var converter = new ZidValueConverter();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType != typeof(Zid) && property.ClrType != typeof(Zid?)) continue;
                property.SetValueConverter(converter);
                    
                // Explicitly set database column type to BIGINT for non-nullable Zid properties
                if (property.ClrType != typeof(Zid)) continue;
                property.SetColumnType("bigint");
                        
                // Automate key generation if it's a primary key
                if (!property.IsPrimaryKey()) continue;
                property.SetValueGeneratorFactory((_, _) => new ZidValueGenerator());
                property.ValueGenerated = ValueGenerated.OnAdd;
            }
        }

        return modelBuilder;
    }
}