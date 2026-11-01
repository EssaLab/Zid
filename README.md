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

**EssaLab.Zid** is an enterprise-grade, lock-free, zero-allocation distributed ID generator for .NET. Inspired by Twitter's Snowflake algorithm, it generates 64-bit time-sortable unique integers optimized for maximum database indexing efficiency, ultra-high throughput, and Base62 URL safety.

---

## 📑 Table of Contents

- [🌟 Key Features](#-key-features)
- [📐 Bit Structure Layout](#-bit-structure-layout)
- [📦 Installation](#-installation)
- [🚀 Quick Start](#-quick-start)
- [📊 Benchmarks](BENCHMARKS.md)
- [⚙️ Configuration Options](#️-configuration-options)
- [🤝 Contributing](CONTRIBUTING.md)
- [🔒 Security](SECURITY.md)
- [📜 License](#-license)

---

## 🌟 Key Features

- **⚡ Lock-Free & Ultra High-Performance:** Built with `Interlocked.CompareExchange` to handle millions of concurrent ID generations without thread contention.
- **📦 Zero Heap Allocation:** Native `readonly struct` utilizing `Span<char>` and `stackalloc` for string formatting and parsing with zero GC pressure.
- **🗄️ Database Friendly:** Generates 64-bit `long` (`BIGINT`) values that are chronologically sortable (Time-sortable), virtually eliminating clustered index fragmentation.
- **🔒 Security & Obfuscation:** Built-in XOR masking and 4-bit integrity checksum to prevent sequence prediction and link tampering.
- **🧩 Modular & Clean Architecture:** Zero external dependencies in the core package. Optional extensions for ASP.NET Core DI and Entity Framework Core.
- **☁️ Cloud & Container Ready:** Automatically resolves Machine IDs via environment variables (`ZID_MACHINE_ID`) or local entropy.

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

> 📖 For full usage guides including ASP.NET Core and EF Core integration, see the [Documentation Guide](DOCS.md).

---

## 📊 Benchmarks

We run detailed performance benchmarks on every release. Detailed results can be found in [BENCHMARKS.md](BENCHMARKS.md).

| Method | Mean | StdDev | Allocated |
| :--- | :--- | :--- | :--- |
| **Zid.Next()** | **4.2 ns** | 0.02 ns | **0 B** |
| **Zid.ToShort()** | **12.8 ns** | 0.07 ns | **0 B** |
| **Guid.NewGuid()** | **35.4 ns** | 0.19 ns | **0 B** |

---

## 🤝 Community & Support

- 🐛 **Found a bug?** Open an issue using our [Bug Report Template](.github/ISSUE_TEMPLATE/bug_report.md).
- 💡 **Have a feature idea?** Submit a [Feature Request](.github/ISSUE_TEMPLATE/feature_request.md).
- 🛠️ **Want to contribute?** Check out our [Contributing Guidelines](CONTRIBUTING.md).

---

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.