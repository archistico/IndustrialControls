using Avalonia.Media;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M6TrendAndScreenContractTests
{
    [Fact]
    public void TrendChart_AddsSeriesAndRejectsDuplicateName()
    {
        var trend = new TrendChart();

        trend.AddSeries("POWER", "MWe", Colors.Green);

        Assert.Equal(1, trend.SeriesCount);
        Assert.Throws<InvalidOperationException>(
            () => trend.AddSeries("power", "MW", Colors.Red));
    }

    [Fact]
    public void TrendChart_AddSampleRejectsUnknownSeries()
    {
        var trend = new TrendChart();

        Assert.False(trend.AddSample("UNKNOWN", 0, 1));
    }

    [Fact]
    public void TrendChart_TrimsSamplesToCapacity()
    {
        var trend = new TrendChart
        {
            MaxSamplesPerSeries = 10
        };

        var series = trend.AddSeries("POWER", "MWe", Colors.Green);

        for (var index = 0; index < 15; index++)
        {
            Assert.True(trend.AddSample("POWER", index, index));
        }

        Assert.Equal(10, series.Samples.Count);
        Assert.Equal(5.0, series.Samples[0].TimestampSeconds, 10);
        Assert.Equal(14.0, trend.LatestTimeSeconds, 10);
    }

    [Fact]
    public void TrendChart_UsesConfiguredRangeWhenAutoScaleIsOff()
    {
        var trend = new TrendChart
        {
            Minimum = 0,
            Maximum = 10,
            AutoScale = false
        };

        var range = trend.GetEffectiveRange();

        Assert.Equal(0.0, range.Minimum, 10);
        Assert.Equal(10.0, range.Maximum, 10);
    }

    [Fact]
    public void TrendChart_AutoScaleAddsMargin()
    {
        var trend = new TrendChart
        {
            AutoScale = true
        };

        trend.AddSeries("POWER", "MWe", Colors.Green);
        trend.AddSample("POWER", 0, 4);
        trend.AddSample("POWER", 1, 6);

        var range = trend.GetEffectiveRange();

        Assert.True(range.Minimum < 4);
        Assert.True(range.Maximum > 6);
    }

    [Fact]
    public void TrendChart_CursorReadoutIncludesNearestSample()
    {
        var trend = new TrendChart
        {
            TimeWindowSeconds = 10,
            CursorFraction = 1
        };

        trend.AddSeries("POWER", "MWe", Colors.Green);
        trend.AddSample("POWER", 5, 4.5);
        trend.AddSample("POWER", 10, 5.25);

        Assert.Contains("POWER: 5.25 MWe", trend.CursorReadout);
        Assert.Contains("[GOOD]", trend.CursorReadout);
    }



    [Fact]
    public void TrendChart_ReducesStoredSamplesWhenCapacityIsLowered()
    {
        var trend = new TrendChart
        {
            MaxSamplesPerSeries = 20
        };

        var series = trend.AddSeries("POWER", "MWe", Colors.Green);

        for (var index = 0; index < 20; index++)
        {
            trend.AddSample("POWER", index, index);
        }

        trend.MaxSamplesPerSeries = 10;

        Assert.Equal(10, series.Samples.Count);
        Assert.Equal(10.0, series.Samples[0].TimestampSeconds, 10);
    }

    [Fact]
    public void Oscilloscope_TrimsSamplesAndTracksLastValue()
    {
        var scope = new OscilloscopeDisplay
        {
            MaxSamples = 16
        };

        for (var index = 0; index < 20; index++)
        {
            Assert.True(scope.AddSample(index));
        }

        Assert.Equal(16, scope.SampleCount);
        Assert.Equal(19.0, scope.LastValue, 10);
        Assert.Equal(4.0, scope.Samples[0], 10);
    }

    [Fact]
    public void Oscilloscope_RejectsNonFiniteSample()
    {
        var scope = new OscilloscopeDisplay();

        Assert.False(scope.AddSample(double.NaN));
        Assert.False(scope.AddSample(double.PositiveInfinity));
        Assert.Equal(0, scope.SampleCount);
    }

    [Fact]
    public void StripChartRecorder_RejectsSamplesWhilePaused()
    {
        var recorder = new StripChartRecorder
        {
            IsRunning = false
        };

        var series = recorder.AddSeries(
            "LEVEL",
            "%",
            Colors.Green);

        Assert.False(recorder.AddSample("LEVEL", 0, 50));
        Assert.Empty(series.Samples);
    }

    [Fact]
    public void StripChartRecorder_AcceptsSamplesWhileRunning()
    {
        var recorder = new StripChartRecorder
        {
            IsRunning = true
        };

        var series = recorder.AddSeries(
            "LEVEL",
            "%",
            Colors.Green);

        Assert.True(recorder.AddSample("LEVEL", 0, 50));
        Assert.Single(series.Samples);
    }

    [Theory]
    [InlineData(SignalQuality.Good, "GOOD", IndustrialLampColor.Green)]
    [InlineData(SignalQuality.Uncertain, "UNCERTAIN", IndustrialLampColor.Amber)]
    [InlineData(SignalQuality.Bad, "BAD", IndustrialLampColor.Red)]
    [InlineData(SignalQuality.Unavailable, "UNAVAILABLE", IndustrialLampColor.Blue)]
    public void SignalQualityIndicator_MapsQuality(
        SignalQuality quality,
        string expectedText,
        IndustrialLampColor expectedLamp)
    {
        var indicator = new SignalQualityIndicator
        {
            Quality = quality
        };

        Assert.Equal(expectedText, indicator.QualityText);
        Assert.Equal(expectedLamp, indicator.LampColor);
    }

    [Fact]
    public void IndustrialScreen_HasStableDefaults()
    {
        var screen = new IndustrialScreen();

        Assert.True(screen.IsOnline);
        Assert.True(screen.ShowScanlines);
        Assert.Equal("ONLINE", screen.StatusText);
    }

    [Fact]
    public void TimeSeriesControl_ClearSamplesPreservesSeries()
    {
        var trend = new TrendChart();
        var series = trend.AddSeries("POWER", "MWe", Colors.Green);

        trend.AddSample("POWER", 1, 5);
        trend.ClearSamples();

        Assert.Equal(1, trend.SeriesCount);
        Assert.Empty(series.Samples);
        Assert.Equal(0.0, trend.LatestTimeSeconds, 10);
        Assert.Equal("NO DATA", trend.CursorReadout);
    }
}
