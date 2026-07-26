using IndustrialControls.Avalonia;

namespace IndustrialControls.Avalonia.Tests;

public sealed class StableReleaseContractTests
{
    [Fact]
    public void RuntimeMetadataUsesStableVersion()
    {
        Assert.Equal(
            "1.0.0",
            IndustrialControlsRelease.Version);
    }

    [Fact]
    public void PackageProjectUsesStableVersionAndReleaseNotes()
    {
        var project =
            ReadAsset(
                "IndustrialControls.Avalonia.csproj");

        Assert.Contains(
            "<Version>1.0.0</Version>",
            project);

        Assert.DoesNotContain(
            "<Version>1.0.0-rc.",
            project);

        Assert.Contains(
            "IndustrialControls.Avalonia 1.0.0",
            project);
    }

    [Fact]
    public void ValidationGateUsesStableReleaseLabel()
    {
        var powerShell =
            ReadAsset(
                "validate.ps1");

        var command =
            ReadAsset(
                "validate.cmd");

        foreach (var source in new[]
                 {
                     powerShell,
                     command
                 })
        {
            Assert.Contains(
                "1.0.0 VALIDATION PASSED",
                source);

            Assert.DoesNotContain(
                "M8 RC6-D VALIDATION PASSED",
                source);
        }
    }

    [Fact]
    public void StableReleaseDocumentationUsesStablePackage()
    {
        var readme =
            ReadAsset(
                "README.md");

        var checklist =
            ReadAsset(
                "RELEASE_CHECKLIST.md");

        var changelog =
            ReadAsset(
                "CHANGELOG.md");

        Assert.Contains(
            "--version 1.0.0",
            readme);

        Assert.DoesNotContain(
            "--version 1.0.0-rc.9",
            readme);

        Assert.Contains(
            "IndustrialControls.Avalonia.1.0.0.nupkg",
            checklist);

        Assert.Contains(
            "PACKAGE CONSUMER PASSED: 1.0.0",
            checklist);

        Assert.Contains(
            "## 1.0.0",
            changelog);
    }

    [Fact]
    public void ReadmeDisplaysCompleteScreenshotCatalog()
    {
        var readme =
            ReadAsset(
                "README.md");

        var screenshots = new[]
        {
            "01-foundation.png",
            "02-lamps-and-led.png",
            "03-gauges.png",
            "04-operator-controls.png",
            "05-trends-and-screens.png",
            "06-alarm-indicators.png",
            "07-static-and-release.png"
        };

        foreach (var screenshot in screenshots)
        {
            Assert.Contains(
                $"screenshot/{screenshot}",
                readme);

            var path = Path.Combine(
                AppContext.BaseDirectory,
                "Assets",
                "screenshot",
                screenshot);

            Assert.True(
                File.Exists(path),
                $"Missing screenshot asset: {path}");

            var signature =
                File.ReadAllBytes(path)
                    .Take(8)
                    .ToArray();

            Assert.Equal(
                new byte[]
                {
                    0x89,
                    0x50,
                    0x4E,
                    0x47,
                    0x0D,
                    0x0A,
                    0x1A,
                    0x0A
                },
                signature);
        }
    }

    [Fact]
    public void StablePromotionDoesNotReintroducePaperSpeed()
    {
        var source =
            ReadAsset(
                "StripChartRecorder.cs");

        var readme =
            ReadAsset(
                "README.md");

        Assert.DoesNotContain(
            "PaperSpeed",
            source);

        Assert.DoesNotContain(
            "PaperSpeed",
            readme);
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
