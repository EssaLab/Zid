# 📊 Performance Benchmarks

All performance tests are generated using [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet).

## Environment Info

```text
BenchmarkDotNet v0.15.8, Windows 11 (25H2)
.NET SDK 10.0.101
Host Processor: 13th Gen Intel Core i7-13700HX (24 logical, 16 physical cores)
```

---

## ⚠️ Correction (1.0.5)

Earlier versions of this file reported `Zid.Next()` at "~4.21 ns" and a "Zid_Next_Batch" of 7,214 µs for 100 IDs. These do not match how the generator behaves: the layout allows at most **256 IDs per millisecond per node**, so *sustained* generation cannot average below ~3.9 µs per ID. The "4.21" figure appears to be in µs, not ns. The values below replace them. Please re-run BenchmarkDotNet (`cd src/EssaLab.Zid.Benchmarks && dotnet run -c Release`) on your hardware and update the table before quoting numbers elsewhere.

## ⚡ Sustained generation (Stopwatch, 1,000,000 consecutive `Zid.Next()` calls, single thread)

Measured on Linux (container), .NET 10: **~217k-231k IDs/sec, ~4.3-4.6 µs per ID**, 0 B allocated. This is the real-world ceiling per node/process; scale out with distinct `MachineId`s.

## 📦 String formatting & Base62 encoding (BenchmarkDotNet, i7-13700HX, Windows 11, .NET 10)

| Method | Mean | Error | StdDev | Gen 0 | Allocated |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `Zid.ToShort()` | 33.25 ns | 0.71 ns | 0.65 ns | 0.0030 | 48 B |
| `Zid.TryParse()` | 23.01 ns | 0.26 ns | 0.24 ns | - | 0 B |
| `Guid.NewGuid()` | 36.66 ns | 0.31 ns | 0.29 ns | - | 0 B |

`ToShort()` allocates only the resulting `string`; use `TryFormat` with format `"S"` for a fully allocation-free path.

## 🎯 Key takeaways

1. **Throughput is layout-bound:** 256 IDs/ms/node (~250k/s). It is not a nanosecond-class generator under sustained load.
2. **Allocation-free generation and parsing**; only `ToShort()` allocates its result string.
3. **Time-sortable 64-bit values** keep B-Tree inserts mostly sequential in EF Core.
4. **Scale-out** is done with distinct `MachineId`s (0..1023).
