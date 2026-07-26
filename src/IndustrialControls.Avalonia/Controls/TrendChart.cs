using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Avalonia;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Trend multicanale con griglia, legenda e cursore temporale.
/// </summary>
public sealed class TrendChart : TimeSeriesControlBase
{
    public static readonly StyledProperty<bool> ShowCursorProperty =
        AvaloniaProperty.Register<TrendChart, bool>(
            nameof(ShowCursor), true);

    public static readonly StyledProperty<double> CursorFractionProperty =
        AvaloniaProperty.Register<TrendChart, double>(
            nameof(CursorFraction),
            0.75,
            validate: value => value is >= 0 and <= 1);

    public static readonly StyledProperty<int> HorizontalGridDivisionsProperty =
        AvaloniaProperty.Register<TrendChart, int>(
            nameof(HorizontalGridDivisions),
            6,
            validate: value => value is >= 2 and <= 20);

    public static readonly StyledProperty<int> VerticalGridDivisionsProperty =
        AvaloniaProperty.Register<TrendChart, int>(
            nameof(VerticalGridDivisions),
            5,
            validate: value => value is >= 2 and <= 20);

    public static readonly DirectProperty<TrendChart, string> CursorReadoutProperty =
        AvaloniaProperty.RegisterDirect<TrendChart, string>(
            nameof(CursorReadout), control => control.CursorReadout);

    private string _cursorReadout = "NO DATA";

    static TrendChart()
    {
        AffectsRender<TrendChart>(
            ShowCursorProperty,
            CursorFractionProperty,
            HorizontalGridDivisionsProperty,
            VerticalGridDivisionsProperty);

        CursorFractionProperty.Changed.AddClassHandler<TrendChart>(
            (control, _) => control.RefreshCursorReadout());
    }

    public bool ShowCursor
    {
        get => GetValue(ShowCursorProperty);
        set => SetValue(ShowCursorProperty, value);
    }

    public double CursorFraction
    {
        get => GetValue(CursorFractionProperty);
        set => SetValue(CursorFractionProperty, value);
    }

    public int HorizontalGridDivisions
    {
        get => GetValue(HorizontalGridDivisionsProperty);
        set => SetValue(HorizontalGridDivisionsProperty, value);
    }

    public int VerticalGridDivisions
    {
        get => GetValue(VerticalGridDivisionsProperty);
        set => SetValue(VerticalGridDivisionsProperty, value);
    }

    public string CursorReadout
    {
        get => _cursorReadout;
        private set => SetAndRaise(
            CursorReadoutProperty,
            ref _cursorReadout,
            value);
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
            new SolidColorBrush(Color.Parse("#080B0C")),
            new Pen(new SolidColorBrush(Color.Parse("#111315")), 5),
            frame);

        context.DrawRectangle(
            null,
            new Pen(new SolidColorBrush(Color.Parse("#7A8286")), 1),
            frame.Deflate(5));

        var plot = new Rect(
            58,
            30,
            Math.Max(20, Bounds.Width - 76),
            Math.Max(20, Bounds.Height - (ShowLegend ? 82 : 52)));

        context.DrawRectangle(
            new SolidColorBrush(Color.Parse("#101719")),
            new Pen(new SolidColorBrush(Color.Parse("#35464B")), 1),
            plot);

        var range = GetEffectiveRange();
        var windowEnd = LatestTimeSeconds;
        var windowStart = windowEnd - TimeWindowSeconds;

        if (ShowGrid)
        {
            DrawGrid(context, plot, range.Minimum, range.Maximum, windowStart, windowEnd);
        }

        DrawSeries(context, plot, range.Minimum, range.Maximum, windowStart, windowEnd);

        if (ShowCursor)
        {
            var cursorX = plot.X + (plot.Width * CursorFraction);
            context.DrawLine(
                new Pen(new SolidColorBrush(Color.Parse("#F1F1DF")), 1),
                new Point(cursorX, plot.Top),
                new Point(cursorX, plot.Bottom));
        }

        if (ShowLegend)
        {
            DrawLegend(context, plot);
        }

