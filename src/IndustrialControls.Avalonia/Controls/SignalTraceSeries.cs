using System;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Serie temporale identificata da nome, unità e colore.
/// </summary>
public sealed class SignalTraceSeries
{
    private readonly BoundedSampleCollection _samples = new();

    internal SignalTraceSeries(
        string name,
        string unit,
        Color color)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Unit = unit ?? string.Empty;
        Color = color;
        TraceBrush = new SolidColorBrush(color);
        TracePen = new Pen(TraceBrush, 2);
    }

    public string Name { get; }

    public string Unit { get; }

    public Color Color { get; }

    public bool IsVisible { get; internal set; } = true;

    public IReadOnlyList<SignalSample> Samples => _samples;

    public SignalSample? LatestSample =>
        _samples.Count == 0
            ? null
            : _samples[^1];

    internal IBrush TraceBrush { get; }

    internal Pen TracePen { get; }

    internal void Add(
        SignalSample sample,
        int capacity) =>
        _samples.Add(sample, capacity);

    internal void TrimToCapacity(int capacity) =>
        _samples.TrimToCapacity(capacity);

    internal void Clear() => _samples.Clear();
}
