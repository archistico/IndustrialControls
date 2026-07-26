# Public API contract

## Release candidate

Version: `1.0.0-rc.2`

The following control families are release-gated:

1. Panel and framing controls
2. Lamps and illuminated commands
3. LED displays and annunciators
4. Gauges
5. Operator controls
6. Trends and screens
7. Alarm indicators
8. Static placards and data plates

## Compatibility policy

For stable `1.x` releases:

- existing public types will not be removed;
- existing public properties and methods will not be renamed;
- new optional properties may be added;
- visual corrections may be delivered without changing behavior;
- breaking changes require a new major version.

## Implementation renderers

Types ending in `Dial` are rendering helpers. They remain public in the first release candidate for XAML compatibility, but applications should use their corresponding high-level controls.

## Theme contract

The supported theme entry point is:

```csharp
IndustrialControls.Avalonia.Themes.IndustrialControlsTheme
```

Resource URI:

```text
avares://IndustrialControls.Avalonia/Themes/IndustrialControlsTheme.axaml
```


## High-frequency time-series API

The existing name-based overload remains supported:

```csharp
trend.AddSample("POWER", time, value);
```

For acquisition loops, applications may retain the series handle:

```csharp
var power = trend.AddSeries("POWER", "MWe", Colors.Green);
trend.AddSample(power, time, value);
```

The direct overload is additive and does not change the existing API contract.
