# Project handoff

## Baseline ufficiale

M7 Hotfix 3 / versione `0.7.3` è la baseline validata.

## Candidate corrente

M8 RC5 — dispatcher-independent alarm palette / versione `1.0.0-rc.5`.

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
