using System.Diagnostics;
using Avalonia.Media;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Benchmarks;

internal static class Program
{
    private const int Iterations = 100_000;
    private const int WarmupIterations = 2_000;

    public static int Main()
    {
        Console.WriteLine(
            "IndustrialControls.Avalonia benchmark smoke suite");
        Console.WriteLine(
            $"Iterations: {Iterations:N0}");
        Console.WriteLine(
            $"Warmup: {WarmupIterations:N0}");
        Console.WriteLine();

        RunTrendBufferBenchmark();
        RunTrendHandleBenchmark();
        RunGaugeUpdateBenchmark();
        RunSelectorBenchmark();
        RunMarqueeAdvanceBenchmark();
        RunStripChartRenderPlanBenchmark();

        return 0;
    }

    private static void RunTrendBufferBenchmark()
    {
        var trend = new TrendChart
        {
            MaxSamplesPerSeries = 600,
            TimeWindowSeconds = 60
        };

        var series = trend.AddSeries(
            "POWER",
            "MWe",
            Colors.Green);

        for (var index = 0;
             index < WarmupIterations;
             index++)
        {
            trend.AddSample(
                "POWER",
                index * 0.1,
                5.0 +
                Math.Sin(
                    index *
                    0.01));
        }

        trend.ClearSamples();

        Measure(
            "Bounded trend buffer / name lookup",
            Iterations,
            () =>
            {
                for (var index = 0;
                     index < Iterations;
                     index++)
                {
                    trend.AddSample(
                        "POWER",
                        index * 0.1,
                        5.0 +
                        Math.Sin(
                            index *
                            0.01));
                }
            });

        ValidateRetainedSamples(
            trend,
            series);
    }

    private static void RunTrendHandleBenchmark()
    {
        var trend = new TrendChart
        {
            MaxSamplesPerSeries = 600,
            TimeWindowSeconds = 60
        };

        var series = trend.AddSeries(
            "POWER",
            "MWe",
            Colors.Green);

        for (var index = 0;
             index < WarmupIterations;
             index++)
        {
            trend.AddSample(
                series,
                index * 0.1,
                5.0 +
                Math.Sin(
                    index *
                    0.01));
        }

        trend.ClearSamples();

        Measure(
            "Bounded trend buffer / direct handle",
            Iterations,
            () =>
            {
                for (var index = 0;
                     index < Iterations;
                     index++)
                {
                    trend.AddSample(
                        series,
                        index * 0.1,
                        5.0 +
                        Math.Sin(
                            index *
                            0.01));
                }
            });

        ValidateRetainedSamples(
            trend,
            series);
    }

    private static void RunGaugeUpdateBenchmark()
    {
        var gauge = new DigitalGauge
        {
            Minimum = 0,
            Maximum = 100,
            Unit = "%",
            DecimalPlaces = 2
        };

        for (var index = 0;
             index < WarmupIterations;
             index++)
        {
            gauge.Value =
                index %
                101;
        }

        Measure(
            "Gauge state updates",
            Iterations,
            () =>
            {
                for (var index = 0;
                     index < Iterations;
                     index++)
                {
                    gauge.Value =
                        index %
                        101;
                }
            });

        Console.WriteLine(
            $"  final value: {gauge.FormattedValue}");
    }

    private static void RunSelectorBenchmark()
    {
        var selector = new SelectorSwitch
        {
            PositionCount = 3,
            PositionLabels =
                "OFF|AUTO|MANUAL"
        };

        for (var index = 0;
             index < WarmupIterations;
             index++)
        {
            selector.Select(
                index %
                3);
        }

        Measure(
            "Selector state transitions",
            Iterations,
            () =>
            {
                for (var index = 0;
                     index < Iterations;
                     index++)
                {
                    selector.Select(
                        index %
                        3);
                }
            });

        Console.WriteLine(
            $"  final state: {selector.SelectedLabel}");
    }

    private static void RunMarqueeAdvanceBenchmark()
    {
        var marquee =
            new LedMarqueeDisplay
            {
                IsRunning = false,
                AutoFitVisibleCharacters = false,
                VisibleCharacters = 32,
                EndPauseCharacters = 8,
                Text =
                    "MAIN STEAM PRESSURE LOW — CHECK HEADER"
            };

        for (var index = 0;
             index < WarmupIterations;
             index++)
        {
            marquee.AdvanceForDiagnostics();
        }

        var sourceBuildsBefore =
            marquee.ScrollSourceBuildCount;

        Measure(
            "LED marquee cached advance",
            Iterations,
            () =>
            {
                for (var index = 0;
                     index < Iterations;
                     index++)
                {
                    marquee.AdvanceForDiagnostics();
                }
            });

        Console.WriteLine(
            $"  source rebuilds during measure: " +
            $"{marquee.ScrollSourceBuildCount - sourceBuildsBefore:N0}");
        Console.WriteLine(
            $"  cached source length: " +
            $"{marquee.CachedScrollSourceLength:N0}");
    }

    private static void RunStripChartRenderPlanBenchmark()
    {
        const int sampleCount = 100_000;
        const int planningIterations = 2_000;
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
                "TEMPERATURE",
                "°C",
                Colors.Green);

        for (var index = 0;
             index < sampleCount;
             index++)
        {
            recorder.AddSample(
                series,
                index * 0.1,
                50 +
                (10 *
                 Math.Sin(
                     index *
                     0.002)));
        }

        for (var index = 0;
             index < WarmupIterations;
             index++)
        {
            _ = recorder.GetRenderDiagnostics(
                plotWidth);
        }

        StripChartRenderDiagnostics diagnostics =
            default;

        Measure(
            "Strip-chart 100k render planning",
            planningIterations,
            () =>
            {
                for (var index = 0;
                     index < planningIterations;
                     index++)
                {
                    diagnostics =
                        recorder.GetRenderDiagnostics(
                            plotWidth);
                }
            });

        Console.WriteLine(
            $"  source samples: " +
            $"{diagnostics.SourceSampleCount:N0}");
        Console.WriteLine(
            $"  selected points: " +
            $"{diagnostics.SelectedPointCount:N0}");
        Console.WriteLine(
            $"  estimated segments: " +
            $"{diagnostics.EstimatedSegmentCount:N0}");

        Console.WriteLine(
            $"  quality breaks: " +
            $"{diagnostics.QualityBreakCount:N0}");
        Console.WriteLine(
            $"  uncertain points: " +
            $"{diagnostics.UncertainPointCount:N0}");
    }

    private static void ValidateRetainedSamples(
        TrendChart trend,
        SignalTraceSeries series)
    {
        if (series.Samples.Count !=
            trend.MaxSamplesPerSeries)
        {
            throw new InvalidOperationException(
                "Trend buffer exceeded or failed to reach its configured capacity.");
        }

        Console.WriteLine(
            $"  retained samples: {series.Samples.Count:N0}");
    }

    private static void Measure(
        string name,
        int operations,
        Action action)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var allocatedBefore =
            GC.GetAllocatedBytesForCurrentThread();

        var stopwatch =
            Stopwatch.StartNew();

        action();

        stopwatch.Stop();

        var allocatedBytes =
            GC.GetAllocatedBytesForCurrentThread() -
            allocatedBefore;

        Console.WriteLine(name);
        Console.WriteLine(
            $"  elapsed: " +
            $"{stopwatch.Elapsed.TotalMilliseconds:N2} ms");
        Console.WriteLine(
            $"  allocated: " +
            $"{allocatedBytes:N0} bytes");
        Console.WriteLine(
            $"  allocated/op: " +
            $"{(double)allocatedBytes / operations:N2} bytes");
    }
}
