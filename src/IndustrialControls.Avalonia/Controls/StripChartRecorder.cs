using System;
using System.Globalization;
using Avalonia;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Registratore multicanale a carta continua.
/// </summary>
public sealed class StripChartRecorder : TimeSeriesControlBase
{
    private const int HorizontalGridDivisions = 10;
    private const int MinorDivisionsPerMajor = 5;

    private static readonly IBrush FrameBrush =
        new SolidColorBrush(
            Color.Parse("#202326"));

    private static readonly IBrush PaperBrush =
        new SolidColorBrush(
            Color.Parse("#D9D5C0"));

    private static readonly IBrush HeaderBrush =
        new SolidColorBrush(
            Color.Parse("#2E372B"));

    private static readonly Pen FramePen =
        new(
            new SolidColorBrush(
                Color.Parse("#0B0C0D")),
            6);

    private static readonly Pen PaperPen =
        new(
            new SolidColorBrush(
                Color.Parse("#77766E")),
            1);

    private static readonly Pen MinorGridPen =
        new(
            new SolidColorBrush(
                Color.Parse("#B8C1A8")),
            1);

    private static readonly Pen MajorGridPen =
        new(
            new SolidColorBrush(
                Color.Parse("#8FA47F")),
            1);

    private static readonly Pen UncertainTracePen =
        new(
            new SolidColorBrush(
                Color.Parse("#A27E18")),
            2);

    public static readonly StyledProperty<bool> IsRunningProperty =
        AvaloniaProperty.Register<StripChartRecorder, bool>(
            nameof(IsRunning),
            true);

    public static readonly StyledProperty<double> MajorGridSecondsProperty =
        AvaloniaProperty.Register<StripChartRecorder, double>(
            nameof(MajorGridSeconds),
            10.0,
            validate: value => value > 0);

    static StripChartRecorder()
    {
        AffectsRender<StripChartRecorder>(
            IsRunningProperty,
            MajorGridSecondsProperty);
    }

    public bool IsRunning
    {
        get => GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }

    public double MajorGridSeconds
    {
        get => GetValue(MajorGridSecondsProperty);
        set => SetValue(MajorGridSecondsProperty, value);
    }

    public override bool AddSample(
        string seriesName,
        double timestampSeconds,
        double value,
        SignalQuality quality = SignalQuality.Good)
    {
        if (!IsRunning)
        {
            return false;
        }

        return base.AddSample(
            seriesName,
            timestampSeconds,
            value,
            quality);
    }

    public override bool AddSample(
        SignalTraceSeries series,
        double timestampSeconds,
        double value,
        SignalQuality quality = SignalQuality.Good)
    {
        if (!IsRunning)
        {
            return false;
        }

        return base.AddSample(
            series,
            timestampSeconds,
            value,
            quality);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width < 120 ||
            Bounds.Height < 100)
        {
            return;
        }

        var frame =
            new Rect(
                0,
                0,
                Bounds.Width,
                Bounds.Height);

        context.DrawRectangle(
            FrameBrush,
            FramePen,
            frame);

        var paper =
            frame.Deflate(14);

        context.DrawRectangle(
            PaperBrush,
            PaperPen,
            paper);

        var plot = new Rect(
            paper.Left + 38,
            paper.Top + 24,
            paper.Width - 54,
            paper.Height - 44);

        var windowEnd =
            LatestTimeSeconds;

        var windowStart =
            windowEnd -
            TimeWindowSeconds;

        DrawPaperGrid(
            context,
            plot,
            windowStart,
            windowEnd);

        DrawTraces(
            context,
            plot,
            windowStart,
            windowEnd);

