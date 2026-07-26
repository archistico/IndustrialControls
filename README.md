# IndustrialControls.Avalonia

Reusable Avalonia control library inspired by 1990s industrial control panels.

## Release candidate

Current version: **1.0.0-rc.2**

Validated baseline before this candidate: **M7 Hotfix 3 / 0.7.3**

M8 adds:

- stabilized public API contract;
- accessibility metadata;
- keyboard and focus contracts;
- theme coverage tests;
- bounded-buffer long-run verification;
- benchmark smoke suite;
- NuGet package generation and inspection;
- release documentation.

## Validate

```powershell
.\scripts\validate.ps1
```

The script restores, builds, tests, packs and inspects the NuGet package.

## Run the demo

```powershell
dotnet run --project src/IndustrialControls.Avalonia.Demo
```

## Run benchmarks

```powershell
.\scripts\benchmark.ps1
```

## Package integration

See `docs/PACKAGE_USAGE.md`.

## Status

M0–M7 Hotfix 3: **VALIDATED**

M8 RC2: **CANDIDATE**


## M8 RC1 Hotfix 1

- corretto il riferimento a `Avalonia.Media.Colors.Green` nel test long-run;
- usato `global::Avalonia.Media.Colors.Green` per evitare la risoluzione relativa
  verso `IndustrialControls.Avalonia.Media`;
- nessuna modifica alla libreria, alla demo, alle API o al pacchetto;
- versione NuGet invariata: `1.0.0-rc.2`.


## RC2 allocation optimization

RC2 keeps the public API and visual language intact while optimizing:

- trend ingestion;
- bounded sample storage;
- cursor readout generation;
- gauge updates;
- selector transitions;
- automation metadata assignments.

For high-frequency acquisition, retain the `SignalTraceSeries` returned by `AddSeries` and use the direct `AddSample(series, ...)` overload.
