# EssaLab.Zid.EntityFramework

Entity Framework Core integration package for `EssaLab.Zid`, providing built-in Value Converters for seamless mapping between `Zid` structs and database `BIGINT` columns.

## 📦 Installation

```bash
dotnet add package EssaLab.Zid.EntityFramework
```

## 💻 Quick Start

Configure your `DbContext` using the provided EF Core extensions:

```csharp
using EssaLab.Zid.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically configure Zid value conversions across all entity properties
        modelBuilder.ApplyZidConventions();
    }
}
```

## 📄 License

This project is licensed under the [MIT License](LICENSE).