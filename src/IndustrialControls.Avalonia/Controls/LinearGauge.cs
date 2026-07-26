using Avalonia;

namespace IndustrialControls.Avalonia.Controls;

public sealed class LinearGauge : GaugeBase
{
    public static readonly StyledProperty<GaugeOrientation> OrientationProperty =
        AvaloniaProperty.Register<LinearGauge, GaugeOrientation>(
            nameof(Orientation), GaugeOrientation.Horizontal);

    public static readonly StyledProperty<bool> ShowScaleProperty =
        AvaloniaProperty.Register<LinearGauge, bool>(nameof(ShowScale), true);

    public GaugeOrientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public bool ShowScale
    {
        get => GetValue(ShowScaleProperty);
        set => SetValue(ShowScaleProperty, value);
    }
}
