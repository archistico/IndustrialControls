using Avalonia.Automation;
using IndustrialControls.Avalonia;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M8FinalApiReleaseGateContractTests
{
    [Fact]
    public void StableVersionIsConsistent()
    {
        Assert.Equal(
            "1.0.0",
            IndustrialControlsRelease.Version);
    }

    [Fact]
    public void StripChartDoesNotExposeDecorativePaperSpeedApi()
    {
        Assert.Null(
            typeof(StripChartRecorder)
                .GetProperty("PaperSpeed"));

        Assert.Null(
            typeof(StripChartRecorder)
                .GetField("PaperSpeedProperty"));
    }

    [Fact]
    public void StripChartSourceReportsActualWindowAndGrid()
    {
        var source =
            ReadAsset(
                "StripChartRecorder.cs");

        Assert.DoesNotContain(
            "PaperSpeed",
            source);

        Assert.Contains(
            "TimeWindowSeconds.ToString",
            source);

        Assert.Contains(
            "MajorGridSeconds.ToString",
            source);
    }

    [Fact]
    public void DemoAndReadmeDoNotUsePaperSpeed()
    {
        var demo =
            ReadAsset(
                "DemoMainWindow.axaml");

        var readme =
            ReadAsset(
                "README.md");

        Assert.DoesNotContain(
            "PaperSpeed",
            demo);

        Assert.DoesNotContain(
            "PaperSpeed",
            readme);

        Assert.Contains(
            "TimeWindowSeconds",
            readme);

        Assert.Contains(
            "MajorGridSeconds",
            readme);
    }

    [Fact]
    public void BistableSwitchesUseSharedInternalBehavior()
    {
        var toggleSource =
            ReadAsset(
                "IndustrialToggleSwitch.cs");

        var rockerSource =
            ReadAsset(
                "IndustrialRockerSwitch.cs");

        foreach (var source in new[]
                 {
                     toggleSource,
                     rockerSource
                 })
        {
            Assert.Contains(
                "IndustrialBistableSwitchBehavior.Evaluate",
                source);

            Assert.Contains(
                "IndustrialBistableSwitchBehavior.TryToggle",
                source);

            Assert.DoesNotContain(
                "IsChecked = IsChecked != true",
                source);
        }
    }

    [Fact]
    public void SharedBistableBehaviorPreservesPublicSemantics()
    {
        var state =
            IndustrialBistableSwitchBehavior.Evaluate(
                true,
                "CLOSED",
                "OPEN",
                true,
                "SYNCHRONISM MISSING");

        Assert.True(state.IsOn);
        Assert.Equal(
            "CLOSED",
            state.StateText);
        Assert.Equal(
            "INTERLOCK — SYNCHRONISM MISSING",
            state.StatusText);

        var toggle =
            new IndustrialToggleSwitch();

        Assert.True(
            IndustrialBistableSwitchBehavior.TryToggle(
                toggle,
                false));

        Assert.True(
            toggle.IsChecked);

        Assert.False(
            IndustrialBistableSwitchBehavior.TryToggle(
                toggle,
                true));

        Assert.True(
            toggle.IsChecked);
    }

    [Theory]
    [InlineData(
        "Main steam / pressure",
        "Gauge.Main-steam-pressure")]
    [InlineData(
        "---",
        "Gauge.Control")]
    public void AutomationIdsAreNormalizedDeterministically(
        string title,
        string expectedAutomationId)
    {
        var gauge =
            new DigitalGauge
            {
                Title = title
            };

        Assert.Equal(
            expectedAutomationId,
            AutomationProperties.GetAutomationId(
                gauge));
    }

    [Fact]
    public void ReleaseGateUsesStableLabelAndPackageConsumer()
    {
        var powerShell =
            ReadAsset(
                "validate.ps1");

        var command =
            ReadAsset(
                "validate.cmd");

        var consumer =
            ReadAsset(
                "validate-package-consumer.ps1");

        Assert.Contains(
            "1.0.0 VALIDATION PASSED",
            powerShell);

        Assert.Contains(
            "1.0.0 VALIDATION PASSED",
            command);

        Assert.DoesNotContain(
            "M8 RC6-D VALIDATION PASSED",
            powerShell);

        Assert.DoesNotContain(
            "M8 RC6-D VALIDATION PASSED",
            command);

        Assert.Contains(
            "validate-package-consumer.ps1",
            powerShell);

        Assert.Contains(
            "validate-package-consumer.ps1",
            command);

        Assert.Contains(
            "PACKAGE CONSUMER PASSED",
            consumer);

        Assert.Contains(
            "PackageReference Include=\"IndustrialControls.Avalonia\"",
            consumer);

        Assert.Contains(
            "<ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>",
            consumer);
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
