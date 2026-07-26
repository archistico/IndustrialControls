using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Oscilloscopio industriale a singola traccia.
/// </summary>
public sealed class OscilloscopeDisplay : Control
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<OscilloscopeDisplay, string>(
            nameof(Title), string.Empty);

    public static readonly StyledProperty<double> VerticalMinimumProperty =
        AvaloniaProperty.Register<OscilloscopeDisplay, double>(
            nameof(VerticalMinimum), -1.0);

    public static readonly StyledProperty<double> VerticalMaximumProperty =
        AvaloniaProperty.Register<OscilloscopeDisplay, double>(
            nameof(VerticalMaximum), 1.0);

    public static readonly StyledProperty<double> TriggerLevelProperty =
        AvaloniaProperty.Register<OscilloscopeDisplay, double>(
            nameof(TriggerLevel), 0.0);

    public static readonly StyledProperty<double> TimebaseMillisecondsProperty =
        AvaloniaProperty.Register<OscilloscopeDisplay, double>(
            nameof(TimebaseMilliseconds),
            100.0,
            validate: value => value > 0);

    public static readonly StyledProperty<int> MaxSamplesProperty =
        AvaloniaProperty.Register<OscilloscopeDisplay, int>(
            nameof(MaxSamples),
            512,
            validate: value => value is >= 16 and <= 65_536);

    public static readonly StyledProperty<Color> TraceColorProperty =
        AvaloniaProperty.Register<OscilloscopeDisplay, Color>(
            nameof(TraceColor), Color.Parse("#58D46C"));

    public static readonly StyledProperty<SignalQuality> QualityProperty =
        AvaloniaProperty.Register<OscilloscopeDisplay, SignalQuality>(
            nameof(Quality), SignalQuality.Good);

    public static readonly DirectProperty<OscilloscopeDisplay, int> SampleCountProperty =
        AvaloniaProperty.RegisterDirect<OscilloscopeDisplay, int>(
            nameof(SampleCount), control => control.SampleCount);

    public static readonly DirectProperty<OscilloscopeDisplay, double> LastValueProperty =
        AvaloniaProperty.RegisterDirect<OscilloscopeDisplay, double>(
            nameof(LastValue), control => control.LastValue);

    private readonly List<double> _samples = new();
    private int _sampleCount;
    private double _lastValue;

    static OscilloscopeDisplay()
    {
        AffectsRender<OscilloscopeDisplay>(
            TitleProperty,
            VerticalMinimumProperty,
            VerticalMaximumProperty,
            TriggerLevelProperty,
            TimebaseMillisecondsProperty,
            TraceColorProperty,
            QualityProperty);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public double VerticalMinimum
    {
        get => GetValue(VerticalMinimumProperty);
        set => SetValue(VerticalMinimumProperty, value);
    }

    public double VerticalMaximum
    {
        get => GetValue(VerticalMaximumProperty);
        set => SetValue(VerticalMaximumProperty, value);
    }

    public double TriggerLevel
    {
        get => GetValue(TriggerLevelProperty);
        set => SetValue(TriggerLevelProperty, value);
    }

    public double TimebaseMilliseconds
    {
        get => GetValue(TimebaseMillisecondsProperty);
        set => SetValue(TimebaseMillisecondsProperty, value);
    }

    public int MaxSamples
    {
        get => GetValue(MaxSamplesProperty);
        set
        {
            SetValue(MaxSamplesProperty, value);
            TrimToCapacity();
        }
    }

    public Color TraceColor
    {
        get => GetValue(TraceColorProperty);
        set => SetValue(TraceColorProperty, value);
    }

    public SignalQuality Quality
    {
        get => GetValue(QualityProperty);
        set => SetValue(QualityProperty, value);
    }

    public int SampleCount
    {
        get => _sampleCount;
        private set => SetAndRaise(
            SampleCountProperty,
            ref _sampleCount,
            value);
    }

    public double LastValue
    {
        get => _lastValue;
        private set => SetAndRaise(
            LastValueProperty,
            ref _lastValue,
            value);
    }

    public IReadOnlyList<double> Samples => _samples;

    public bool AddSample(double value)
    {
        if (!double.IsFinite(value))
        {
            return false;
        }

        _samples.Add(value);
        TrimToCapacity();
        LastValue = value;
        SampleCount = _samples.Count;
        InvalidateVisual();
        return true;
    }

    public void SetSamples(IEnumerable<double> samples)
    {
        ArgumentNullException.ThrowIfNull(samples);

        _samples.Clear();

        foreach (var sample in samples.Where(double.IsFinite))
        {
            _samples.Add(sample);
        }

        TrimToCapacity();
        SampleCount = _samples.Count;
        LastValue = _samples.Count == 0 ? 0 : _samples[^1];
        InvalidateVisual();
    }

    public void ClearSamples()
    {
        _samples.Clear();
        SampleCount = 0;
        LastValue = 0;
        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width < 100 || Bounds.Height < 80)
        {
            return;
        }

        var frame = new Rect(0, 0, Bounds.Width, Bounds.Height);
        context.DrawRectangle(
            new SolidColorBrush(Color.Parse("#07100C")),
            new Pen(new SolidColorBrush(Color.Parse("#101312")), 6),
            frame);

        context.DrawRectangle(
            null,
            new Pen(new SolidColorBrush(Color.Parse("#6C8177")), 1),
            frame.Deflate(6));

        var plot = new Rect(
            38,
            28,
            Bounds.Width - 52,
            Bounds.Height - 48);

        context.DrawRectangle(
            new SolidColorBrush(Color.Parse("#06120B")),
            null,
            plot);

        DrawGrid(context, plot);
        DrawTrigger(context, plot);
        DrawTrace(context, plot);
        DrawHeader(context);
    }

    private void DrawGrid(DrawingContext context, Rect plot)
    {
        var minorPen = new Pen(
            new SolidColorBrush(Color.Parse("#123323")),
            1);
        var majorPen = new Pen(
            new SolidColorBrush(Color.Parse("#1A4A31")),
            1);

        for (var index = 0; index <= 10; index++)
        {
            var x = plot.Left + (plot.Width * index / 10.0);
            context.DrawLine(
                index == 5 ? majorPen : minorPen,
                new Point(x, plot.Top),
                new Point(x, plot.Bottom));
        }

        for (var index = 0; index <= 8; index++)
        {
            var y = plot.Top + (plot.Height * index / 8.0);
            context.DrawLine(
                index == 4 ? majorPen : minorPen,
                new Point(plot.Left, y),
                new Point(plot.Right, y));
        }
    }

    private void DrawTrigger(DrawingContext context, Rect plot)
    {
        var range = VerticalMaximum - VerticalMinimum;
        if (range <= 0)
        {
            return;
        }

        var fraction = Math.Clamp(
            (TriggerLevel - VerticalMinimum) / range,
            0,
            1);
        var y = plot.Bottom - (plot.Height * fraction);

        context.DrawLine(
            new Pen(new SolidColorBrush(Color.Parse("#E3C83B")), 1),
            new Point(plot.Left, y),
            new Point(plot.Right, y));
    }

    private void DrawTrace(DrawingContext context, Rect plot)
    {
        if (_samples.Count < 2 ||
            Quality is SignalQuality.Bad or SignalQuality.Unavailable)
        {
            return;
        }

        var range = VerticalMaximum - VerticalMinimum;
        if (range <= 0)
        {
            return;
        }

        var color = Quality == SignalQuality.Uncertain
            ? Color.Parse("#E3C83B")
            : TraceColor;

        var pen = new Pen(new SolidColorBrush(color), 2);

        for (var index = 1; index < _samples.Count; index++)
        {
            var previous = MapSample(plot, index - 1, _samples[index - 1], range);
            var current = MapSample(plot, index, _samples[index], range);
            context.DrawLine(pen, previous, current);
        }
    }

    private Point MapSample(
        Rect plot,
        int index,
        double value,
        double range)
    {
        var denominator = Math.Max(1, _samples.Count - 1);
        var x = plot.Left + (plot.Width * index / denominator);
        var fraction = Math.Clamp(
            (value - VerticalMinimum) / range,
            0,
            1);
        var y = plot.Bottom - (plot.Height * fraction);
        return new Point(x, y);
    }

    private void DrawHeader(DrawingContext context)
    {
        var brush = new SolidColorBrush(Color.Parse("#BFE8C9"));
        var title = new FormattedText(
            Title,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            11,
            brush);

        context.DrawText(title, new Point(12, 7));

        var status = new FormattedText(
            string.Concat(
                TimebaseMilliseconds.ToString("0.##", CultureInfo.InvariantCulture),
                " ms | ",
                Quality.ToString().ToUpperInvariant()),
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            Typeface.Default,
            9,
            brush);

        context.DrawText(
            status,
            new Point(Bounds.Width - status.Width - 12, 8));
    }

    private void TrimToCapacity()
    {
        var excess = _samples.Count - MaxSamples;
        if (excess > 0)
        {
            _samples.RemoveRange(0, excess);
        }

        SampleCount = _samples.Count;
    }
}
