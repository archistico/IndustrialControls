using System;
using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Trend multicanale con griglia, legenda e cursore temporale.
/// </summary>
public sealed class TrendChart : TimeSeriesControlBase
{
    private static readonly IBrush FrameBrush =
        new SolidColorBrush(Color.Parse("#080B0C"));
    private static readonly IBrush PlotBrush =
        new SolidColorBrush(Color.Parse("#101719"));
    private static readonly IBrush TitleBrush =
        new SolidColorBrush(Color.Parse("#E5E7DE"));
    private static readonly IBrush LabelBrush =
        new SolidColorBrush(Color.Parse("#A8ADA8"));
    private static readonly IBrush LegendBrush =
        new SolidColorBrush(Color.Parse("#D8DBD4"));
    private static readonly IBrush UncertainBrush =
        new SolidColorBrush(Color.Parse("#E3C83B"));
    private static readonly IBrush BadBrush =
        new SolidColorBrush(Color.Parse("#F14C4C"));
    private static readonly IBrush UnavailableBrush =
        new SolidColorBrush(Color.Parse("#7B7F80"));

    private static readonly Pen FramePen =
        new(new SolidColorBrush(Color.Parse("#111315")), 5);
    private static readonly Pen EdgePen =
        new(new SolidColorBrush(Color.Parse("#7A8286")), 1);
    private static readonly Pen PlotPen =
        new(new SolidColorBrush(Color.Parse("#35464B")), 1);
    private static readonly Pen MinorGridPen =
        new(new SolidColorBrush(Color.Parse("#243236")), 1);
    private static readonly Pen MajorGridPen =
        new(new SolidColorBrush(Color.Parse("#34474C")), 1);
    private static readonly Pen CursorPen =
        new(new SolidColorBrush(Color.Parse("#F1F1DF")), 1);
    private static readonly Pen UncertainPen =
        new(UncertainBrush, 2);

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

    private readonly StringBuilder _cursorBuilder = new(256);
    private string _cursorReadout = "NO DATA";
    private bool _cursorReadoutDirty = true;

    static TrendChart()
    {
        AffectsRender<TrendChart>(
            ShowCursorProperty,
            CursorFractionProperty,
            HorizontalGridDivisionsProperty,
            VerticalGridDivisionsProperty);

        CursorFractionProperty.Changed.AddClassHandler<TrendChart>(
            (control, _) => control.MarkCursorReadoutDirty());
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
        get
        {
            EnsureCursorReadout();
            return _cursorReadout;
        }

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
        context.DrawRectangle(FrameBrush, FramePen, frame);
        context.DrawRectangle(null, EdgePen, frame.Deflate(5));

        var plot = new Rect(
            58,
            30,
            Math.Max(20, Bounds.Width - 76),
            Math.Max(20, Bounds.Height - (ShowLegend ? 82 : 52)));

        context.DrawRectangle(PlotBrush, PlotPen, plot);

        var range = GetEffectiveRange();
        var windowEnd = LatestTimeSeconds;
        var windowStart = windowEnd - TimeWindowSeconds;

        if (ShowGrid)
        {
            DrawGrid(
                context,
                plot,
                range.Minimum,
                range.Maximum,
                windowStart,
                windowEnd);
        }

        DrawSeries(
            context,
            plot,
            range.Minimum,
            range.Maximum,
            windowStart,
            windowEnd);

        if (ShowCursor)
        {
            var cursorX = plot.X + (plot.Width * CursorFraction);
            context.DrawLine(
                CursorPen,
                new Point(cursorX, plot.Top),
                new Point(cursorX, plot.Bottom));
        }

        if (ShowLegend)
        {
            DrawLegend(context, plot);
        }

        DrawTitle(context);
    }

    protected override void OnSeriesChanged() =>
        MarkCursorReadoutDirty();

    protected override void OnSamplesChanged() =>
        MarkCursorReadoutDirty();

