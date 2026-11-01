# Changelog

All notable changes to **EssaLab.Zid** will be documented in this file.

The format is based on [Keep a Changelog](https.keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2026-10-30

### Added
- **Core Package (`EssaLab.Zid`):**
  - Initial public release of `Zid` - Enterprise-Grade Distributed ID Generator.
  - Implemented 64-bit ID layout: `[41-bit Time] [10-bit Machine] [8-bit Sequence] [4-bit Checksum]`.
  - Added Lock-Free thread synchronization using `Interlocked.CompareExchange` for maximum generation speed.
  - Implemented `ISpanFormattable` and `ISpanParsable<Zid>` for zero-allocation Base62 encoding and decoding.
  - Added property extractors for `Timestamp`, `MachineId`, `Sequence`, and `Checksum`.
  - Added XOR masking configuration to protect IDs against reverse engineering.

- **Dependency Injection Package (`EssaLab.Zid.Extensions.DependencyInjection`):**
  - Added `AddZid()` extension method for `IServiceCollection` to configure startup options (`Epoch`, `MachineId`, `MaskKey`).

- **Entity Framework Core Package (`EssaLab.Zid.EntityFramework`):**
  - Added `ZidValueConverter` for seamless conversion between `Zid` struct and database `BIGINT`.
  - Added `ApplyZidConventions()` extension method for `ModelBuilder` to automatically map all `Zid` entity properties.

## [1.0.1] - 2026-11-1

### Fixed / Changed
- **Entity Framework Core Package (`EssaLab.Zid.EntityFramework`):**
  - Added custom `ZidValueGenerator` with `GeneratesTemporaryValues = false` to prevent tracking collisions and identity errors when inserting entities.
  - Automated primary key value generator factory assignment inside `ApplyZidConventions()` so `Zid.Next()` is called automatically for primary keys without manual entity configuration.

## [1.0.2] - 2026-11-01

### Added / Improved
- **Entity Framework Core Package (`EssaLab.Zid.EntityFramework`):**
  - Added `AddZid()` extension method for `ModelConfigurationBuilder` to enable global pre-convention registration of `ZidValueConverter` and `ZidValueComparer`.
  - Added `ZidValueComparer` to optimize entity tracking and prevent identity conflicts for the `Zid` struct.
  - Enhanced `ZidValueConverter` with safe default/zero value handling.
  - Updated documentation and quick-start examples in `README.md`.

## [1.0.3] - 2026-11-01

### Changed / Unified
- **Global Ecosystem Configuration:**
  - Migrated versioning architecture to a unified system using `Directory.Build.props` across all packages (`EssaLab.Zid`, `EssaLab.Zid.EntityFramework`, and `EssaLab.Zid.Extensions.DependencyInjection`).
  - Standardized package metadata, logos, MIT licensing, and repository tracking configurations.
  - Aligned all package dependencies to firmly target synchronized ecosystem version releases, eliminating potential runtime version mismatch bugs.
