using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Manopola rotativa industriale con incremento discreto e interlock.
/// </summary>
public sealed class RotaryKnob : TemplatedControl
{
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<RotaryKnob, double>(
            nameof(Minimum),
            0.0);

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<RotaryKnob, double>(
            nameof(Maximum),
            100.0);

    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<RotaryKnob, double>(
            nameof(Value),
            0.0);

    public static readonly StyledProperty<double> SmallChangeProperty =
        AvaloniaProperty.Register<RotaryKnob, double>(
            nameof(SmallChange),
            1.0,
            validate: value => value > 0);

    public static readonly StyledProperty<int> TickCountProperty =
        AvaloniaProperty.Register<RotaryKnob, int>(
            nameof(TickCount),
            11,
            validate: value => value is >= 2 and <= 101);

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<RotaryKnob, string>(
            nameof(Title),
            string.Empty);

    public static readonly StyledProperty<string> UnitProperty =
        AvaloniaProperty.Register<RotaryKnob, string>(
            nameof(Unit),
            string.Empty);

    public static readonly StyledProperty<int> DecimalPlacesProperty =
        AvaloniaProperty.Register<RotaryKnob, int>(
            nameof(DecimalPlaces),
            1,
            validate: value => value is >= 0 and <= 8);

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<RotaryKnob, bool>(
            nameof(IsInterlocked));

    public static readonly StyledProperty<string> InterlockReasonProperty =
        AvaloniaProperty.Register<RotaryKnob, string>(
            nameof(InterlockReason),
            "COMMAND NOT PERMITTED");

    public static readonly DirectProperty<RotaryKnob, double> IndicatorAngleProperty =
        AvaloniaProperty.RegisterDirect<RotaryKnob, double>(
            nameof(IndicatorAngle),
            control => control.IndicatorAngle);

    public static readonly DirectProperty<RotaryKnob, string> FormattedValueProperty =
        AvaloniaProperty.RegisterDirect<RotaryKnob, string>(
            nameof(FormattedValue),
            control => control.FormattedValue);

    public static readonly DirectProperty<RotaryKnob, string> StatusTextProperty =
        AvaloniaProperty.RegisterDirect<RotaryKnob, string>(
            nameof(StatusText),
            control => control.StatusText);

    private double _indicatorAngle = -135;
    private string _formattedValue = "0.0";
    private string _statusText = "COMMAND AVAILABLE";

    static RotaryKnob()
    {
        MinimumProperty.Changed.AddClassHandler<RotaryKnob>(
            (control, _) => control.OnRangeChanged());

        MaximumProperty.Changed.AddClassHandler<RotaryKnob>(
            (control, _) => control.OnRangeChanged());

        ValueProperty.Changed.AddClassHandler<RotaryKnob>(
            (control, _) => control.OnValueChanged());

        TitleProperty.Changed.AddClassHandler<RotaryKnob>(
            (control, _) => control.RefreshState());

        UnitProperty.Changed.AddClassHandler<RotaryKnob>(
            (control, _) => control.RefreshState());

        DecimalPlacesProperty.Changed.AddClassHandler<RotaryKnob>(
            (control, _) => control.RefreshState());

        IsInterlockedProperty.Changed.AddClassHandler<RotaryKnob>(
            (control, _) => control.RefreshState());

        InterlockReasonProperty.Changed.AddClassHandler<RotaryKnob>(
            (control, _) => control.RefreshState());
    }

    public RotaryKnob()
    {
        Focusable = true;
        RefreshState();
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

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetCurrentValue(
            ValueProperty,
            ClampValue(value));
    }

    public double SmallChange
    {
        get => GetValue(SmallChangeProperty);
        set => SetValue(SmallChangeProperty, value);
    }

