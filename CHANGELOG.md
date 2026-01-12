# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to Semantic Versioning.

## Unreleased

### Changed
- Upgraded the library and all sample projects to **.NET 10**.
- Upgraded NuGet dependencies and local tooling (CSharpier, Husky.Net, Nerdbank.GitVersioning).

### Fixed
- `IRenderContext` shadow property interop (`shadowOffsetX` / `shadowOffsetY`) now calls the correct JS property names.
- `ICanvas.ToDataUrl()` now returns an actual **data URL** instead of an object URL.
- Fixed per-canvas JS API instantiation issues by using a shared JS API instance (prevents duplicate global event handlers / animation loops).
- Improved `CanvasManager` rendering to avoid relying on dictionary indexer `@ref` patterns.

### Performance
- Reduced JS hot-path overhead in batch processing (`processBatch` loop) and interop argument handling.
- Added allocation-free marshal-reference hashing overloads for common numeric cases (gradients/patterns).

