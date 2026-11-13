# EssaLab.Zid

<p align="center">
  <img src="./banner.png" alt="EssaLab.Zid Banner" width="600" />
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/EssaLab.Zid"><img src="https://img.shields.io/nuget/v/EssaLab.Zid.svg?logo=nuget" alt="NuGet Version" /></a>
  <a href="https://www.nuget.org/packages/EssaLab.Zid"><img src="https://img.shields.io/nuget/dt/EssaLab.Zid?logo=nuget&color=blue" alt="NuGet Downloads" /></a>
  <a href="https://github.com/EssaLab/Zid/actions"><img src="https://img.shields.io/github/actions/workflow/status/EssaLab/Zid/ci.yml?branch=main&logo=github" alt="Build Status" /></a>
  <a href="SECURITY.md"><img src="https://img.shields.io/badge/security-policy-brightgreen?logo=shield" alt="Security Policy" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/github/license/EssaLab/Zid?color=blue" alt="License" /></a>
  <a href="https://dotnet.microsoft.com/"><img src="https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet" alt=".NET Version" /></a>
  <a href="https://github.com/EssaLab/Zid/stargazers"><img src="https://img.shields.io/github/stars/EssaLab/Zid?style=social" alt="GitHub Stars" /></a>
</p>

**EssaLab.Zid** is a lock-free distributed ID generator for .NET. Inspired by Twitter's Snowflake algorithm, it generates 64-bit time-sortable unique integers optimized for database indexing efficiency and Base62 URL-friendly short codes.

---

## 📑 Table of Contents

- [🌟 Key Features](#-key-features)
- [📐 Bit Structure Layout](#-bit-structure-layout)
- [📦 Installation](#-installation)
- [🚀 Quick Start](#-quick-start)
- [📊 Benchmarks](BENCHMARKS.md)
- [⚙️ Configuration & Production Checklist](#️-configuration--production-checklist)
- [🤝 Contributing](CONTRIBUTING.md)
- [🔒 Security](SECURITY.md)
- [📜 License](#-license)

---

## 🌟 Key Features

- **⚡ Lock-Free:** Built with `Interlocked.CompareExchange`; thread-safe without locks on the hot path. Throughput is capped at **256 IDs per millisecond per node** (~250k IDs/sec) by the bit layout.
- **📦 Allocation-Free Core:** `Next()`, `TryParse()` and `TryFormat()` (`"S"` format) allocate nothing. `ToShort()` returns a `string`, so it allocates it (48 B).
- **🗄️ Database Friendly:** Generates 64-bit `long` (`BIGINT`) values that are chronologically sortable (Time-sortable), virtually eliminating clustered index fragmentation.
- **🔎 Light Obfuscation & Typo Detection:** Short codes are XOR-masked and carry a 4-bit checksum that catches *accidental* corruption (about 94% of random errors). This is **not security**: the default mask key is public, the checksum is unkeyed, and IDs reveal creation time and node. Never rely on Zid IDs as secrets; always enforce authorization server-side.
- **🧩 Modular & Clean Architecture:** Zero external dependencies in the core package. Optional extensions for ASP.NET Core DI and Entity Framework Core.
- **☁️ Cloud & Container Ready:** Machine ID can come from `ZidOptions.MachineId` or the `ZID_MACHINE_ID` environment variable. Without either, a *random* ID is used, which can collide between replicas (see the production checklist).

---

## 📐 Bit Structure Layout (63-bit)

```text
+--------------------+---------------------+------------------+------------------+
| Timestamp (41 bit) | Machine ID (10 bit) | Sequence (8 bit) | Checksum (4 bit) |
+--------------------+---------------------+------------------+------------------+
```

- **41 bits Timestamp:** ~69 years lifespan from configured Epoch.
- **10 bits Machine ID:** Supports up to 1,024 independent nodes/servers.
- **8 bits Sequence:** 256 IDs per millisecond per node (~250k IDs/sec).
- **4 bits Checksum:** Integrity verification to detect tampering and accidental input corruption.

---

## 📦 Installation

You can install the core package or optional integration packages via NuGet:

```bash
# Core Package (Pure C#, Zero Dependencies)
dotnet add package EssaLab.Zid

# ASP.NET Core Dependency Injection Support
dotnet add package EssaLab.Zid.Extensions.DependencyInjection

# Entity Framework Core Integration
dotnet add package EssaLab.Zid.EntityFramework
```

---

## 🚀 Quick Start

### Standalone Usage

```csharp
using EssaLab.Zid;

// Generate next unique ID
Zid id = Zid.Next();

long dbValue = id.Value;        // 64-bit integer for Database Primary Key
string shortUrl = id.ToShort(); // Base62 string (e.g., "3aB7x9L2kQ1")

// Extract metadata
DateTimeOffset createdAt = id.Timestamp;
long machineId = id.MachineId;
```

> 📖 For ASP.NET Core and EF Core integration, see the package READMEs: [Dependency Injection](src/EssaLab.Zid.Extensions.DependencyInjection/README.md) and [Entity Framework](src/EssaLab.Zid.EntityFramework/README.md).

---

## ⚙️ Configuration & Production Checklist

```csharp
Zid.Initialize(new ZidOptions
{
    MachineId = 12,                                   // 0..1023, unique per running instance
    Epoch = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
    MaskKey = 0x5D3759DF12345678                      // only affects Base62 short codes
});
```

- **Initialize before first use.** `Zid.Initialize` (or `AddZid`) must run before the first `Zid.Next()`. After IDs are issued the configuration is frozen: identical settings are a no-op, conflicting settings throw `InvalidOperationException`.
- **Set `MachineId` explicitly in production.** Every concurrently running instance (pod, VM, process) needs a different value in 0..1023. The automatic fallback is random and collides often: in a test with 10 simultaneously started instances, duplicates appeared in ~30% of runs, and with 40 instances in ~95%. Use `ZidOptions.MachineId` or `ZID_MACHINE_ID` (an invalid value throws instead of being ignored).
- **Never change `Epoch`, `MachineId` assignment scheme or `MaskKey`** after IDs or short codes have been issued.
- **Options are validated:** `MachineId` outside 0..1023 or an `Epoch` in the future/before 1970 throws `ArgumentOutOfRangeException`.
- **Keep clocks sane (NTP).** If the system clock moves backwards, generation waits for it to catch up.

---

## 📊 Benchmarks

Sustained single-thread generation is bounded by the 256 IDs/ms layout limit, i.e. roughly **4-5 µs per ID (~220k IDs/sec)**. See [BENCHMARKS.md](BENCHMARKS.md) for methodology and caveats.

| Method | Mean | Allocated |
| :--- | :--- | :--- |
| `Zid.Next()` (sustained, 1M calls) | ~4.3 µs | 0 B |
| `Zid.ToShort()` | ~33 ns | 48 B |
| `Zid.TryParse()` | ~23 ns | 0 B |
| `Guid.NewGuid()` | ~37 ns | 0 B |

---

## 🤝 Community & Support

- 🐛 **Found a bug or have an idea?** [Open an issue](https://github.com/EssaLab/Zid/issues).
- 🗺️ **What's next?** See the [Roadmap](ROADMAP.md).
- 🛠️ **Want to contribute?** Check out our [Contributing Guidelines](CONTRIBUTING.md).

---

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.