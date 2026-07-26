# Project handoff

## Baseline ufficiale

M7 Hotfix 3 / versione `0.7.3` è la baseline validata.

## Candidate corrente

M8 RC6-B — Avalonia Property & Reactive Contracts / versione `1.0.0-rc.7`.

## Contenuto

- contratto API pubblico;
- `IndustrialControlsRelease`;
- metadati Avalonia Automation;
- live region per annunciatori;
- keyboard contract completo;
- focus visuale condiviso;
- theme-coverage gate;
- bounded-buffer long-run gate;
- benchmark console;
- pack NuGet e validazione del contenuto;
- documentazione release.

## Gate

```powershell
.\scripts\validate.ps1
```

Risultati richiesti:

- build Release senza warning;
- suite completa superata;
- package `.nupkg` e `.snupkg` generati;
- contenuto del package validato;
- demo verificata manualmente;
- navigazione da tastiera verificata.

## Dopo la validazione

La release stabile richiede soltanto:

- versione `1.0.0`;
- changelog finale;
- nuova esecuzione completa del gate;
- ZIP completo archiviato come baseline.

## Consegna

Ogni consegna deve essere uno ZIP completo dell'intero progetto pronto per compilazione e test.


## M8 RC1 Hotfix 1

Correzione di compilazione esclusivamente nel progetto di test:

- il riferimento `Avalonia.Media.Colors.Green` era risolto come
  `IndustrialControls.Avalonia.Media` a causa del namespace del test;
- il test usa ora il qualificatore globale
  `global::Avalonia.Media.Colors.Green`;
- la libreria e il contratto release restano invariati.


## M8 RC2

Optimization candidate based on RC1 Hotfix 1:

- no public API removals or renames;
- time-series storage is now a circular buffer;
- series lookup is dictionary-backed;
- direct-series ingestion is available;
- trend cursor text is generated only when read;
- gauge and selector accessibility updates are coalesced;
- stable brushes, pens, formats and selector labels are cached;
- benchmark output includes bytes per operation.

Validation requires the complete release gate and a new benchmark run on the same machine used for RC1.


## M8 RC3

Fixes the demo regression found after RC2:

- the focus adorner now uses Avalonia's `FocusAdornerTemplate`;
- startup failures are written to
  `%LOCALAPPDATA%\IndustrialControls.Avalonia.Demo\startup-error.log`;
- main-window construction failures show a fallback diagnostic window;
- the demo is a complete catalog with seven functional tabs;
- all high-level public controls from M0 through M7 are represented;
- dynamic M6 and M7 interaction examples are preserved.


## M8 RC4

Visual corrections requested from the complete demo catalog:

- `LedMarqueeDisplay` automatically fits its visible character count to the
  arranged width;
- a full blank viewport precedes every message cycle;
- resizing restarts the message from the real right edge;
- `AlarmAnnunciator` uses a neutral industrial enclosure;
- the legacy alarm lens is rendered with concentric `Ellipse` elements;
- priority colors are advisory blue, caution yellow, warning amber and
  critical red;
- clickable operator controls use the hand cursor.


## M8 RC4 Hotfix 1

Compile-only test correction:

- XML fragments embedded in C# assertions now escape internal double quotes;
- the corrected assertions cover the circular legacy lens, removed brown
  palette and hand-cursor theme setters;
- production code and visual output are unchanged.


## M8 RC5

The RC4 palette tests failed because `SolidColorBrush` is an Avalonia object
owned by a dispatcher thread. RC5 adds `AlarmAnnunciator.PriorityColor`, a
plain `Color` value suitable for tests and non-rendering logic. The existing
`PriorityBrush` and visual output remain unchanged.


## Documentation update after RC5 validation

The root README is now a user-facing integration guide. Historical milestone
status was removed from the README and remains available only in roadmap,
handoff and changelog documents.

The README now documents:

- package and project-reference installation;
- theme registration;
- every public control family;
- AXAML and C# examples;
- MVVM and binding;
- alarm sequencing;
- high-frequency trend acquisition;
- accessibility;
- keyboard interaction;
- performance;
- customization;
- demo and validation commands.


## M8 RC6-A candidate

Functional-safety candidate based on the validated RC5 baseline:

- gauge evaluation may use a derived engineering value;
- `DeviationGauge` uses deadband-adjusted deviation for scale and status;
- legacy alarm annunciators now retain transient alarms;
- spring-return input uses pointer capture;
- binding-driven interlock changes return the command to center;
- illuminated push buttons expose and enforce interlock state.

The next planned phase after validation is RC6-B, covering Avalonia property
coercion and reactive notification contracts.


## M8 RC6-A Hotfix 1

Compile-only correction:

- `SpringReturnSwitch` and `IlluminatedPushButton` now import
  `Avalonia.Styling`;
- this enables the `PseudoClasses.Set(...)` extension method;
- functional behavior and package version remain unchanged.


## M8 RC6-A Hotfix 2

Definitive pseudo-class compile correction:

- replaced `PseudoClasses.Set(name, bool)` with direct
  `IPseudoClasses.Add(name)` / `Remove(name)` calls;
- removed the incorrect `Avalonia.Styling` import;
- added a source-level regression test for both affected controls.


## M8 RC6-B candidate

Cumulative candidate based on RC6-A Hotfix 2:

- dynamic capacities and ranges are enforced through Avalonia-property
  handlers rather than CLR setters;
- `SetCurrentValue` preserves bindings and styles during normalization;
- alarm summary counts are observable;
- cosmetic alarm text changes preserve blink phase;
- cursor readout tracks time-window changes;
- accessibility metadata has a synchronous fallback;
- validation scripts fail on native command errors and use the Microsoft
  Testing Platform `--project` syntax.

After validation, the planned next phase is RC6-C rendering and performance
hardening.


## M8 RC6-B Hotfix 1

Compile-only test correction:

- escaped the trailing backslash in the validation-script regression test;
- production code and validation scripts are unchanged;
- package version remains `1.0.0-rc.7`.
