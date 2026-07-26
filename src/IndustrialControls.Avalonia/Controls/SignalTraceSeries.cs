using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Serie temporale identificata da nome, unità e colore.
/// </summary>
public sealed class SignalTraceSeries
{
    private readonly List<SignalSample> _samples = new();

    internal SignalTraceSeries(string name, string unit, Color color)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;
        Unit = unit ?? string.Empty;
        Color = color;
    }

    public string Name { get; }

    public string Unit { get; }

    public Color Color { get; }

    public bool IsVisible { get; internal set; } = true;

    public IReadOnlyList<SignalSample> Samples => _samples;

    public SignalSample? LatestSample =>
        _samples.Count == 0 ? null : _samples[^1];

    internal void Add(SignalSample sample, int capacity)
    {
        _samples.Add(sample);
        TrimToCapacity(capacity);
    }

    internal void TrimToCapacity(int capacity)
    {
        var safeCapacity = Math.Max(1, capacity);
        var excess = _samples.Count - safeCapacity;
        if (excess > 0)
        {
            _samples.RemoveRange(0, excess);
        }
    }

    internal void Clear() => _samples.Clear();
}
