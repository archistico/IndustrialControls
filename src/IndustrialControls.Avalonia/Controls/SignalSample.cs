namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Campione temporale immutabile usato dai controlli M6.
/// </summary>
public readonly record struct SignalSample(
    double TimestampSeconds,
    double Value,
    SignalQuality Quality);
