namespace IndustrialControls.Avalonia.Tests;

public sealed class M8DemoCatalogContractTests
{
    [Fact]
    public void Theme_UsesTheCorrectFocusAdornerTemplateType()
    {
        var theme = ReadAsset("Industrial90.axaml");

        Assert.Contains(
            "<FocusAdornerTemplate x:Key=\"Industrial90.FocusAdorner\">",
            theme);

        Assert.DoesNotContain(
            "<ControlTemplate x:Key=\"Industrial90.FocusAdorner\">",
            theme);
    }

    [Fact]
    public void DemoCatalog_ContainsEveryPublicHighLevelControl()
    {
        var demo = ReadAsset("DemoMainWindow.axaml");

        var requiredControls = new[]
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
            "AlarmAnnunciatorPanel",
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
            "TrendChart",
            "OscilloscopeDisplay",
            "StripChartRecorder",
            "SignalQualityIndicator",
            "IndustrialScreen",
            "BacklitAlarmIndicator",
            "AlarmIndicatorPanel",
            "SafetyPlacard",
            "BoltedDataPlate"
        };

        foreach (var controlName in requiredControls)
        {
            Assert.Contains(
                $"industrial:{controlName}",
                demo);
        }
    }

    [Fact]
    public void DemoCatalog_IsOrganizedIntoSevenFunctionalTabs()
    {
        var demo = ReadAsset("DemoMainWindow.axaml");

        var requiredTabs = new[]
        {
            "FOUNDATION",
            "LAMPS &amp; LED",
            "GAUGES",
            "OPERATOR CONTROLS",
            "TRENDS &amp; SCREENS",
            "ALARM INDICATORS",
            "STATIC &amp; RELEASE"
        };

        foreach (var tab in requiredTabs)
        {
            Assert.Contains(
                $"Header=\"{tab}\"",
                demo);
        }
    }

    private static string ReadAsset(string fileName)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            fileName);

        Assert.True(
            File.Exists(path),
            $"Missing test asset: {path}");

        return File.ReadAllText(path);
    }
}
