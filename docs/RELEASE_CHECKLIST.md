# IndustrialControls.Avalonia 1.0.0 release checklist

## Automated stable-release gate

Run:

```powershell
.\scripts\validate.ps1
```

The gate must complete every step:

- clean restore;
- Release build with warnings treated as errors;
- complete Microsoft Testing Platform suite;
- NuGet and symbol-package creation;
- exact package-version and package-content inspection;
- standalone consumer restore from the generated `.nupkg`;
- standalone consumer build and execution.

Expected final lines for the stable release:

```text
PACKAGE CONTENT PASSED: IndustrialControls.Avalonia.1.0.0.nupkg
PACKAGE CONSUMER PASSED: 1.0.0
1.0.0 VALIDATION PASSED
```

## Performance gate

Run:

```powershell
.\scripts\benchmark.ps1
```

Record:

- name-lookup and direct-handle trend timings;
- gauge allocations per operation;
- selector allocations per operation;
- marquee source rebuild count during measurement;
- strip-chart selected-point and segment counts;
- strip-chart render-plan timing and allocations.

The marquee must report zero source rebuilds during the measured advance loop.
The 100,000-sample strip-chart scenario must remain decimated near the plot
pixel budget.

## Manual demo gate

Run:

```powershell
dotnet run --project .\src\IndustrialControls.Avalonia.Demo\
```

Verify all seven catalog tabs.

### Foundation and static controls

- panel depth and borders are readable;
- bezels, engraved labels, safety placards and data plates render correctly;
- resizing does not clip essential text.

### Lamps, LED and annunciators

- marquee text enters from the right edge;
- resizing recalculates the visible capacity;
- legacy alarm lenses remain circular;
- alarm priority colors and state text are distinguishable;
- ACK, return and RESET sequences remain correct.

### Gauges

- radial needles remain centered;
- linear, digital and deviation gauges show coherent ranges;
- `DeviationGauge` uses `Value - Setpoint`;
- the deadband centers small deviations without changing the raw displayed
  deviation.

### Operator controls

- every clickable control shows the hand cursor;
- keyboard focus is visible;
- interlocked controls use the common amber treatment;
- commands do not execute while interlocked;
- the spring-return switch returns to center after releasing outside the
  control.

### Trends and screens

- trend cursor line and text remain synchronized;
- strip-chart pause blocks both name-based and direct-handle acquisition;
- strip-chart header shows the time window and grid interval;
- bad and unavailable quality create visible discontinuities;
- oscilloscope capacity changes trim existing samples.

## Separate application integration gate

In a separate Avalonia application:

1. add the local package source;
2. install `IndustrialControls.Avalonia` version `1.0.0`;
3. add `IndustrialControlsTheme` after the application theme;
4. render at least one gauge, one operator control and one alarm indicator;
5. confirm keyboard focus and automation metadata;
6. confirm no project reference to the library repository exists.

The automated package-consumer smoke covers compilation and basic runtime use;
this manual gate covers actual AXAML theme integration.

The generated smoke project explicitly disables inherited Central Package
Management so that its `PackageReference` pins the exact candidate version.

## API lock review

For the stable `1.0.0` API lock, confirm:

- no public property is decorative or behaviorless;
- `PaperSpeed` is absent;
- toggle and rocker public contracts remain unchanged;
- implementation-only helpers are internal;
- public API documentation matches the assembly;
- all package documentation uses the final candidate version.

## Stable baseline acceptance

After this package passes every gate:

1. confirm the complete automated output;
2. repeat the manual demo gate;
3. record the benchmark output;
4. archive the complete validated ZIP as the official `1.0.0` baseline;
5. create any repository tag or publication only from that validated baseline.
