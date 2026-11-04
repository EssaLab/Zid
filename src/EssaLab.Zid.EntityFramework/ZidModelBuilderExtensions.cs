using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EssaLab.Zid.EntityFramework;

/// <summary>
/// Provides extension methods for <see cref="ModelBuilder"/> to automatically apply <see cref="Zid"/> configurations.
/// </summary>
public static class ZidModelBuilderExtensions
{
    /// <summary>
    /// Automatically sets column types to <c>bigint</c> and configures automated client-side 
    /// ID generation for primary keys across all entities in the <see cref="DbContext"/>.
    /// </summary>
    /// <param name="modelBuilder">The EF Core <see cref="ModelBuilder"/> instance.</param>
    /// <returns>The same <see cref="ModelBuilder"/> instance for method chaining.</returns>
    public static ModelBuilder ApplyZidConventions(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                // Support both standard Zid and nullable Zid properties
                if (property.ClrType != typeof(Zid) && property.ClrType != typeof(Zid?)) 
                    continue;
                
                // Set the database column type to bigint for both types
                property.SetColumnType("bigint");
                        
                // Skip value generation setup if the property is not a primary key
                if (!property.IsPrimaryKey()) 
                    continue;
                
                // Enable client-side value generation using ZidValueGenerator
                property.SetValueGeneratorFactory((_, _) => new ZidValueGenerator());
                property.ValueGenerated = ValueGenerated.OnAdd;

                // Prevent database providers (like SQL Server) from configuring the column as an identity (IDENTITY(1,1))
                property.SetAnnotation("Relational:ValueGenerationStrategy", null);
                property.SetAnnotation("SqlServer:ValueGenerationStrategy", null);
            }
        }

        return modelBuilder;
    }
}