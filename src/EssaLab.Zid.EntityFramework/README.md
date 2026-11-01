# EssaLab.Zid.EntityFramework

Entity Framework Core integration package for `EssaLab.Zid`, providing built-in Value Converters for seamless mapping between `Zid` structs and database `BIGINT` columns.

## 📦 Installation

```bash
dotnet add package EssaLab.Zid.EntityFramework
```

## 💻 Quick Start

Configure your `DbContext` using both pre-convention global registration and model conventions:
```csharp
using EssaLab.Zid.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    // 1. Register Zid globally before model discovery to prevent validation errors
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.AddZid();
    }

    // 2. Apply database column types and automated client-side primary key generation
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyZidConventions();
    }
}
```

## 📄 License

This project is licensed under the [MIT License](LICENSE).