        DrawHeader(
            context,
            paper);
    }

    internal StripChartRenderDiagnostics GetRenderDiagnostics(
        double plotWidth)
    {
        var windowEnd =
            LatestTimeSeconds;

        var windowStart =
            windowEnd -
            TimeWindowSeconds;

        var sourceSampleCount = 0;
        var visibleSampleCount = 0;
        var selectedPointCount = 0;
        var estimatedSegmentCount = 0;
        var qualityBreakCount = 0;
        var uncertainPointCount = 0;

        foreach (var series in TraceSeries)
        {
            if (!series.IsVisible)
            {
                continue;
            }

            sourceSampleCount +=
                series.Samples.Count;

            AnalyzeSeries(
                series.Samples,
                windowStart,
                windowEnd,
                plotWidth,
                out var visible,
                out var selected,
                out var segments,
                out var qualityBreaks,
                out var uncertainPoints);

            visibleSampleCount += visible;
            selectedPointCount += selected;
            estimatedSegmentCount += segments;
            qualityBreakCount += qualityBreaks;
            uncertainPointCount += uncertainPoints;
        }

        return new StripChartRenderDiagnostics(
            sourceSampleCount,
            visibleSampleCount,
            selectedPointCount,
            estimatedSegmentCount,
            qualityBreakCount,
            uncertainPointCount);
    }

    private void DrawPaperGrid(
        DrawingContext context,
        Rect plot,
        double windowStart,
        double windowEnd)
    {
        var minorGridSeconds =
            MajorGridSeconds /
            MinorDivisionsPerMajor;

        var firstGridTime =
            Math.Ceiling(
                windowStart /
                minorGridSeconds) *
            minorGridSeconds;

        var timeSpan = Math.Max(
            1e-12,
            windowEnd -
            windowStart);

        var gridIndex = (long)Math.Round(
            firstGridTime /
            minorGridSeconds);

        for (var gridTime = firstGridTime;
             gridTime <= windowEnd + 1e-9;
             gridTime += minorGridSeconds,
             gridIndex++)
        {
            var xFraction =
                (gridTime - windowStart) /
                timeSpan;

            var x =
                plot.Left +
                (plot.Width * xFraction);

            var isMajor =
                gridIndex %
                MinorDivisionsPerMajor ==
                0;

            context.DrawLine(
                isMajor
                    ? MajorGridPen
                    : MinorGridPen,
                new Point(
                    x,
                    plot.Top),
                new Point(
                    x,
                    plot.Bottom));
        }

        for (var index = 0;
             index <= HorizontalGridDivisions;
             index++)
        {
            var y =
                plot.Top +
                (plot.Height *
                 index /
                 HorizontalGridDivisions);

            context.DrawLine(
                index %
                    (HorizontalGridDivisions / 2) ==
                    0
                    ? MajorGridPen
                    : MinorGridPen,
                new Point(
                    plot.Left,
                    y),
                new Point(
                    plot.Right,
                    y));
        }
    }

    private void DrawTraces(
        DrawingContext context,
        Rect plot,
        double windowStart,
        double windowEnd)
    {
        var range =
            GetEffectiveRange();

        var timeSpan = Math.Max(
            1e-12,
            windowEnd -
            windowStart);

        var valueSpan = Math.Max(
            1e-12,
            range.Maximum -
            range.Minimum);

        foreach (var series in TraceSeries)
        {
            if (!series.IsVisible)
            {
                continue;
            }

            DrawSeries(
                context,
                plot,
                series,
                windowStart,
                windowEnd,
                timeSpan,
                range.Minimum,
                valueSpan);
        }
    }

    private static void DrawSeries(
        DrawingContext context,
        Rect plot,
        SignalTraceSeries series,
        double windowStart,
        double windowEnd,
        double timeSpan,
        double minimum,
        double valueSpan)
    {
        var samples =
            series.Samples;

        FindVisibleRange(
            samples,
            windowStart,
            windowEnd,
            out var visibleCount,
            out var lastVisibleIndex);

        if (visibleCount == 0)
        {
            return;
        }

        var step =
            CalculateDecimationStep(
                visibleCount,
                plot.Width);

        var visibleOrdinal = 0;
        SignalSample? previousSelected = null;
        var uncertainSegment = false;

        for (var index = 0;
             index < samples.Count;
             index++)
        {
            var sample =
                samples[index];

            if (sample.TimestampSeconds < windowStart ||
                sample.TimestampSeconds > windowEnd)
            {
                continue;
            }

            var mustSelect =
                visibleOrdinal % step == 0 ||
                index == lastVisibleIndex ||
                sample.Quality is not SignalQuality.Good;

            visibleOrdinal++;

            if (sample.Quality is
                SignalQuality.Bad or
                SignalQuality.Unavailable)
            {
                previousSelected = null;
                uncertainSegment = false;
                continue;
            }

            if (sample.Quality ==
                SignalQuality.Uncertain)
            {
                uncertainSegment = true;
            }

            if (!mustSelect)
            {
                continue;
            }

            var currentPoint =
                MapPoint(
                    plot,
                    sample,
                    windowStart,
                    timeSpan,
                    minimum,
                    valueSpan);

            if (previousSelected is
                { } previousSample)
            {
                context.DrawLine(
                    uncertainSegment
                        ? UncertainTracePen
                        : series.TracePen,
                    MapPoint(
                        plot,
                        previousSample,
                        windowStart,
                        timeSpan,
                        minimum,
                        valueSpan),
                    currentPoint);
            }

            previousSelected = sample;
            uncertainSegment =
                sample.Quality ==
                SignalQuality.Uncertain;
        }
    }

    private static void AnalyzeSeries(
        IReadOnlyList<SignalSample> samples,
        double windowStart,
        double windowEnd,
        double plotWidth,
        out int visibleCount,
        out int selectedPointCount,
        out int estimatedSegmentCount,
        out int qualityBreakCount,
        out int uncertainPointCount)
    {
        FindVisibleRange(
            samples,
            windowStart,
            windowEnd,
            out visibleCount,
            out var lastVisibleIndex);

        selectedPointCount = 0;
        estimatedSegmentCount = 0;
        qualityBreakCount = 0;
        uncertainPointCount = 0;

        if (visibleCount == 0)
        {
            return;
        }

        var step =
            CalculateDecimationStep(
                visibleCount,
                plotWidth);

        var visibleOrdinal = 0;
        var hasPreviousSelected = false;

        for (var index = 0;
             index < samples.Count;
             index++)
        {
            var sample =
                samples[index];

            if (sample.TimestampSeconds < windowStart ||
                sample.TimestampSeconds > windowEnd)
            {
                continue;
            }

            var mustSelect =
                visibleOrdinal % step == 0 ||
                index == lastVisibleIndex ||
                sample.Quality is not SignalQuality.Good;

            visibleOrdinal++;

            if (sample.Quality is
                SignalQuality.Bad or
                SignalQuality.Unavailable)
            {
                qualityBreakCount++;
                hasPreviousSelected = false;
                continue;
            }

            if (sample.Quality ==
                SignalQuality.Uncertain)
            {
                uncertainPointCount++;
            }

            if (!mustSelect)
            {
                continue;
            }

            selectedPointCount++;

            if (hasPreviousSelected)
            {
                estimatedSegmentCount++;
            }

            hasPreviousSelected = true;
        }
    }

    private static void FindVisibleRange(
        IReadOnlyList<SignalSample> samples,
        double windowStart,
        double windowEnd,
        out int visibleCount,
        out int lastVisibleIndex)
    {
        visibleCount = 0;
        lastVisibleIndex = -1;

        for (var index = 0;
             index < samples.Count;
             index++)
        {
            var timestamp =
                samples[index]
                    .TimestampSeconds;

            if (timestamp < windowStart ||
                timestamp > windowEnd)
            {
                continue;
            }

            visibleCount++;
            lastVisibleIndex = index;
        }
    }

    private static int CalculateDecimationStep(
        int visibleCount,
        double plotWidth)
    {
        var pixelBudget = Math.Max(
            1,
            (int)Math.Floor(
                Math.Max(
                    1,
                    plotWidth)));

        return Math.Max(
            1,
            visibleCount /
            pixelBudget);
    }

    private void DrawHeader(
        DrawingContext context,
        Rect paper)
    {
        var title =
            new FormattedText(
                Title,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                Typeface.Default,
                11,
                HeaderBrush);

        context.DrawText(
            title,
            new Point(
                paper.Left + 8,
                paper.Top + 5));

        var statusText = string.Concat(
            IsRunning
                ? "RUN"
                : "PAUSE",
            " | WINDOW ",
            TimeWindowSeconds.ToString(
                "0.##",
                CultureInfo.InvariantCulture),
            " s | GRID ",
            MajorGridSeconds.ToString(
                "0.##",
                CultureInfo.InvariantCulture),
            " s");

        var status =
            new FormattedText(
                statusText,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                Typeface.Default,
                9,
                HeaderBrush);

        context.DrawText(
            status,
            new Point(
                paper.Right -
                status.Width -
                8,
                paper.Top + 6));
    }

    private static Point MapPoint(
        Rect plot,
        SignalSample sample,
        double windowStart,
        double timeSpan,
        double minimum,
        double valueSpan)
    {
        var xFraction =
            Math.Clamp(
                (sample.TimestampSeconds -
                 windowStart) /
                timeSpan,
                0,
                1);

        var yFraction =
            Math.Clamp(
                (sample.Value -
                 minimum) /
                valueSpan,
                0,
                1);

        return new Point(
            plot.Left +
            (plot.Width *
             xFraction),
            plot.Bottom -
            (plot.Height *
             yFraction));
    }
}
