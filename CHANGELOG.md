# Changelog

## 1.0.0-rc.7

### Performance

- replaced shifting time-series lists with an O(1) circular buffer;
- added dictionary-backed series lookup and direct-series ingestion;
- made trend cursor readout lazy;
- removed LINQ allocations from range calculation and trend rendering;
- cached trace brushes, pens, gauge status brushes and numeric formats;
- coalesced high-frequency automation metadata updates;
- cached unchanged Avalonia automation attached properties;
- cached selector position labels.

### Diagnostics

- benchmark now includes warmup and bytes per operation;
- benchmark compares name lookup with direct-series ingestion;
- added optimized-path regression tests.



### Added

- release metadata through `IndustrialControlsRelease`;
- accessibility names, help text, automation IDs and live-region metadata;
- explicit keyboard contract for `SpringReturnSwitch`;
- shared Industrial90 focus adorner;
- public API and theme-coverage tests;
- bounded-buffer long-acquisition test;
- dependency-free benchmark smoke project;
- NuGet package content validation;
- release and integration documentation.

### Stabilized

- M0–M7 validated control families;
- deterministic capacity limits for time-series controls;
- Industrial90 theme inclusion contract;
- release packaging for `net10.0`.

### Release status

This is a release candidate. The stable `1.0.0` version is issued only after the local M8 gate and final manual validation.
