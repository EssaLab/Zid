using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EssaLab.Zid.EntityFramework;

/// <summary>
/// EF Core Value Converter to store <see cref="Zid"/> as a 64-bit integer (<c>BIGINT</c>) in the database
/// and convert it back automatically when reading.
/// </summary>
public class ZidValueConverter : ValueConverter<Zid, long>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZidValueConverter"/> class.
    /// </summary>
    public ZidValueConverter() : base(
        zid => zid.Value,
        value => new Zid(value))
    { }
}

/// <summary>
/// Provides extension methods for <see cref="ModelBuilder"/> to automatically apply <see cref="Zid"/> configurations.
/// </summary>
public static class ZidModelBuilderExtensions
{
    /// <summary>
    /// Automatically applies the <see cref="ZidValueConverter"/> to all <see cref="Zid"/> and nullable <see cref="Zid"/> 
    /// properties across all entities in the <see cref="DbContext"/> without manually configuring each entity.
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
                if (property.ClrType == typeof(Zid) || property.ClrType == typeof(Zid?))
                {
                    property.SetValueConverter(converter);
                    
                    // Explicitly set database column type to BIGINT for non-nullable Zid properties
                    if (property.ClrType == typeof(Zid))
                    {
                        property.SetColumnType("bigint");
                    }
                }
            }
        }

        return modelBuilder;
    }
}