using System;
using System.Globalization;
using Avalonia;

namespace IndustrialControls.Avalonia.Controls;

public sealed class SevenSegmentDisplay : LedMatrixDisplay
{
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<SevenSegmentDisplay, double>(nameof(Value));

    public static readonly StyledProperty<int> DigitsProperty =
        AvaloniaProperty.Register<SevenSegmentDisplay, int>(
            nameof(Digits), 6, validate: value => value is >= 1 and <= 16);

    public static readonly StyledProperty<int> DecimalPlacesProperty =
        AvaloniaProperty.Register<SevenSegmentDisplay, int>(
            nameof(DecimalPlaces), 1, validate: value => value is >= 0 and <= 8);

    public static readonly StyledProperty<bool> ShowLeadingZerosProperty =
        AvaloniaProperty.Register<SevenSegmentDisplay, bool>(nameof(ShowLeadingZeros));

    public static readonly StyledProperty<string> UnitProperty =
        AvaloniaProperty.Register<SevenSegmentDisplay, string>(nameof(Unit), string.Empty);

    static SevenSegmentDisplay()
    {
        ValueProperty.Changed.AddClassHandler<SevenSegmentDisplay>((control, _) => control.RefreshText());
        DigitsProperty.Changed.AddClassHandler<SevenSegmentDisplay>((control, _) => control.RefreshText());
        DecimalPlacesProperty.Changed.AddClassHandler<SevenSegmentDisplay>((control, _) => control.RefreshText());
        ShowLeadingZerosProperty.Changed.AddClassHandler<SevenSegmentDisplay>((control, _) => control.RefreshText());
        UnitProperty.Changed.AddClassHandler<SevenSegmentDisplay>((control, _) => control.RefreshText());
    }

    public SevenSegmentDisplay() => RefreshText();

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int Digits
    {
        get => GetValue(DigitsProperty);
        set => SetValue(DigitsProperty, value);
    }

    public int DecimalPlaces
    {
        get => GetValue(DecimalPlacesProperty);
        set => SetValue(DecimalPlacesProperty, value);
    }

    public bool ShowLeadingZeros
    {
        get => GetValue(ShowLeadingZerosProperty);
        set => SetValue(ShowLeadingZerosProperty, value);
    }

    public string Unit
    {
        get => GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    private void RefreshText()
    {
        var format = ShowLeadingZeros
            ? new string('0', Math.Max(1, Digits - (DecimalPlaces > 0 ? DecimalPlaces + 1 : 0)))
              + (DecimalPlaces > 0 ? "." + new string('0', DecimalPlaces) : string.Empty)
            : "F" + DecimalPlaces.ToString(CultureInfo.InvariantCulture);

        Text = string.Concat(Value.ToString(format, CultureInfo.InvariantCulture), Unit.Length > 0 ? " " + Unit : string.Empty);
    }
}