    private void DrawTitle(DrawingContext context)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            return;
        }

        context.DrawText(
            CreateText(Title, 12, TitleBrush),
            new Point(12, 8));
    }

    private void DrawGrid(
        DrawingContext context,
        Rect plot,
        double minimum,
        double maximum,
        double windowStart,
        double windowEnd)
    {
        for (var index = 0; index <= HorizontalGridDivisions; index++)
        {
            var fraction = index / (double)HorizontalGridDivisions;
            var x = plot.Left + (plot.Width * fraction);

            context.DrawLine(
                index is 0 || index == HorizontalGridDivisions
                    ? MajorGridPen
                    : MinorGridPen,
                new Point(x, plot.Top),
                new Point(x, plot.Bottom));

            var seconds =
                windowStart +
                ((windowEnd - windowStart) * fraction);

            var label = CreateText(
                seconds.ToString(
                    "0.0",
                    CultureInfo.InvariantCulture) + " s",
                9,
                LabelBrush);

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
                    ? MajorGridPen
                    : MinorGridPen,
                new Point(plot.Left, y),
                new Point(plot.Right, y));

            var value =
                minimum +
                ((maximum - minimum) * fraction);

            var label = CreateText(
                value.ToString(
                    "0.##",
                    CultureInfo.InvariantCulture),
                9,
                LabelBrush);

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
        foreach (var series in TraceSeries)
        {
            if (!series.IsVisible)
            {
                continue;
            }

            var samples = series.Samples;
            var visibleCount = 0;
            var lastVisibleIndex = -1;

            for (var index = 0; index < samples.Count; index++)
            {
                var timestamp = samples[index].TimestampSeconds;
                if (timestamp < windowStart ||
                    timestamp > windowEnd)
                {
                    continue;
                }

                visibleCount++;
                lastVisibleIndex = index;
            }

            if (visibleCount == 0)
            {
                continue;
            }

            var step = Math.Max(
                1,
                visibleCount / Math.Max(1, (int)plot.Width));

            var visibleIndex = 0;
            SignalSample? previousConnected = null;
            var uncertainSegment = false;

            for (var index = 0; index < samples.Count; index++)
            {
                var sample = samples[index];
                if (sample.TimestampSeconds < windowStart ||
                    sample.TimestampSeconds > windowEnd)
                {
                    continue;
                }

                var mustDraw =
                    visibleIndex % step == 0 ||
                    index == lastVisibleIndex ||
                    sample.Quality is not SignalQuality.Good;

                visibleIndex++;

                if (sample.Quality is
                    SignalQuality.Bad or
                    SignalQuality.Unavailable)
                {
                    previousConnected = null;
                    uncertainSegment = false;

                    if (mustDraw)
                    {
                        DrawSamplePoint(
                            context,
                            MapPoint(
                                plot,
                                minimum,
                                maximum,
                                windowStart,
                                windowEnd,
                                sample),
                            GetPointBrush(sample.Quality));
                    }

                    continue;
                }

                if (sample.Quality == SignalQuality.Uncertain)
                {
                    uncertainSegment = true;
                }

                if (!mustDraw)
                {
                    continue;
                }

                var point = MapPoint(
                    plot,
                    minimum,
                    maximum,
                    windowStart,
                    windowEnd,
                    sample);

                if (previousConnected is { } previousSample)
                {
                    context.DrawLine(
                        uncertainSegment
                            ? UncertainPen
                            : series.TracePen,
                        MapPoint(
                            plot,
                            minimum,
                            maximum,
                            windowStart,
                            windowEnd,
                            previousSample),
                        point);
                }

                DrawSamplePoint(
                    context,
                    point,
                    sample.Quality == SignalQuality.Uncertain
                        ? UncertainBrush
                        : series.TraceBrush);

                previousConnected = sample;
                uncertainSegment =
                    sample.Quality == SignalQuality.Uncertain;
            }
        }
    }

    private static void DrawSamplePoint(
        DrawingContext context,
        Point point,
        IBrush brush) =>
        context.DrawEllipse(
            brush,
            null,
            point,
            2.4,
            2.4);

    private void DrawLegend(
        DrawingContext context,
        Rect plot)
    {
        var x = plot.Left;
        var y = plot.Bottom + 26;

        foreach (var series in TraceSeries)
        {
            if (!series.IsVisible)
            {
                continue;
            }

            context.DrawLine(
                series.TracePen,
                new Point(x, y + 6),
                new Point(x + 18, y + 6));

            var legendText = string.IsNullOrWhiteSpace(series.Unit)
                ? series.Name
                : string.Concat(
                    series.Name,
                    " [",
                    series.Unit,
                    "]");

            var text = CreateText(
                legendText,
                9,
                LegendBrush);

            context.DrawText(
                text,
                new Point(x + 24, y));

            x += 36 + text.Width;

            if (x > plot.Right - 100)
            {
                x = plot.Left;
                y += 16;
            }
        }
    }

    private void MarkCursorReadoutDirty() =>
        _cursorReadoutDirty = true;

    private void EnsureCursorReadout()
    {
        if (!_cursorReadoutDirty)
        {
            return;
        }

        _cursorReadoutDirty = false;

        if (TraceSeries.Count == 0)
        {
            CursorReadout = "NO DATA";
            return;
        }

        var hasSamples = false;
        var cursorTime =
            (LatestTimeSeconds - TimeWindowSeconds) +
            (TimeWindowSeconds * CursorFraction);

        _cursorBuilder.Clear();
        _cursorBuilder.Append(
            cursorTime.ToString(
                "0.0",
                CultureInfo.InvariantCulture));
        _cursorBuilder.Append(" s");

        foreach (var series in TraceSeries)
        {
            if (!series.IsVisible ||
                series.Samples.Count == 0)
            {
                continue;
            }

            hasSamples = true;
            var samples = series.Samples;
            var nearest = samples[0];
            var nearestDistance =
                Math.Abs(nearest.TimestampSeconds - cursorTime);

            for (var index = 1; index < samples.Count; index++)
            {
                var candidate = samples[index];
                var distance =
                    Math.Abs(candidate.TimestampSeconds - cursorTime);

                if (distance >= nearestDistance)
                {
                    continue;
                }

                nearest = candidate;
                nearestDistance = distance;
            }

            _cursorBuilder.Append(" | ");
            _cursorBuilder.Append(series.Name);
            _cursorBuilder.Append(": ");
            _cursorBuilder.Append(
                nearest.Value.ToString(
                    "0.###",
                    CultureInfo.InvariantCulture));

            if (!string.IsNullOrWhiteSpace(series.Unit))
            {
                _cursorBuilder.Append(' ');
                _cursorBuilder.Append(series.Unit);
            }

            _cursorBuilder.Append(" [");
            _cursorBuilder.Append(
                nearest.Quality
                    .ToString()
                    .ToUpperInvariant());
            _cursorBuilder.Append(']');
        }

        CursorReadout = hasSamples
            ? _cursorBuilder.ToString()
            : "NO DATA";
    }

    private static IBrush GetPointBrush(
        SignalQuality quality) =>
        quality switch
        {
            SignalQuality.Bad => BadBrush,
            SignalQuality.Unavailable => UnavailableBrush,
            SignalQuality.Uncertain => UncertainBrush,
            _ => LegendBrush
        };

    private static Point MapPoint(
        Rect plot,
        double minimum,
        double maximum,
        double windowStart,
        double windowEnd,
        SignalSample sample)
    {
        var timeSpan = Math.Max(
            1e-12,
            windowEnd - windowStart);
        var valueSpan = Math.Max(
            1e-12,
            maximum - minimum);

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
