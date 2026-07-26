using Avalonia.Media;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M8RenderingPerformanceContractTests
{
    [Theory]
    [InlineData(0.0, true, 24, 24)]
    [InlineData(40.0, true, 24, 4)]
    [InlineData(50.0, true, 24, 4)]
    [InlineData(354.0, true, 24, 20)]
    [InlineData(354.0, false, 24, 24)]
    public void MarqueeCapacityUsesOneDeterministicLaw(
        double width,
        bool autoFit,
        int manualCapacity,
        int expectedCapacity)
    {
        Assert.Equal(
            expectedCapacity,
            LedMarqueeDisplay
                .CalculateEffectiveVisibleCharacters(
                    width,
                    autoFit,
                    manualCapacity));
    }

    [Fact]
    public void MarqueeAdvancingDoesNotRebuildCachedSource()
    {
        var marquee =
            new LedMarqueeDisplay
            {
                IsRunning = false,
                AutoFitVisibleCharacters = false,
                VisibleCharacters = 16,
                EndPauseCharacters = 4,
                Text = "ALARM"
            };

        var sourceBuilds =
            marquee.ScrollSourceBuildCount;

        for (var index = 0;
             index < 1_000;
             index++)
        {
            marquee.AdvanceForDiagnostics();
        }

        Assert.Equal(
            sourceBuilds,
            marquee.ScrollSourceBuildCount);

        Assert.Equal(
            16 +
            "ALARM".Length +
            4,
            marquee.CachedScrollSourceLength);
    }

    [Fact]
    public void MarqueeSourceRebuildsOnlyWhenItsInputsChange()
    {
        var marquee =
            new LedMarqueeDisplay
            {
                IsRunning = false,
                AutoFitVisibleCharacters = false,
                VisibleCharacters = 12,
                Text = "READY"
            };

        var initialBuildCount =
            marquee.ScrollSourceBuildCount;

        marquee.ScrollIntervalMilliseconds = 200;
        marquee.IsRunning = true;
        marquee.IsRunning = false;

        Assert.Equal(
            initialBuildCount,
            marquee.ScrollSourceBuildCount);

        marquee.Text = "TRIP";

        Assert.Equal(
            initialBuildCount + 1,
            marquee.ScrollSourceBuildCount);

        marquee.EndPauseCharacters = 10;

        Assert.Equal(
            initialBuildCount + 2,
            marquee.ScrollSourceBuildCount);
    }

    [Fact]
    public void StripChartDecimatesOneHundredThousandGoodSamplesToPixelBudget()
    {
        const int sampleCount = 100_000;
        const double plotWidth = 900;

        var recorder =
            new StripChartRecorder
            {
                MaxSamplesPerSeries = sampleCount,
                TimeWindowSeconds = 10_000,
                AutoScale = true
            };

        var series =
            recorder.AddSeries(
                "POWER",
                "MWe",
                Colors.Green);

        for (var index = 0;
             index < sampleCount;
             index++)
        {
            Assert.True(
                recorder.AddSample(
                    series,
                    index * 0.1,
                    5 +
                    Math.Sin(
                        index *
                        0.002)));
        }

        var diagnostics =
            recorder.GetRenderDiagnostics(
                plotWidth);

        Assert.Equal(
            sampleCount,
            diagnostics.SourceSampleCount);

        Assert.Equal(
            sampleCount,
            diagnostics.VisibleSampleCount);

        Assert.InRange(
            diagnostics.SelectedPointCount,
            890,
            920);

        Assert.Equal(
            diagnostics.SelectedPointCount - 1,
            diagnostics.EstimatedSegmentCount);
    }

    [Fact]
    public void StripChartPreservesQualityBreaksDuringDecimation()
    {
        var recorder =
            new StripChartRecorder
            {
                MaxSamplesPerSeries = 10_000,
                TimeWindowSeconds = 1_000
            };

        var series =
            recorder.AddSeries(
                "PRESSURE",
                "MPa",
                Colors.Green);

        for (var index = 0;
             index < 10_000;
             index++)
        {
            var quality = index switch
            {
                2_500 => SignalQuality.Uncertain,
                5_000 => SignalQuality.Bad,
                7_500 => SignalQuality.Unavailable,
                _ => SignalQuality.Good
            };

            recorder.AddSample(
                series,
                index * 0.1,
                6.8,
                quality);
        }

        var diagnostics =
            recorder.GetRenderDiagnostics(
                500);

        Assert.Equal(
            499,
            diagnostics.SelectedPointCount);

        Assert.Equal(
            496,
            diagnostics.EstimatedSegmentCount);

        Assert.Equal(
            2,
            diagnostics.QualityBreakCount);

        Assert.Equal(
            1,
            diagnostics.UncertainPointCount);
    }

    [Fact]
    public void StripChartDirectHandleRespectsPausedState()
    {
        var recorder =
            new StripChartRecorder
            {
                IsRunning = false
            };

        var series =
            recorder.AddSeries(
                "LEVEL",
                "%",
                Colors.Green);

        Assert.False(
            recorder.AddSample(
                series,
                1,
                50));

        Assert.Empty(
            series.Samples);
    }

    [Fact]
    public void StripChartSourceDoesNotUseUnresolvedInheritedCref()
    {
        var source =
            ReadAsset(
                "StripChartRecorder.cs");

        Assert.DoesNotContain(
            "cref=\"TimeWindowSeconds\"",
            source);

        Assert.Contains(
            "TimeWindowSeconds.ToString",
            source);

        Assert.Contains(
            "MajorGridSeconds.ToString",
            source);
    }

    [Fact]
    public void StripChartSourceContainsNoPerSegmentPenAllocationOrLinq()
    {
        var source =
            ReadAsset(
                "StripChartRecorder.cs");

        Assert.DoesNotContain(
            "System.Linq",
            source);

        var drawSeriesStart =
            source.IndexOf(
                "private static void DrawSeries",
                StringComparison.Ordinal);

        var analyzeStart =
            source.IndexOf(
                "private static void AnalyzeSeries",
                StringComparison.Ordinal);

        Assert.True(
            drawSeriesStart >= 0 &&
            analyzeStart > drawSeriesStart);

        var drawSeriesSource =
            source[
                drawSeriesStart..
                analyzeStart];

        Assert.DoesNotContain(
            "new Pen(",
            drawSeriesSource);

        Assert.DoesNotContain(
            "new SolidColorBrush(",
            drawSeriesSource);

        Assert.Contains(
            "series.TracePen",
            drawSeriesSource);

        Assert.Contains(
            "UncertainTracePen",
            drawSeriesSource);
    }

    [Fact]
    public void AllOperatorThemesExposeCommonInterlockStyle()
    {
        var theme =
            ReadAsset(
                "Industrial90.axaml");

        var controls = new[]
        {
            "IndustrialSlider",
            "RotaryKnob",
            "SelectorSwitch",
            "IndustrialToggleSwitch",
            "SpringReturnSwitch",
            "IndustrialRockerSwitch",
            "IlluminatedPushButton"
        };

        foreach (var control in controls)
        {
            Assert.Contains(
                $"controls:{control}",
                theme);
        }

        Assert.True(
            CountOccurrences(
                theme,
                "Selector=\"^:interlocked\"") >= 5);

        Assert.True(
            CountOccurrences(
                theme,
                "TextBlock#InterlockStatusText") >= 6);

        Assert.Contains(
            "controls|IlluminatedPushButton:interlocked",
            theme);
    }

    private static int CountOccurrences(
        string text,
        string value)
    {
        var count = 0;
        var offset = 0;

        while (true)
        {
            offset = text.IndexOf(
                value,
                offset,
                StringComparison.Ordinal);

            if (offset < 0)
            {
                return count;
            }

            count++;
            offset += value.Length;
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