        DrawTitle(context);
    }

    protected override void OnSeriesChanged() => RefreshCursorReadout();

    protected override void OnSamplesChanged() => RefreshCursorReadout();

    private void DrawTitle(DrawingContext context)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            return;
        }

        var text = CreateText(
            Title,
            12,
            new SolidColorBrush(Color.Parse("#E5E7DE")));

        context.DrawText(text, new Point(12, 8));
    }

    private void DrawGrid(
        DrawingContext context,
        Rect plot,
        double minimum,
        double maximum,
        double windowStart,
        double windowEnd)
    {
        var minorPen = new Pen(
            new SolidColorBrush(Color.Parse("#243236")),
            1);
        var majorPen = new Pen(
            new SolidColorBrush(Color.Parse("#34474C")),
            1);
        var labelBrush = new SolidColorBrush(Color.Parse("#A8ADA8"));

        for (var index = 0; index <= HorizontalGridDivisions; index++)
        {
            var fraction = index / (double)HorizontalGridDivisions;
            var x = plot.Left + (plot.Width * fraction);
            context.DrawLine(
                index is 0 || index == HorizontalGridDivisions
                    ? majorPen
                    : minorPen,
                new Point(x, plot.Top),
                new Point(x, plot.Bottom));

            var seconds = windowStart + ((windowEnd - windowStart) * fraction);
            var label = CreateText(
                seconds.ToString("0.0", CultureInfo.InvariantCulture) + " s",
                9,
                labelBrush);

            context.DrawText(
                label,
                new Point(
                    x - (label.Width / 2.0),
                    plot.Bottom + 5));
        }

        for (var index = 0; index <= VerticalGridDivisions; index++)
        {
            var fraction = index / (double)VerticalGridDivisions;
            var y = plot.Bottom - (plot.Height * fraction);
            context.DrawLine(
                index is 0 || index == VerticalGridDivisions
                    ? majorPen
                    : minorPen,
                new Point(plot.Left, y),
                new Point(plot.Right, y));

            var value = minimum + ((maximum - minimum) * fraction);
            var label = CreateText(
                value.ToString("0.##", CultureInfo.InvariantCulture),
                9,
                labelBrush);

            context.DrawText(
                label,
                new Point(
                    plot.Left - label.Width - 7,
                    y - (label.Height / 2.0)));
        }
    }

    private void DrawSeries(
        DrawingContext context,
        Rect plot,
        double minimum,
        double maximum,
        double windowStart,
        double windowEnd)
    {
        foreach (var series in TraceSeries.Where(series => series.IsVisible))
        {
            var visibleSamples = series.Samples
                .Where(sample =>
                    sample.TimestampSeconds >= windowStart &&
                    sample.TimestampSeconds <= windowEnd)
                .ToList();

            if (visibleSamples.Count == 0)
            {
                continue;
            }

            var step = Math.Max(1, visibleSamples.Count / Math.Max(1, (int)plot.Width));
            var decimated = step == 1
                ? visibleSamples
                : visibleSamples.Where((_, index) => index % step == 0).ToList();

            SignalSample? previousConnected = null;

            foreach (var sample in decimated)
            {
                var point = MapPoint(
                    plot,
                    minimum,
                    maximum,
                    windowStart,
                    windowEnd,
                    sample);

                if (sample.Quality is SignalQuality.Good or SignalQuality.Uncertain)
                {
                    if (previousConnected is { } previousSample)
                    {
                        var color = sample.Quality == SignalQuality.Uncertain ||
                                    previousSample.Quality == SignalQuality.Uncertain
                            ? GetQualityColor(SignalQuality.Uncertain, series.Color)
                            : series.Color;

                        context.DrawLine(
                            new Pen(new SolidColorBrush(color), 2),
                            MapPoint(
                                plot,
                                minimum,
                                maximum,
                                windowStart,
                                windowEnd,
                                previousSample),
                            point);
                    }

                    previousConnected = sample;
                }
                else
                {
                    previousConnected = null;
                }

                DrawSamplePoint(
                    context,
                    point,
                    GetQualityColor(sample.Quality, series.Color),
                    sample.Quality);
            }
        }
    }

    private static void DrawSamplePoint(
        DrawingContext context,
        Point point,
        Color color,
        SignalQuality quality)
    {
        var brush = new SolidColorBrush(color);

        if (quality is SignalQuality.Bad or SignalQuality.Unavailable)
        {
            context.DrawEllipse(
                brush,
                null,
                point,
                3.2,
                3.2);
            return;
        }

        context.DrawEllipse(
            brush,
            null,
            point,
            2.4,
            2.4);
    }

    private void DrawLegend(DrawingContext context, Rect plot)
    {
        var x = plot.Left;
        var y = plot.Bottom + 26;

        foreach (var series in TraceSeries.Where(series => series.IsVisible))
        {
            context.DrawLine(
                new Pen(new SolidColorBrush(series.Color), 3),
                new Point(x, y + 6),
                new Point(x + 18, y + 6));

            var legendText = string.IsNullOrWhiteSpace(series.Unit)
                ? series.Name
                : string.Concat(series.Name, " [", series.Unit, "]");

            var text = CreateText(
                legendText,
                9,
                new SolidColorBrush(Color.Parse("#D8DBD4")));

            context.DrawText(text, new Point(x + 24, y));
            x += 36 + text.Width;

            if (x > plot.Right - 100)
            {
                x = plot.Left;
                y += 16;
            }
        }
    }

    private void RefreshCursorReadout()
    {
        if (TraceSeries.Count == 0 ||
            TraceSeries.All(series => series.Samples.Count == 0))
        {
            CursorReadout = "NO DATA";
            return;
        }

        var cursorTime =
            (LatestTimeSeconds - TimeWindowSeconds) +
            (TimeWindowSeconds * CursorFraction);

        var builder = new StringBuilder();
        builder.Append(cursorTime.ToString("0.0", CultureInfo.InvariantCulture));
        builder.Append(" s");

        foreach (var series in TraceSeries.Where(series => series.IsVisible))
        {
            var nearest = series.Samples
                .OrderBy(sample =>
                    Math.Abs(sample.TimestampSeconds - cursorTime))
                .FirstOrDefault();

            if (series.Samples.Count == 0)
            {
                continue;
            }

            builder.Append(" | ");
            builder.Append(series.Name);
            builder.Append(": ");
            builder.Append(nearest.Value.ToString("0.###", CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(series.Unit))
            {
                builder.Append(' ');
                builder.Append(series.Unit);
            }

            builder.Append(" [");
            builder.Append(nearest.Quality.ToString().ToUpperInvariant());
            builder.Append(']');
        }

        CursorReadout = builder.ToString();
    }

    private static Point MapPoint(
        Rect plot,
        double minimum,
        double maximum,
        double windowStart,
        double windowEnd,
        SignalSample sample)
    {
        var timeSpan = Math.Max(1e-12, windowEnd - windowStart);
        var valueSpan = Math.Max(1e-12, maximum - minimum);

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

    private static FormattedText CreateText(
        string text,
        double size,
        IBrush brush) =>
        new(
            text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            size,
            brush);
}
