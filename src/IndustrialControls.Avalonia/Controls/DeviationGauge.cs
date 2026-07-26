using System.Globalization;
using Avalonia;

namespace IndustrialControls.Avalonia.Controls;

public sealed class DeviationGauge : GaugeBase
{
    public static readonly StyledProperty<double> SetpointProperty =
        AvaloniaProperty.Register<DeviationGauge, double>(nameof(Setpoint), 0.0);

    public static readonly StyledProperty<double> DeadbandProperty =
        AvaloniaProperty.Register<DeviationGauge, double>(
            nameof(Deadband), 0.0, validate: value => value >= 0);

    public static readonly DirectProperty<DeviationGauge, double> DeviationProperty =
        AvaloniaProperty.RegisterDirect<DeviationGauge, double>(
            nameof(Deviation), control => control.Deviation);

    public static readonly DirectProperty<DeviationGauge, string> FormattedDeviationProperty =
        AvaloniaProperty.RegisterDirect<DeviationGauge, string>(
            nameof(FormattedDeviation), control => control.FormattedDeviation);

    private double _deviation;
    private string _formattedDeviation = "+0.0";

    static DeviationGauge()
    {
        ValueProperty.Changed.AddClassHandler<DeviationGauge>((control, _) => control.RefreshDeviation());
        SetpointProperty.Changed.AddClassHandler<DeviationGauge>((control, _) => control.RefreshDeviation());
        DecimalPlacesProperty.Changed.AddClassHandler<DeviationGauge>((control, _) => control.RefreshDeviation());
    }

    public DeviationGauge()
    {
        Minimum = -100;
        Maximum = 100;
        RefreshDeviation();
    }

    public double Setpoint
    {
        get => GetValue(SetpointProperty);
        set => SetValue(SetpointProperty, value);
    }

    public double Deadband
    {
        get => GetValue(DeadbandProperty);
        set => SetValue(DeadbandProperty, value);
    }

    public double Deviation
    {
        get => _deviation;
        private set => SetAndRaise(DeviationProperty, ref _deviation, value);
    }

    public string FormattedDeviation
    {
        get => _formattedDeviation;
        private set => SetAndRaise(FormattedDeviationProperty, ref _formattedDeviation, value);
    }

    private void RefreshDeviation()
    {
        Deviation = Value - Setpoint;

        var integerPattern = "0";
        var decimalPattern = DecimalPlaces > 0
            ? "." + new string('0', DecimalPlaces)
            : string.Empty;

        var format = string.Concat(
            "+", integerPattern, decimalPattern,
            ";-", integerPattern, decimalPattern,
            ";0");

        FormattedDeviation = Deviation.ToString(format, CultureInfo.InvariantCulture);
    }
}
