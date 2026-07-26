# IndustrialControls.Avalonia

Reusable Avalonia control library inspired by 1990s industrial control panels.

## Release candidate

Current version: **1.0.0-rc.5**

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

M8 RC5: **CANDIDATE**


## M8 RC1 Hotfix 1

- corretto il riferimento a `Avalonia.Media.Colors.Green` nel test long-run;
- usato `global::Avalonia.Media.Colors.Green` per evitare la risoluzione relativa
  verso `IndustrialControls.Avalonia.Media`;
- nessuna modifica alla libreria, alla demo, alle API o al pacchetto;
- versione NuGet invariata: `1.0.0-rc.5`.


## RC2 allocation optimization

RC2 keeps the public API and visual language intact while optimizing:

- trend ingestion;
- bounded sample storage;
- cursor readout generation;
- gauge updates;
- selector transitions;
- automation metadata assignments.

For high-frequency acquisition, retain the `SignalTraceSeries` returned by `AddSeries` and use the direct `AddSample(series, ...)` overload.


## RC3 demo catalog and startup hardening

- corrected the focus-adornment template contract;
- added a startup diagnostic log under the user's local application data;
- added a fallback diagnostic window for main-window construction errors;
- rebuilt the demo as a complete seven-tab catalog containing every public
  high-level control created in M0–M7.


## RC4 lamps and LED refinement

- responsive marquee with true edge entry after window resizing;
- neutral black/grey legacy annunciators;
- circular priority lenses;
- blue, yellow, amber and red priority palette;
- hand cursor on clickable industrial controls.


## M8 RC4 Hotfix 1

- corrected four malformed XML-fragment string literals in
  `M8LampsAndLedVisualContractTests`;
- escaped the quotes used by the `Ellipse`, palette and cursor assertions;
- no library, theme, demo, API or package behavior changed;
- NuGet version remains `1.0.0-rc.5`.


## RC5 dispatcher-independent alarm palette contract

`AlarmAnnunciator.PriorityColor` exposes the selected priority color as an
Avalonia `Color` value. Tests and non-rendering logic no longer need to inspect
the thread-affine `SolidColorBrush` used by the control template.
