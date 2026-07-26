using Avalonia.Media;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M8AllocationOptimizationContractTests
{
    [Fact]
    public void TrendRingBuffer_PreservesChronologicalOrderAfterWrap()
    {
        var trend = new TrendChart
        {
            MaxSamplesPerSeries = 10
        };

        var series = trend.AddSeries(
            "POWER",
            "MWe",
            Colors.Green);

        for (var index = 0; index < 25; index++)
        {
            Assert.True(
                trend.AddSample(
                    series,
                    index,
                    index));
        }

        Assert.Equal(10, series.Samples.Count);

        for (var index = 0; index < 10; index++)
        {
            Assert.Equal(
                15 + index,
                series.Samples[index].Value,
                10);
        }
    }

    [Fact]
    public void TrendDirectHandle_RejectsSeriesOwnedByAnotherControl()
    {
        var firstTrend = new TrendChart();
        var secondTrend = new TrendChart();

        var foreignSeries = firstTrend.AddSeries(
            "POWER",
            "MWe",
            Colors.Green);

        Assert.False(
            secondTrend.AddSample(
                foreignSeries,
                0,
                5));
    }

    [Fact]
    public void TrendNameLookup_RemainsCaseInsensitive()
    {
        var trend = new TrendChart();
        var series = trend.AddSeries(
            "POWER",
            "MWe",
            Colors.Green);

        Assert.True(
            trend.AddSample(
                "power",
                1,
                5));

        Assert.Single(series.Samples);
    }

    [Fact]
    public void TrendCursorReadout_IsUpdatedLazilyButImmediatelyOnRead()
    {
        var trend = new TrendChart
        {
            TimeWindowSeconds = 10,
            CursorFraction = 1
        };

        var series = trend.AddSeries(
            "POWER",
            "MWe",
            Colors.Green);

        trend.AddSample(series, 10, 5.25);

        Assert.Contains(
            "POWER: 5.25 MWe",
            trend.CursorReadout);
    }

    [Fact]
    public void Gauge_ReusesBrushWhenStatusDoesNotChange()
    {
        var gauge = new DigitalGauge
        {
            Minimum = 0,
            Maximum = 100,
            Value = 10
        };

        var firstBrush = gauge.StatusBrush;
        gauge.Value = 20;

        Assert.Same(
            firstBrush,
            gauge.StatusBrush);
    }

    [Fact]
    public void Selector_UsesCachedLabelsAcrossTransitions()
    {
        var selector = new SelectorSwitch
        {
            PositionCount = 3,
            PositionLabels = "OFF|AUTO|MANUAL"
        };

        selector.Select(1);
        Assert.Equal("AUTO", selector.SelectedLabel);

        selector.Select(2);
        Assert.Equal("MANUAL", selector.SelectedLabel);

        selector.Select(0);
        Assert.Equal("OFF", selector.SelectedLabel);
    }
}
