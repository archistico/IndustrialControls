using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Registratore multicanale a carta continua.
/// </summary>
public sealed class StripChartRecorder : TimeSeriesControlBase
{
    public static readonly StyledProperty<bool> IsRunningProperty =
        AvaloniaProperty.Register<StripChartRecorder, bool>(
            nameof(IsRunning), true);

    public static readonly StyledProperty<double> PaperSpeedProperty =
        AvaloniaProperty.Register<StripChartRecorder, double>(
            nameof(PaperSpeed),
            10.0,
            validate: value => value > 0);

    public static readonly StyledProperty<double> MajorGridSecondsProperty =
        AvaloniaProperty.Register<StripChartRecorder, double>(
            nameof(MajorGridSeconds),
            10.0,
            validate: value => value > 0);

    static StripChartRecorder()
    {
        AffectsRender<StripChartRecorder>(
            IsRunningProperty,
            PaperSpeedProperty,
            MajorGridSecondsProperty);
    }

    public bool IsRunning
    {
        get => GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }

    public double PaperSpeed
    {
        get => GetValue(PaperSpeedProperty);
        set => SetValue(PaperSpeedProperty, value);
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

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width < 120 || Bounds.Height < 100)
        {
            return;
        }

        var frame = new Rect(0, 0, Bounds.Width, Bounds.Height);
        context.DrawRectangle(
            new SolidColorBrush(Color.Parse("#202326")),
            new Pen(new SolidColorBrush(Color.Parse("#0B0C0D")), 6),
            frame);

        var paper = frame.Deflate(14);
        context.DrawRectangle(
            new SolidColorBrush(Color.Parse("#D9D5C0")),
            new Pen(new SolidColorBrush(Color.Parse("#77766E")), 1),
            paper);

        var plot = new Rect(
            paper.Left + 38,
            paper.Top + 24,
            paper.Width - 54,
            paper.Height - 44);

        DrawPaperGrid(context, plot);
        DrawTraces(context, plot);
        DrawHeader(context, paper);
    }

    private void DrawPaperGrid(DrawingContext context, Rect plot)
    {
        var minorPen = new Pen(
            new SolidColorBrush(Color.Parse("#B8C1A8")),
            1);
        var majorPen = new Pen(
            new SolidColorBrush(Color.Parse("#8FA47F")),
            1);

        for (var index = 0; index <= 20; index++)
        {
            var x = plot.Left + (plot.Width * index / 20.0);
            context.DrawLine(
                index % 5 == 0 ? majorPen : minorPen,
                new Point(x, plot.Top),
                new Point(x, plot.Bottom));
        }

        for (var index = 0; index <= 10; index++)
        {
            var y = plot.Top + (plot.Height * index / 10.0);
            context.DrawLine(
                index % 5 == 0 ? majorPen : minorPen,
                new Point(plot.Left, y),
                new Point(plot.Right, y));
        }
    }

    private void DrawTraces(DrawingContext context, Rect plot)
    {
        var range = GetEffectiveRange();
        var windowEnd = LatestTimeSeconds;
        var windowStart = windowEnd - TimeWindowSeconds;
        var timeSpan = Math.Max(1e-12, windowEnd - windowStart);
        var valueSpan = Math.Max(1e-12, range.Maximum - range.Minimum);

        foreach (var series in TraceSeries.Where(series => series.IsVisible))
        {
            SignalSample? previous = null;

            foreach (var sample in series.Samples)
            {
                if (sample.TimestampSeconds < windowStart ||
                    sample.TimestampSeconds > windowEnd)
                {
                    continue;
                }

                if (sample.Quality is SignalQuality.Bad or SignalQuality.Unavailable)
                {
                    previous = null;
                    continue;
                }

                if (previous is { } previousSample)
                {
                    var color = sample.Quality == SignalQuality.Uncertain ||
                                previousSample.Quality == SignalQuality.Uncertain
                        ? Color.Parse("#A27E18")
                        : series.Color;

                    context.DrawLine(
                        new Pen(new SolidColorBrush(color), 2),
                        MapPoint(
                            plot,
                            previousSample,
                            windowStart,
                            timeSpan,
                            range.Minimum,
                            valueSpan),
                        MapPoint(
                            plot,
                            sample,
                            windowStart,
                            timeSpan,
                            range.Minimum,
                            valueSpan));
                }

                previous = sample;
            }
        }
    }

    private void DrawHeader(DrawingContext context, Rect paper)
    {
        var brush = new SolidColorBrush(Color.Parse("#2E372B"));
        var title = new FormattedText(
            Title,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            11,
            brush);
        context.DrawText(title, new Point(paper.Left + 8, paper.Top + 5));

        var statusText = string.Concat(
            IsRunning ? "RUN" : "PAUSE",
            " | ",
            PaperSpeed.ToString("0.##", CultureInfo.InvariantCulture),
            " mm/s");

        var status = new FormattedText(
            statusText,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            9,
            brush);

        context.DrawText(
            status,
            new Point(paper.Right - status.Width - 8, paper.Top + 6));
    }

    private static Point MapPoint(
        Rect plot,
        SignalSample sample,
        double windowStart,
        double timeSpan,
        double minimum,
        double valueSpan)
    {
        var xFraction = Math.Clamp(
            (sample.TimestampSeconds - windowStart) / timeSpan,
            0,
            1);
        var yFraction = Math.Clamp(
            (sample.Value - minimum) / valueSpan,
            0,
            1);

        return new Point(
            plot.Left + (plot.Width * xFraction),
            plot.Bottom - (plot.Height * yFraction));
    }
}
