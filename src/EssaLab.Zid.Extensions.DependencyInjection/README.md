# EssaLab.Zid.Extensions.DependencyInjection

Seamless Dependency Injection extensions for configuring and integrating `EssaLab.Zid` into ASP.NET Core applications and service containers.

## 📦 Installation

```bash
dotnet add package EssaLab.Zid.Extensions.DependencyInjection
```

## 💻 Quick Start

Register `Zid` services in your `Program.cs`:

```csharp
using EssaLab.Zid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add Zid with custom node/machine options
builder.Services.AddZid(options =>
{
    options.MachineId = 1; // Configure node identifier for distributed environments
});

var app = builder.Build();
```

## 📄 License

This project is licensed under the [MIT License](LICENSE).