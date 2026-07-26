using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Slider industriale per valori di riferimento, limiti e comandi analogici.
/// </summary>
public sealed class IndustrialSlider : Slider
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<IndustrialSlider, string>(nameof(Title), string.Empty);

    public static readonly StyledProperty<string> UnitProperty =
        AvaloniaProperty.Register<IndustrialSlider, string>(nameof(Unit), string.Empty);

    public static readonly StyledProperty<int> DecimalPlacesProperty =
        AvaloniaProperty.Register<IndustrialSlider, int>(
            nameof(DecimalPlaces), 1, validate: value => value is >= 0 and <= 8);

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<IndustrialSlider, bool>(nameof(IsInterlocked));

    public static readonly StyledProperty<string> InterlockReasonProperty =
        AvaloniaProperty.Register<IndustrialSlider, string>(
            nameof(InterlockReason), "COMMAND NOT PERMITTED");

    public static readonly DirectProperty<IndustrialSlider, string> FormattedValueProperty =
        AvaloniaProperty.RegisterDirect<IndustrialSlider, string>(
            nameof(FormattedValue), control => control.FormattedValue);

    public static readonly DirectProperty<IndustrialSlider, string> InterlockTextProperty =
        AvaloniaProperty.RegisterDirect<IndustrialSlider, string>(
            nameof(InterlockText), control => control.InterlockText);

    private string _formattedValue = "0.0";
    private string _interlockText = string.Empty;

    static IndustrialSlider()
    {
        RangeBase.ValueProperty.Changed.AddClassHandler<IndustrialSlider>(
            (control, _) => control.RefreshState());
        TitleProperty.Changed.AddClassHandler<IndustrialSlider>(
            (control, _) => control.RefreshState());
        UnitProperty.Changed.AddClassHandler<IndustrialSlider>(
            (control, _) => control.RefreshState());
        DecimalPlacesProperty.Changed.AddClassHandler<IndustrialSlider>(
            (control, _) => control.RefreshState());
        IsInterlockedProperty.Changed.AddClassHandler<IndustrialSlider>(
            (control, _) => control.RefreshState());
        InterlockReasonProperty.Changed.AddClassHandler<IndustrialSlider>(
            (control, _) => control.RefreshState());
    }

    public IndustrialSlider()
    {
        Focusable = true;
        Minimum = 0;
        Maximum = 100;
        SmallChange = 1;
        LargeChange = 10;
        TickFrequency = 1;
        IsSnapToTickEnabled = true;
        RefreshState();
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

    public string FormattedValue
    {
        get => _formattedValue;
        private set => SetAndRaise(FormattedValueProperty, ref _formattedValue, value);
    }

    public string InterlockText
    {
        get => _interlockText;
        private set => SetAndRaise(InterlockTextProperty, ref _interlockText, value);
    }

    private void RefreshState()
    {
        var format = "F" + DecimalPlaces.ToString(CultureInfo.InvariantCulture);
        FormattedValue = string.Concat(
            Value.ToString(format, CultureInfo.InvariantCulture),
            string.IsNullOrWhiteSpace(Unit) ? string.Empty : " " + Unit);

        InterlockText = IsInterlocked
            ? string.Concat("INTERLOCK — ", InterlockReason)
            : "COMMAND AVAILABLE";

        if (IsInterlocked)
        {
            PseudoClasses.Add(":interlocked");
        }
        else
        {
            PseudoClasses.Remove(":interlocked");
        }

        // Slider input remains physically blocked through the standard
        // disabled state, while :interlocked provides a distinct HMI style.
        IsEnabled = !IsInterlocked;

        IndustrialAutomationMetadata.Apply(
            this,
            Title,
            string.Concat(FormattedValue, "; ", InterlockText),
            "IndustrialSlider");
    }
}
