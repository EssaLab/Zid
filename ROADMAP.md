# Roadmap

Planned order of work for the EssaLab.Zid ecosystem, based on a full review of the core, EF Core and DI packages.

## 1.0.5 - Patch: correctness & honest docs  (this release)
- [x] Fix `Initialize`/`AddZid` ignoring options
- [x] Validate `MachineId` (0..1023) and `Epoch`; fix infinite spin on future epoch
- [x] Fail loudly on invalid `ZID_MACHINE_ID`
- [x] Correct benchmark numbers, security and zero-allocation claims; fix dead doc links
- [x] CI workflow, tag/version guard in publish workflow, concurrency + configuration tests

## 1.1 - Minor: production readiness & DX
- [ ] `IZidGenerator` (instance-based) + `TimeProvider`; `Zid.Next()` becomes a facade
- [ ] `IZidMachineIdProvider` with strategies: Fixed, EnvVar, Hostname-hash, Kubernetes StatefulSet ordinal, Redis/SQL lease; optional `RequireExplicitMachineId`
- [ ] `ClockBackwardsPolicy` (Wait / Throw / Logical) and `SpinOnce(sleep1Threshold: -1)` to avoid sleep latency spikes
- [ ] `System.Text.Json` converter (string by default, JS-safe) and `TypeConverter`
- [ ] EF Core: single `UseZid()` entry point, `IModelFinalizingConvention`, drop hard-coded `bigint`, skip PK+FK shared keys, safe handling of legacy/invalid values (`Zid.TryCreate`, `FromRaw(validate:false)`)
- [ ] DI: `IOptions<ZidOptions>`, `AddZid(IConfiguration)`, `ValidateOnStart`, register `IZidGenerator`
- [ ] Multi-target `net8.0;net9.0;net10.0` (+ matching EF Core versions)
- [ ] SourceLink, snupkg, deterministic builds, package validation, MinVer; EF/DI tests (SQLite in-memory)

## 2.0 - Major: ecosystem
- [ ] Rename root namespace to avoid the `EssaLab.Zid` namespace / `Zid` type clash
- [ ] `EssaLab.Zid.AspNetCore`: model binder, route constraint `{id:zid}`, OpenAPI schema
- [ ] Configurable bit layout presets (e.g. checksum outside the 64 bits for 4096 IDs/ms)
- [ ] Strongly-typed IDs via source generator (`[ZidId] partial struct UserId;`)
- [ ] Metrics (`System.Diagnostics.Metrics`) and `IHealthCheck`
- [ ] Dapper TypeHandler, MongoDB serializer; optional keyed public-ID encoding and sortable fixed-width format
