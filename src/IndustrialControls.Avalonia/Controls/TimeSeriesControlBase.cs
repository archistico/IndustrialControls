using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Contratto comune per trend e registratori temporali.
/// </summary>
public abstract class TimeSeriesControlBase : Control
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<TimeSeriesControlBase, string>(
            nameof(Title), string.Empty);

    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<TimeSeriesControlBase, double>(
            nameof(Minimum), 0.0);

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<TimeSeriesControlBase, double>(
            nameof(Maximum), 100.0);

    public static readonly StyledProperty<double> TimeWindowSecondsProperty =
        AvaloniaProperty.Register<TimeSeriesControlBase, double>(
            nameof(TimeWindowSeconds), 60.0, validate: value => value > 0);

    public static readonly StyledProperty<int> MaxSamplesPerSeriesProperty =
        AvaloniaProperty.Register<TimeSeriesControlBase, int>(
            nameof(MaxSamplesPerSeries),
            600,
            validate: value => value is >= 10 and <= 100_000);

    public static readonly StyledProperty<bool> AutoScaleProperty =
        AvaloniaProperty.Register<TimeSeriesControlBase, bool>(
            nameof(AutoScale));

    public static readonly StyledProperty<bool> ShowGridProperty =
        AvaloniaProperty.Register<TimeSeriesControlBase, bool>(
            nameof(ShowGrid), true);

    public static readonly StyledProperty<bool> ShowLegendProperty =
        AvaloniaProperty.Register<TimeSeriesControlBase, bool>(
            nameof(ShowLegend), true);

    public static readonly DirectProperty<TimeSeriesControlBase, int> SeriesCountProperty =
        AvaloniaProperty.RegisterDirect<TimeSeriesControlBase, int>(
            nameof(SeriesCount), control => control.SeriesCount);

    public static readonly DirectProperty<TimeSeriesControlBase, double> LatestTimeSecondsProperty =
        AvaloniaProperty.RegisterDirect<TimeSeriesControlBase, double>(
            nameof(LatestTimeSeconds), control => control.LatestTimeSeconds);

    private readonly List<SignalTraceSeries> _series = new();
    private int _seriesCount;
    private double _latestTimeSeconds;

    static TimeSeriesControlBase()
    {
        AffectsRender<TimeSeriesControlBase>(
            TitleProperty,
            MinimumProperty,
            MaximumProperty,
            TimeWindowSecondsProperty,
            AutoScaleProperty,
            ShowGridProperty,
            ShowLegendProperty);

        MaxSamplesPerSeriesProperty.Changed.AddClassHandler<TimeSeriesControlBase>(
            (control, _) => control.TrimAllSeriesToCapacity());
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public double TimeWindowSeconds
    {
        get => GetValue(TimeWindowSecondsProperty);
        set => SetValue(TimeWindowSecondsProperty, value);
    }

    public int MaxSamplesPerSeries
    {
        get => GetValue(MaxSamplesPerSeriesProperty);
        set => SetValue(MaxSamplesPerSeriesProperty, value);
    }

    public bool AutoScale
    {
        get => GetValue(AutoScaleProperty);
        set => SetValue(AutoScaleProperty, value);
    }

    public bool ShowGrid
    {
        get => GetValue(ShowGridProperty);
        set => SetValue(ShowGridProperty, value);
    }

    public bool ShowLegend
    {
        get => GetValue(ShowLegendProperty);
        set => SetValue(ShowLegendProperty, value);
    }

    public int SeriesCount
    {
        get => _seriesCount;
        private set => SetAndRaise(SeriesCountProperty, ref _seriesCount, value);
    }

    public double LatestTimeSeconds
    {
        get => _latestTimeSeconds;
        private set => SetAndRaise(
            LatestTimeSecondsProperty,
            ref _latestTimeSeconds,
            value);
    }

    public IReadOnlyList<SignalTraceSeries> TraceSeries => _series;

    public SignalTraceSeries AddSeries(string name, string unit, Color color)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_series.Any(series =>
                string.Equals(series.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                $"A series named '{name}' already exists.");
        }

        var created = new SignalTraceSeries(name, unit, color);
        _series.Add(created);
        SeriesCount = _series.Count;
        OnSeriesChanged();
        InvalidateVisual();
        return created;
    }

    public virtual bool AddSample(
        string seriesName,
        double timestampSeconds,
        double value,
        SignalQuality quality = SignalQuality.Good)
    {
        var series = FindSeries(seriesName);
        if (series is null)
        {
            return false;
        }

        if (!double.IsFinite(timestampSeconds) || !double.IsFinite(value))
        {
            return false;
        }

        series.Add(
            new SignalSample(timestampSeconds, value, quality),
            MaxSamplesPerSeries);

        if (timestampSeconds > LatestTimeSeconds || TotalSampleCount == 1)
        {
            LatestTimeSeconds = timestampSeconds;
        }

        OnSamplesChanged();
        InvalidateVisual();
        return true;
    }

    public bool SetSeriesVisibility(string seriesName, bool isVisible)
    {
        var series = FindSeries(seriesName);
        if (series is null)
        {
            return false;
        }

        series.IsVisible = isVisible;
        OnSeriesChanged();
        InvalidateVisual();
        return true;
    }

    public void ClearSamples()
    {
        foreach (var series in _series)
        {
            series.Clear();
        }

        LatestTimeSeconds = 0;
        OnSamplesChanged();
        InvalidateVisual();
    }

    public (double Minimum, double Maximum) GetEffectiveRange()
    {
        if (!AutoScale)
        {
            return NormalizeRange(Minimum, Maximum);
        }

        var visibleValues = _series
            .Where(series => series.IsVisible)
            .SelectMany(series => series.Samples)
            .Where(sample =>
                sample.Quality is SignalQuality.Good or SignalQuality.Uncertain)
            .Select(sample => sample.Value)
            .ToArray();

        if (visibleValues.Length == 0)
        {
            return NormalizeRange(Minimum, Maximum);
        }

        var minimum = visibleValues.Min();
        var maximum = visibleValues.Max();

        if (Math.Abs(maximum - minimum) < 1e-12)
        {
            var padding = Math.Max(1.0, Math.Abs(minimum) * 0.05);
            return (minimum - padding, maximum + padding);
        }

        var range = maximum - minimum;
        var margin = range * 0.08;
        return (minimum - margin, maximum + margin);
    }

    protected int TotalSampleCount =>
        _series.Sum(series => series.Samples.Count);

    protected SignalTraceSeries? FindSeries(string seriesName) =>
        _series.FirstOrDefault(series =>
            string.Equals(
                series.Name,
                seriesName,
                StringComparison.OrdinalIgnoreCase));

    protected virtual void OnSeriesChanged()
    {
    }

    protected virtual void OnSamplesChanged()
    {
    }

    protected static Color GetQualityColor(
        SignalQuality quality,
        Color goodColor) =>
        quality switch
        {
            SignalQuality.Uncertain => Color.Parse("#E3C83B"),
            SignalQuality.Bad => Color.Parse("#F14C4C"),
            SignalQuality.Unavailable => Color.Parse("#7B7F80"),
            _ => goodColor
        };

    private void TrimAllSeriesToCapacity()
    {
        foreach (var series in _series)
        {
            series.TrimToCapacity(MaxSamplesPerSeries);
        }

        OnSamplesChanged();
        InvalidateVisual();
    }

    private static (double Minimum, double Maximum) NormalizeRange(
        double minimum,
        double maximum)
    {
        if (!double.IsFinite(minimum) ||
            !double.IsFinite(maximum) ||
            maximum <= minimum)
        {
            return (0, 1);
        }

        return (minimum, maximum);
    }
}
