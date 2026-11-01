# EssaLab.Zid

A high-performance, zero-allocation, distributed 64-bit ID generator (Snowflake-based) for .NET. Designed for high concurrency, database index optimization, and clean architectural purity.

## 🚀 Features

- **Lightning Fast:** Single-call generation in single-digit nanoseconds with **0 B heap allocation**.
- **Time-Sortable:** Naturally ordered chronologically to prevent B-Tree index fragmentation in databases (Entity Framework Core friendly).
- **Compact & Efficient:** 64-bit integer (`BIGINT`) storage layout.
- **Base62 Encoding:** Built-in zero-allocation short string formatting and parsing (`ToShort()` / `TryParse()`).

## 📦 Installation

```bash
dotnet add package EssaLab.Zid
```

## 💻 Quick Start

```csharp
using EssaLab.Zid;

// Generate a new unique ID
Zid id = Zid.Next();

// Get raw 64-bit value for database storage
long rawValue = id.Value;

// Convert to short Base62 string for URLs
string shortCode = id.ToShort();

// Parse back from short string
if (Zid.TryParse(shortCode, out Zid parsedId))
{
    // Success
}
```

## 📄 License

This project is licensed under the [MIT License](LICENSE).