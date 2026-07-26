using Avalonia;

namespace IndustrialControls.Avalonia.Controls;

public sealed class RadialGauge : GaugeBase
{
    public static readonly StyledProperty<double> StartAngleProperty =
        AvaloniaProperty.Register<RadialGauge, double>(nameof(StartAngle), -135.0);

    public static readonly StyledProperty<double> SweepAngleProperty =
        AvaloniaProperty.Register<RadialGauge, double>(
            nameof(SweepAngle), 270.0, validate: value => value is > 0 and <= 360);

    public static readonly StyledProperty<int> MajorTickCountProperty =
        AvaloniaProperty.Register<RadialGauge, int>(
            nameof(MajorTickCount), 11, validate: value => value is >= 2 and <= 101);

    public static readonly StyledProperty<int> MinorTicksPerIntervalProperty =
        AvaloniaProperty.Register<RadialGauge, int>(
            nameof(MinorTicksPerInterval), 4, validate: value => value is >= 0 and <= 20);

    public static readonly StyledProperty<int> ScaleDecimalPlacesProperty =
        AvaloniaProperty.Register<RadialGauge, int>(
            nameof(ScaleDecimalPlaces), 0, validate: value => value is >= 0 and <= 4);

    public static readonly StyledProperty<bool> ShowScaleLabelsProperty =
        AvaloniaProperty.Register<RadialGauge, bool>(nameof(ShowScaleLabels), true);

    public static readonly StyledProperty<bool> ShowOperatingBandsProperty =
        AvaloniaProperty.Register<RadialGauge, bool>(nameof(ShowOperatingBands), true);

    public static readonly DirectProperty<RadialGauge, double> NeedleAngleProperty =
        AvaloniaProperty.RegisterDirect<RadialGauge, double>(
            nameof(NeedleAngle), control => control.NeedleAngle);

    private double _needleAngle = -135.0;

    static RadialGauge()
    {
        ValueProperty.Changed.AddClassHandler<RadialGauge>((control, _) => control.RefreshNeedle());
        MinimumProperty.Changed.AddClassHandler<RadialGauge>((control, _) => control.RefreshNeedle());
        MaximumProperty.Changed.AddClassHandler<RadialGauge>((control, _) => control.RefreshNeedle());
        StartAngleProperty.Changed.AddClassHandler<RadialGauge>((control, _) => control.RefreshNeedle());
        SweepAngleProperty.Changed.AddClassHandler<RadialGauge>((control, _) => control.RefreshNeedle());
    }

    public RadialGauge() => RefreshNeedle();

    public double StartAngle
    {
        get => GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    public double SweepAngle
    {
        get => GetValue(SweepAngleProperty);
        set => SetValue(SweepAngleProperty, value);
    }

    public int MajorTickCount
    {
        get => GetValue(MajorTickCountProperty);
        set => SetValue(MajorTickCountProperty, value);
    }

    public int MinorTicksPerInterval
    {
        get => GetValue(MinorTicksPerIntervalProperty);
        set => SetValue(MinorTicksPerIntervalProperty, value);
    }

    public int ScaleDecimalPlaces
    {
        get => GetValue(ScaleDecimalPlacesProperty);
        set => SetValue(ScaleDecimalPlacesProperty, value);
    }

    public bool ShowScaleLabels
    {
        get => GetValue(ShowScaleLabelsProperty);
        set => SetValue(ShowScaleLabelsProperty, value);
    }

    public bool ShowOperatingBands
    {
        get => GetValue(ShowOperatingBandsProperty);
        set => SetValue(ShowOperatingBandsProperty, value);
    }

    public double NeedleAngle
    {
        get => _needleAngle;
        private set => SetAndRaise(NeedleAngleProperty, ref _needleAngle, value);
    }

    public double GetAngleForValue(double value)
    {
        var span = Maximum - Minimum;
        var normalized = span > 0
            ? System.Math.Clamp((value - Minimum) / span, 0.0, 1.0)
            : 0.0;

        return StartAngle + (normalized * SweepAngle);
    }

    private void RefreshNeedle() => NeedleAngle = GetAngleForValue(Value);
}
