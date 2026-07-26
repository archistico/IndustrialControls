# Package usage

## Install

```powershell
dotnet add package IndustrialControls.Avalonia --version 1.0.0-rc.7
```

## Include the theme

```xml
<Application
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:industrialTheme="using:IndustrialControls.Avalonia.Themes">
  <Application.Styles>
    <FluentTheme />
    <industrialTheme:IndustrialControlsTheme />
  </Application.Styles>
</Application>
```

## Use a control

```xml
<Window
    xmlns="https://github.com/avaloniaui"
    xmlns:industrial="using:IndustrialControls.Avalonia.Controls">

  <industrial:RadialGauge
      Title="STEAM PRESSURE"
      Minimum="0"
      Maximum="8"
      Value="6.85"
      Unit="MPa"
      CautionHigh="7.2"
      WarningHigh="7.6" />
</Window>
```

## Accessibility

Set meaningful visible properties such as `Title`, `Text`, `SignalName` and `AlarmId`. Interactive and diagnostic controls derive their automation metadata from these properties.

Explicit application-specific metadata can still be supplied with Avalonia `AutomationProperties`.

## Time-series memory

Always choose `MaxSamplesPerSeries` or `MaxSamples` according to the acquisition frequency and required history. Buffers are bounded and discard their oldest samples when full.


## High-frequency acquisition

Retain the `SignalTraceSeries` handle returned by `AddSeries`:

```csharp
var powerSeries =
    trend.AddSeries("POWER", "MWe", Colors.Green);

trend.AddSample(
    powerSeries,
    timestampSeconds,
    measuredPower);
```

This avoids repeated name lookup and is the preferred path for dense acquisition.
