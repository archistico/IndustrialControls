# Project handoff

## Baseline ufficiale

IndustrialControls.Avalonia `1.0.0` è la baseline funzionale validata: build Release, 167 test, package content e consumer smoke sono passati.

## Candidate corrente

IndustrialControls.Avalonia `1.0.0 Docs1` — screenshot and README documentation update.

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
- contenuto e versione del package validati;
- package consumato da un'applicazione standalone;
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
- package version remains `1.0.0-rc.8`.


## M8 RC6-C candidate

Cumulative candidate based on validated RC6-B:

- marquee source padding is cached and capacity has a single deterministic law;
- strip-chart rendering is decimated to a pixel-oriented budget;
- all strip-chart pens and brushes are reused;
- quality discontinuities remain explicit;
- `MajorGridSeconds` drives the time grid;
- direct-handle ingestion obeys recorder pause state;
- operator controls share an interlocked pseudo-class and amber status
  treatment;
- benchmark coverage includes marquee advancement and 100,000-sample strip
  planning.

The planned next phase after validation is RC6-D, the final API cleanup and
release gate before 1.0.0.


## M8 RC6-C Hotfix 1

Compile-only XML documentation correction:

- replaced unresolved inherited-property
  `<see cref="TimeWindowSeconds"/>` with
  `<c>TimeWindowSeconds</c>`;
- production behavior is unchanged;
- package version remains `1.0.0-rc.8`.


## M8 RC6-C Hotfix 2

The failing quality-decimation assertion expected at least 500 selected points.
The deterministic plan actually contains:

- 499 selected points;
- 496 rendered segments;
- 2 quality breaks (`Bad`, `Unavailable`);
- 1 uncertain point.

The renderer was already preserving the discontinuities. The internal
diagnostic contract now reports quality breaks and uncertain points directly,
and the regression test validates those values. Production rendering is
unchanged.


## M8 RC6-D candidate

Final release-candidate cleanup based on validated RC6-C Hotfix 2:

- removed `StripChartRecorder.PaperSpeed`, which was decorative and did not
  affect geometry;
- the strip-chart header now reports the actual time window and grid interval;
- toggle and rocker controls share internal bistable behavior without changing
  their public contracts;
- automation IDs now collapse separators and provide a deterministic fallback;
- package inspection validates the exact expected version and additional
  release documentation;
- a standalone consumer project restores, builds and runs against the generated
  package;
- PowerShell and CMD gates now end with `M8 RC6-D VALIDATION PASSED`.

After validation, the only planned code change is the stable-version promotion
from `1.0.0-rc.9` to `1.0.0`.


## M8 RC6-D Hotfix 1

Compile-only test correction:

- escaped the XML attribute quotes in the package-consumer source assertion;
- no production, package or release-gate behavior changed;
- package version remains `1.0.0-rc.9`.


## M8 RC6-D Hotfix 2

Test-contract correction:

- the old test required `<c>TimeWindowSeconds</c>`, which had legitimately
  disappeared when the decorative `PaperSpeed` XML comment was removed;
- the test now verifies that no unresolved `cref` exists and that the
  strip-chart header uses the actual `TimeWindowSeconds` and
  `MajorGridSeconds` values;
- production code and package version are unchanged.


## M8 RC6-D Hotfix 3

Package-consumer gate correction:

- the temporary consumer project is generated below the repository and was
  inheriting the parent `Directory.Packages.props`;
- the generated project now sets
  `<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>`;
- its explicit `PackageReference` version therefore validates the exact
  candidate package without conflicting with repository CPM;
- library, public API and package version are unchanged.


## IndustrialControls.Avalonia 1.0.0 stable release candidate

This candidate is a version-promotion-only release based on validated RC6-D
Hotfix 3.

Changes:

- package version changed from `1.0.0-rc.9` to `1.0.0`;
- runtime release metadata changed to `1.0.0`;
- README and package-usage examples target `1.0.0`;
- package, consumer and gate expectations target the stable version;
- gate label changed to `1.0.0 VALIDATION PASSED`;
- stable changelog and release-checklist entries added;
- no functional production behavior changed after the validated RC6-D
  baseline.

The stable baseline becomes official only after the complete local gate and
manual demo validation pass.


## IndustrialControls.Avalonia 1.0.0 Docs1

Documentation-only update based on the validated stable `1.0.0` candidate:

- seven user-provided PNG screenshots copied to the root `screenshot` folder;
- README gallery uses repository-relative image paths;
- images are included in the NuGet package;
- package validation requires all seven entries;
- a stable-release test verifies every README reference, file and PNG
  signature;
- production code and public API are unchanged.

Because the `.nupkg` contents changed, rerun the complete validation gate before
accepting Docs1 as the final archived stable baseline.
