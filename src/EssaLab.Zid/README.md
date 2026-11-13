# EssaLab.Zid

A lock-free, distributed 64-bit ID generator (Snowflake-based) for .NET. Designed for high concurrency and database index friendliness.

## 🚀 Features

- **Lock-Free & Allocation-Free Generation:** `Zid.Next()` allocates nothing. Sustained throughput is up to **256 IDs/ms per node** (~220k IDs/sec).
- **Time-Sortable:** Naturally ordered chronologically to prevent B-Tree index fragmentation in databases (Entity Framework Core friendly).
- **Compact & Efficient:** 64-bit integer (`BIGINT`) storage layout.
- **Base62 Short Codes:** Built-in formatting and parsing (`ToShort()` allocates its result string; `TryParse()` and `TryFormat("S")` do not). Light obfuscation and typo detection only, **not a security feature**.

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