    public int TickCount
    {
        get => GetValue(TickCountProperty);
        set => SetValue(TickCountProperty, value);
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

    public bool IsInterlocked
    {
        get => GetValue(IsInterlockedProperty);
        set => SetValue(IsInterlockedProperty, value);
    }

    public string InterlockReason
    {
        get => GetValue(InterlockReasonProperty);
        set => SetValue(InterlockReasonProperty, value);
    }

    public double IndicatorAngle
    {
        get => _indicatorAngle;
        private set => SetAndRaise(
            IndicatorAngleProperty,
            ref _indicatorAngle,
            value);
    }

    public string FormattedValue
    {
        get => _formattedValue;
        private set => SetAndRaise(
            FormattedValueProperty,
            ref _formattedValue,
            value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetAndRaise(
            StatusTextProperty,
            ref _statusText,
            value);
    }

    public bool TrySetValue(double value)
    {
        if (IsInterlocked)
        {
            return false;
        }

        Value = value;
        return true;
    }

    public bool Increase() =>
        TrySetValue(
            Value + SmallChange);

    public bool Decrease() =>
        TrySetValue(
            Value - SmallChange);

    protected override void OnPointerPressed(
        PointerPressedEventArgs e)
    {
        if (IsInterlocked)
        {
            e.Handled = true;
            return;
        }

        var point = e.GetPosition(this);

        if (point.X < Bounds.Width / 2.0)
        {
            Decrease();
        }
        else
        {
            Increase();
        }

        Focus();
        e.Handled = true;
    }

    protected override void OnPointerWheelChanged(
        PointerWheelEventArgs e)
    {
        if (IsInterlocked)
        {
            e.Handled = true;
            return;
        }

        if (e.Delta.Y >= 0)
        {
            Increase();
        }
        else
        {
            Decrease();
        }

        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (IsInterlocked)
        {
            e.Handled = true;
            return;
        }

        switch (e.Key)
        {
            case Key.Left:
            case Key.Down:
                Decrease();
                e.Handled = true;
                break;

            case Key.Right:
            case Key.Up:
                Increase();
                e.Handled = true;
                break;

            case Key.Home:
                TrySetValue(Minimum);
                e.Handled = true;
                break;

            case Key.End:
                TrySetValue(Maximum);
                e.Handled = true;
                break;

            default:
                base.OnKeyDown(e);
                break;
        }
    }

    private void OnRangeChanged()
    {
        if (NormalizeValue())
        {
            return;
        }

        RefreshState();
    }

    private void OnValueChanged()
    {
        if (NormalizeValue())
        {
            return;
        }

        RefreshState();
    }

    private bool NormalizeValue()
    {
        var normalized = ClampValue(Value);

        if (normalized.Equals(Value))
        {
            return false;
        }

        SetCurrentValue(
            ValueProperty,
            normalized);

        return true;
    }

    private double ClampValue(double value)
    {
        if (Maximum <= Minimum)
        {
            return Minimum;
        }

        return Math.Clamp(
            value,
            Minimum,
            Maximum);
    }

    private void RefreshState()
    {
        var span = Maximum - Minimum;

        var normalized = span > 0
            ? Math.Clamp(
                (Value - Minimum) / span,
                0.0,
                1.0)
            : 0.0;

        IndicatorAngle =
            -135.0 +
            (normalized * 270.0);

        var format = string.Concat(
            "F",
            DecimalPlaces.ToString(
                CultureInfo.InvariantCulture));

        FormattedValue = string.Concat(
            Value.ToString(
                format,
                CultureInfo.InvariantCulture),
            string.IsNullOrWhiteSpace(Unit)
                ? string.Empty
                : string.Concat(
                    " ",
                    Unit));

        StatusText = IsInterlocked
            ? string.Concat(
                "INTERLOCK — ",
                InterlockReason)
            : "COMMAND AVAILABLE";

        IndustrialAutomationMetadata.Apply(
            this,
            Title,
            string.Concat(
                FormattedValue,
                "; ",
                StatusText),
            "RotaryKnob");
    }
}
