using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Contratto comune per trend e registratori temporali.
/// </summary>
public abstract class TimeSeriesControlBase : Control
{
    private static readonly Color UncertainColor = Color.Parse("#E3C83B");
    private static readonly Color BadColor = Color.Parse("#F14C4C");
    private static readonly Color UnavailableColor = Color.Parse("#7B7F80");

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
    private readonly Dictionary<string, SignalTraceSeries> _seriesByName =
        new(StringComparer.OrdinalIgnoreCase);

    private int _seriesCount;
    private double _latestTimeSeconds;
    private bool _hasSamples;

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

        TitleProperty.Changed.AddClassHandler<TimeSeriesControlBase>(
            (control, _) => control.RefreshAutomationMetadata());

        MaxSamplesPerSeriesProperty.Changed.AddClassHandler<TimeSeriesControlBase>(
            (control, _) => control.TrimAllSeriesToCapacity());
    }

    protected TimeSeriesControlBase() => RefreshAutomationMetadata();

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
        private set => SetAndRaise(
            SeriesCountProperty,
            ref _seriesCount,
            value);
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

    public SignalTraceSeries AddSeries(
        string name,
        string unit,
        Color color)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (_seriesByName.ContainsKey(name))
        {
            throw new InvalidOperationException(
                $"A series named '{name}' already exists.");
        }

        var created = new SignalTraceSeries(name, unit, color);
        _series.Add(created);
        _seriesByName.Add(name, created);

        SeriesCount = _series.Count;
        OnSeriesChanged();
        RefreshAutomationMetadata();
        InvalidateVisual();
        return created;
    }

    public virtual bool AddSample(
        string seriesName,
        double timestampSeconds,
        double value,
        SignalQuality quality = SignalQuality.Good)
    {
        if (!_seriesByName.TryGetValue(seriesName, out var series))
        {
            return false;
        }

        return AddSampleCore(
            series,
            timestampSeconds,
            value,
            quality);
    }

    /// <summary>
    /// Aggiunge un campione usando direttamente l'handle della serie,
    /// evitando la ricerca per nome nei percorsi di acquisizione ad alta frequenza.
    /// </summary>
    public virtual bool AddSample(
        SignalTraceSeries series,
        double timestampSeconds,
        double value,
        SignalQuality quality = SignalQuality.Good)
    {
        ArgumentNullException.ThrowIfNull(series);

        if (!_seriesByName.TryGetValue(series.Name, out var registered) ||
            !ReferenceEquals(series, registered))
        {
            return false;
        }

        return AddSampleCore(
            series,
            timestampSeconds,
            value,
            quality);
    }

    public bool SetSeriesVisibility(
        string seriesName,
        bool isVisible)
    {
        if (!_seriesByName.TryGetValue(seriesName, out var series))
        {
            return false;
        }

        if (series.IsVisible == isVisible)
        {
            return true;
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

        _hasSamples = false;
        LatestTimeSeconds = 0;
        OnSamplesChanged();
        RefreshAutomationMetadata();
        InvalidateVisual();
    }

    public (double Minimum, double Maximum) GetEffectiveRange()
    {
        if (!AutoScale)
        {
            return NormalizeRange(Minimum, Maximum);
        }

        var foundValue = false;
        var minimum = double.PositiveInfinity;
        var maximum = double.NegativeInfinity;

        foreach (var series in _series)
        {
            if (!series.IsVisible)
            {
                continue;
            }

            var samples = series.Samples;
            for (var index = 0; index < samples.Count; index++)
            {
                var sample = samples[index];
                if (sample.Quality is not
                    (SignalQuality.Good or SignalQuality.Uncertain))
                {
                    continue;
                }

                foundValue = true;
                minimum = Math.Min(minimum, sample.Value);
                maximum = Math.Max(maximum, sample.Value);
            }
        }

        if (!foundValue)
        {
            return NormalizeRange(Minimum, Maximum);
        }

        if (Math.Abs(maximum - minimum) < 1e-12)
        {
            var padding = Math.Max(1.0, Math.Abs(minimum) * 0.05);
            return (minimum - padding, maximum + padding);
        }

        var range = maximum - minimum;
        var margin = range * 0.08;
        return (minimum - margin, maximum + margin);
    }

    protected SignalTraceSeries? FindSeries(string seriesName) =>
        _seriesByName.TryGetValue(seriesName, out var series)
            ? series
            : null;

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
            SignalQuality.Uncertain => UncertainColor,
            SignalQuality.Bad => BadColor,
            SignalQuality.Unavailable => UnavailableColor,
            _ => goodColor
        };

    private bool AddSampleCore(
        SignalTraceSeries series,
        double timestampSeconds,
        double value,
        SignalQuality quality)
    {
        if (!double.IsFinite(timestampSeconds) ||
            !double.IsFinite(value))
        {
            return false;
        }

        var firstSample = !_hasSamples;

        series.Add(
            new SignalSample(timestampSeconds, value, quality),
            MaxSamplesPerSeries);

        if (firstSample || timestampSeconds > LatestTimeSeconds)
        {
            LatestTimeSeconds = timestampSeconds;
        }

        _hasSamples = true;
        OnSamplesChanged();
        InvalidateVisual();
        return true;
    }

    private void RefreshAutomationMetadata()
    {
        IndustrialAutomationMetadata.Apply(
            this,
            Title,
            string.Concat(
                SeriesCount,
                " series; bounded history ",
                MaxSamplesPerSeries,
                " samples per series"),
            GetType().Name);
    }

    private void TrimAllSeriesToCapacity()
    {
        foreach (var series in _series)
        {
            series.TrimToCapacity(MaxSamplesPerSeries);
        }

        OnSamplesChanged();
        RefreshAutomationMetadata();
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
