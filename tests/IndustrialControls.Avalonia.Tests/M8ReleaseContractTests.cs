using System.Reflection;
using Avalonia.Automation;
using IndustrialControls.Avalonia;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M8ReleaseContractTests
{
    [Fact]
    public void ReleaseMetadata_UsesExpectedReleaseCandidateVersion()
    {
        Assert.Equal(
            "IndustrialControls.Avalonia",
            IndustrialControlsRelease.ProductName);
        Assert.Equal(
            "1.0.0-rc.6",
            IndustrialControlsRelease.Version);
        Assert.Equal(
            "avares://IndustrialControls.Avalonia/Themes/IndustrialControlsTheme.axaml",
            IndustrialControlsRelease.ThemeResourceUri);
    }

    [Fact]
    public void PublicAssembly_ExportsRequiredControlFamilies()
    {
        var exportedNames = typeof(IndustrialPanel)
            .Assembly
            .GetExportedTypes()
            .Select(type => type.FullName!)
            .ToHashSet(StringComparer.Ordinal);

        var requiredTypes = new[]
        {
            typeof(IndustrialPanel),
            typeof(IndustrialLamp),
            typeof(RadialGauge),
            typeof(IndustrialSlider),
            typeof(RotaryKnob),
            typeof(TrendChart),
            typeof(BacklitAlarmIndicator),
            typeof(SafetyPlacard),
            typeof(BoltedDataPlate)
        };

        foreach (var requiredType in requiredTypes)
        {
            Assert.Contains(requiredType.FullName!, exportedNames);
        }
    }

    [Fact]
    public void Gauge_ExposesAccessibleNameAndState()
    {
        var gauge = new DigitalGauge
        {
            Title = "Steam pressure",
            Value = 6.85,
            Unit = "MPa",
            DecimalPlaces = 2
        };

        Assert.Equal(
            "Steam pressure",
            AutomationProperties.GetName(gauge));
        var gaugeHelpText =
            AutomationProperties.GetHelpText(gauge) ?? string.Empty;

        Assert.Contains("6.85 MPa", gaugeHelpText);
        Assert.Contains("Normal", gaugeHelpText);
    }

    [Fact]
    public void RotaryKnob_ExposesAccessibleNameAndIsKeyboardFocusable()
    {
        var knob = new RotaryKnob
        {
            Title = "Generator load",
            Value = 5,
            Unit = "MWe"
        };

        Assert.True(knob.Focusable);
        Assert.Equal(
            "Generator load",
            AutomationProperties.GetName(knob));
        var knobHelpText =
            AutomationProperties.GetHelpText(knob) ?? string.Empty;

        Assert.Contains("5.0 MWe", knobHelpText);
    }

    [Fact]
    public void BacklitAlarmIndicator_IsAnAssertiveLiveRegionForNewAlarm()
    {
        var indicator = new BacklitAlarmIndicator
        {
            AlarmId = "STEAM_LOW",
            Text = "Main steam",
            SecondaryText = "Low pressure"
        };

        indicator.Activate();

        Assert.Equal(
            "Main steam",
            AutomationProperties.GetName(indicator));
        Assert.Equal(
            AutomationLiveSetting.Assertive,
            AutomationProperties.GetLiveSetting(indicator));
        var alarmHelpText =
            AutomationProperties.GetHelpText(indicator) ?? string.Empty;

        Assert.Contains("NEW ALARM", alarmHelpText);
    }

    [Fact]
    public void LegacyAlarmPriorityColor_IsDispatcherIndependent()
    {
        var annunciator = new AlarmAnnunciator
        {
            Priority = AlarmPriority.Critical
        };

        Assert.Equal(
            global::Avalonia.Media.Color.Parse("#F14C4C"),
            annunciator.PriorityColor);
    }

    [Fact]
    public void SignalQualityIndicator_ExposesQualityAndSource()
    {
        var indicator = new SignalQualityIndicator
        {
            SignalName = "Grid frequency",
            Source = "FT-GRID-01",
            Quality = SignalQuality.Uncertain
        };

        Assert.Equal(
            "Grid frequency",
            AutomationProperties.GetName(indicator));
        var qualityHelpText =
            AutomationProperties.GetHelpText(indicator) ?? string.Empty;

        Assert.Contains("UNCERTAIN", qualityHelpText);
        Assert.Contains("FT-GRID-01", qualityHelpText);
    }

    [Fact]
    public void InteractiveControls_AreKeyboardFocusable()
    {
        Assert.True(new IndustrialSlider().Focusable);
        Assert.True(new RotaryKnob().Focusable);
        Assert.True(new SelectorSwitch().Focusable);
        Assert.True(new IndustrialToggleSwitch().Focusable);
        Assert.True(new IndustrialRockerSwitch().Focusable);
        Assert.True(new SpringReturnSwitch().Focusable);
        Assert.True(new IlluminatedPushButton().Focusable);
    }

    [Fact]
    public void Theme_CoversEveryReleaseTemplatedControl()
    {
        var themePath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Industrial90.axaml");

        Assert.True(File.Exists(themePath));

        var theme = File.ReadAllText(themePath);
        var requiredThemes = new[]
        {
            "IndustrialPanel",
            "InstrumentBezel",
            "EngravedLabel",
            "IndustrialLamp",
            "IlluminatedPushButton",
            "LedMatrixDisplay",
            "LedMarqueeDisplay",
            "SevenSegmentDisplay",
            "AlarmAnnunciator",
            "RadialGauge",
            "LinearGauge",
            "DigitalGauge",
            "DeviationGauge",
            "IndustrialSlider",
            "RotaryKnob",
            "SelectorSwitch",
            "IndustrialToggleSwitch",
            "IndustrialRockerSwitch",
            "SpringReturnSwitch",
            "InterlockIndicator",
            "SignalQualityIndicator",
            "IndustrialScreen",
            "BacklitAlarmIndicator",
            "AlarmIndicatorPanel",
            "SafetyPlacard",
            "BoltedDataPlate"
        };

        foreach (var typeName in requiredThemes)
        {
            Assert.Contains(
                $"x:Type controls:{typeName}",
                theme);
        }

        Assert.Contains(
            "Industrial90.FocusAdorner",
            theme,
            StringComparison.Ordinal);
    }

    [Fact]
    public void TrendBuffer_RemainsBoundedDuringLongAcquisition()
    {
        var trend = new TrendChart
        {
            MaxSamplesPerSeries = 600
        };

        var series = trend.AddSeries(
            "POWER",
            "MWe",
            global::Avalonia.Media.Colors.Green);

        for (var index = 0; index < 100_000; index++)
        {
            trend.AddSample(
                "POWER",
                index * 0.1,
                5.0 + Math.Sin(index * 0.01));
        }

        Assert.Equal(600, series.Samples.Count);
        Assert.Equal(9_940.0, series.Samples[0].TimestampSeconds, 6);
        Assert.Equal(9_999.9, series.Samples[^1].TimestampSeconds, 6);
    }
}
