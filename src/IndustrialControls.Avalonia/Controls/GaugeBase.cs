using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

public abstract class GaugeBase : TemplatedControl
{
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<GaugeBase, double>(nameof(Minimum), 0.0);

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<GaugeBase, double>(nameof(Maximum), 100.0);

    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<GaugeBase, double>(nameof(Value), 0.0);

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<GaugeBase, string>(nameof(Title), string.Empty);

    public static readonly StyledProperty<string> UnitProperty =
        AvaloniaProperty.Register<GaugeBase, string>(nameof(Unit), string.Empty);

    public static readonly StyledProperty<int> DecimalPlacesProperty =
        AvaloniaProperty.Register<GaugeBase, int>(
            nameof(DecimalPlaces), 1, validate: value => value is >= 0 and <= 8);

    public static readonly StyledProperty<double> CautionLowProperty =
        AvaloniaProperty.Register<GaugeBase, double>(nameof(CautionLow), double.NaN);

    public static readonly StyledProperty<double> CautionHighProperty =
        AvaloniaProperty.Register<GaugeBase, double>(nameof(CautionHigh), double.NaN);

    public static readonly StyledProperty<double> WarningLowProperty =
        AvaloniaProperty.Register<GaugeBase, double>(nameof(WarningLow), double.NaN);

    public static readonly StyledProperty<double> WarningHighProperty =
        AvaloniaProperty.Register<GaugeBase, double>(nameof(WarningHigh), double.NaN);

    public static readonly StyledProperty<bool> IsAvailableProperty =
        AvaloniaProperty.Register<GaugeBase, bool>(nameof(IsAvailable), true);

    public static readonly DirectProperty<GaugeBase, double> NormalizedValueProperty =
        AvaloniaProperty.RegisterDirect<GaugeBase, double>(
            nameof(NormalizedValue), control => control.NormalizedValue);

    public static readonly DirectProperty<GaugeBase, double> PercentageProperty =
        AvaloniaProperty.RegisterDirect<GaugeBase, double>(
            nameof(Percentage), control => control.Percentage);

    public static readonly DirectProperty<GaugeBase, string> FormattedValueProperty =
        AvaloniaProperty.RegisterDirect<GaugeBase, string>(
            nameof(FormattedValue), control => control.FormattedValue);

    public static readonly DirectProperty<GaugeBase, GaugeStatus> StatusProperty =
        AvaloniaProperty.RegisterDirect<GaugeBase, GaugeStatus>(
            nameof(Status), control => control.Status);

    public static readonly DirectProperty<GaugeBase, IBrush> StatusBrushProperty =
        AvaloniaProperty.RegisterDirect<GaugeBase, IBrush>(
            nameof(StatusBrush), control => control.StatusBrush);

    private double _normalizedValue;
    private double _percentage;
    private string _formattedValue = "0.0";
    private GaugeStatus _status = GaugeStatus.Normal;
    private IBrush _statusBrush = new SolidColorBrush(Color.Parse("#58D46C"));

    static GaugeBase()
    {
        MinimumProperty.Changed.AddClassHandler<GaugeBase>((control, _) => control.RefreshState());
        MaximumProperty.Changed.AddClassHandler<GaugeBase>((control, _) => control.RefreshState());
        ValueProperty.Changed.AddClassHandler<GaugeBase>((control, _) => control.RefreshState());
        UnitProperty.Changed.AddClassHandler<GaugeBase>((control, _) => control.RefreshState());
        DecimalPlacesProperty.Changed.AddClassHandler<GaugeBase>((control, _) => control.RefreshState());
        CautionLowProperty.Changed.AddClassHandler<GaugeBase>((control, _) => control.RefreshState());
        CautionHighProperty.Changed.AddClassHandler<GaugeBase>((control, _) => control.RefreshState());
        WarningLowProperty.Changed.AddClassHandler<GaugeBase>((control, _) => control.RefreshState());
        WarningHighProperty.Changed.AddClassHandler<GaugeBase>((control, _) => control.RefreshState());
        IsAvailableProperty.Changed.AddClassHandler<GaugeBase>((control, _) => control.RefreshState());
    }

    protected GaugeBase() => RefreshState();

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

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Unit
    {
        get => GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public int DecimalPlaces
    {
        get => GetValue(DecimalPlacesProperty);
        set => SetValue(DecimalPlacesProperty, value);
    }

    public double CautionLow
    {
        get => GetValue(CautionLowProperty);
        set => SetValue(CautionLowProperty, value);
    }

    public double CautionHigh
    {
        get => GetValue(CautionHighProperty);
        set => SetValue(CautionHighProperty, value);
    }

    public double WarningLow
    {
        get => GetValue(WarningLowProperty);
        set => SetValue(WarningLowProperty, value);
    }

    public double WarningHigh
    {
        get => GetValue(WarningHighProperty);
        set => SetValue(WarningHighProperty, value);
    }

    public bool IsAvailable
    {
        get => GetValue(IsAvailableProperty);
        set => SetValue(IsAvailableProperty, value);
    }

    public double NormalizedValue
    {
        get => _normalizedValue;
        private set => SetAndRaise(NormalizedValueProperty, ref _normalizedValue, value);
    }

    public double Percentage
    {
        get => _percentage;
        private set => SetAndRaise(PercentageProperty, ref _percentage, value);
    }

    public string FormattedValue
    {
        get => _formattedValue;
        private set => SetAndRaise(FormattedValueProperty, ref _formattedValue, value);
    }

    public GaugeStatus Status
    {
        get => _status;
        private set => SetAndRaise(StatusProperty, ref _status, value);
    }

    public IBrush StatusBrush
    {
        get => _statusBrush;
        private set => SetAndRaise(StatusBrushProperty, ref _statusBrush, value);
    }

    private void RefreshState()
    {
        var span = Maximum - Minimum;
        NormalizedValue = span > 0
            ? Math.Clamp((Value - Minimum) / span, 0.0, 1.0)
            : 0.0;
        Percentage = NormalizedValue * 100.0;

        var format = "F" + DecimalPlaces.ToString(CultureInfo.InvariantCulture);
        FormattedValue = string.Concat(
            Value.ToString(format, CultureInfo.InvariantCulture),
            string.IsNullOrWhiteSpace(Unit) ? string.Empty : " " + Unit);

        Status = CalculateStatus();
        StatusBrush = new SolidColorBrush(Status switch
        {
            GaugeStatus.Caution => Color.Parse("#F2DD4B"),
            GaugeStatus.Warning => Color.Parse("#F14C4C"),
            GaugeStatus.OutOfRange => Color.Parse("#D04ADF"),
            GaugeStatus.Unavailable => Color.Parse("#7B7F80"),
            _ => Color.Parse("#58D46C")
        });
    }

    private GaugeStatus CalculateStatus()
    {
        if (!IsAvailable)
        {
            return GaugeStatus.Unavailable;
        }

        if (Value < Minimum || Value > Maximum)
        {
            return GaugeStatus.OutOfRange;
        }

        if ((!double.IsNaN(WarningLow) && Value <= WarningLow) ||
            (!double.IsNaN(WarningHigh) && Value >= WarningHigh))
        {
            return GaugeStatus.Warning;
        }

        if ((!double.IsNaN(CautionLow) && Value <= CautionLow) ||
            (!double.IsNaN(CautionHigh) && Value >= CautionHigh))
        {
            return GaugeStatus.Caution;
        }

        return GaugeStatus.Normal;
    }
}
