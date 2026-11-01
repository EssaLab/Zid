# 📊 Performance Benchmarks

All performance tests are generated using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet).

## Environment Info

```text
BenchmarkDotNet v0.15.8, Windows 11 (25H2)
.NET SDK 10.0.101
Host Processor: 13th Gen Intel Core i7-13700HX (24 logical, 16 physical cores)
```

---

## ⚡ Generation Speed (Single Call / Micro-Benchmark)

Comparison between `Zid.Next()` and standard .NET ID generators:

| Method | Mean | Error | StdDev | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **`Zid.Next()`** | **~4.21 ns** | - | - | - | **0 B** |
| `Guid.NewGuid()` | 36.66 ns | 0.31 ns | 0.29 ns | - | 0 B |

---

## 📦 String Formatting & Base62 Encoding

Zero heap allocation string formatting and parsing benchmark:

| Method | Mean | Error | StdDev | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **`Zid.ToShort()`** | **33.25 ns** | 0.71 ns | 0.65 ns | 0.0030 | 48 B |
| **`Zid.TryParse()`** | **23.01 ns** | 0.26 ns | 0.24 ns | - | **0 B** |

---

## 🔄 Multi-Threaded & Batch Workload Analysis

Evaluating concurrency and lock-free thread safety under parallel execution (Batch Size = 100):

| Method | Mean | Allocated | Notes |
| :--- | :--- | :--- | :--- |
| `Standard_Guid_Batch` | 10.72 μs | 4.96 KB | Independent random GUID generation |
| **`Zid_Next_Batch`** | **7,214.84 μs** | 5.10 KB | Lock-free sequence management (Protected by 256 IDs/ms node threshold) |

---

## 🎯 Key Takeaways

1. **Zero Allocations & High Speed:** `Zid.Next()` executes in single-digit nanoseconds with zero heap allocation (`0 B`) in single-thread scenarios.
2. **Lock-Free Thread Safety:** Built with `Interlocked.CompareExchange` and `SpinWait` to handle concurrent thread requests safely without deadlocks.
3. **Database Efficiency:** 64-bit time-sortable integer layout prevents B-Tree index fragmentation in Entity Framework Core.
4. **Predictable Sequence Limits:** Enforces a strict 256 IDs/millisecond per node rate limit to guarantee chronological uniqueness. For extreme multi-node distributed environments, scale-out is easily managed via `MachineId`.