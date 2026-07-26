# Changelog

## 1.0.0-rc.9 Hotfix 3

### Package-consumer gate

- disabled inherited Central Package Management in the generated standalone
  consumer project;
- the smoke project can now declare the exact candidate package version;
- library code, public API and package contents are unchanged.


## 1.0.0-rc.9 Hotfix 2

### Test contract

- removed the obsolete requirement for a `<c>TimeWindowSeconds</c>` XML-doc
  fragment;
- the strip-chart source test now checks the actual window and grid values used
  by the final header;
- production code and package contents are unchanged.


## 1.0.0-rc.9 Hotfix 1

### Test compilation

- escaped the XML attribute quotes in the package-consumer release-gate
  assertion;
- production code, package contents and release scripts are unchanged.


## 1.0.0-rc.9

### API cleanup

- removed release-candidate-only `StripChartRecorder.PaperSpeed`;
- strip-chart status now reports the actual time window and grid interval;
- retained the public toggle and rocker APIs while moving shared behavior to
  an internal helper;
- hardened automation-ID normalization and fallback behavior.

### Release gate

- package validation now targets the exact project version;
- package validation checks additional user and release documentation;
- added a standalone package-consumer restore, build and run gate;
- corrected the final validation label to `M8 RC6-D VALIDATION PASSED`.

### Documentation

- aligned README, package usage, public API, catalog, architecture, roadmap,
  handoff and release checklist with the final candidate.


## 1.0.0-rc.8 Hotfix 2

### Diagnostics

- strip-chart diagnostics now expose `QualityBreakCount`;
- strip-chart diagnostics now expose `UncertainPointCount`;
- the quality-decimation test verifies the deterministic 499-point,
  496-segment, two-break render plan;
- rendering behavior is unchanged.


## 1.0.0-rc.8

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
