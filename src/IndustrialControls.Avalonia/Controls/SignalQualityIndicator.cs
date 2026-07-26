using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Indicatore compatto della qualità di un segnale.
/// </summary>
public sealed class SignalQualityIndicator : TemplatedControl
{
    public static readonly StyledProperty<string> SignalNameProperty =
        AvaloniaProperty.Register<SignalQualityIndicator, string>(
            nameof(SignalName), "SIGNAL");

    public static readonly StyledProperty<string> SourceProperty =
        AvaloniaProperty.Register<SignalQualityIndicator, string>(
            nameof(Source), string.Empty);

    public static readonly StyledProperty<SignalQuality> QualityProperty =
        AvaloniaProperty.Register<SignalQualityIndicator, SignalQuality>(
            nameof(Quality), SignalQuality.Good);

    public static readonly DirectProperty<SignalQualityIndicator, string> QualityTextProperty =
        AvaloniaProperty.RegisterDirect<SignalQualityIndicator, string>(
            nameof(QualityText), control => control.QualityText);

    public static readonly DirectProperty<SignalQualityIndicator, IBrush> QualityBrushProperty =
        AvaloniaProperty.RegisterDirect<SignalQualityIndicator, IBrush>(
            nameof(QualityBrush), control => control.QualityBrush);

    public static readonly DirectProperty<SignalQualityIndicator, IndustrialLampColor> LampColorProperty =
        AvaloniaProperty.RegisterDirect<SignalQualityIndicator, IndustrialLampColor>(
            nameof(LampColor), control => control.LampColor);

    private string _qualityText = "GOOD";
    private IBrush _qualityBrush =
        new SolidColorBrush(Color.Parse("#58D46C"));
    private IndustrialLampColor _lampColor =
        IndustrialLampColor.Green;

    static SignalQualityIndicator()
    {
        QualityProperty.Changed.AddClassHandler<SignalQualityIndicator>(
            (control, _) => control.RefreshState());
    }

    public SignalQualityIndicator() => RefreshState();

    public string SignalName
    {
        get => GetValue(SignalNameProperty);
        set => SetValue(SignalNameProperty, value);
    }

    public string Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public SignalQuality Quality
    {
        get => GetValue(QualityProperty);
        set => SetValue(QualityProperty, value);
    }

    public string QualityText
    {
        get => _qualityText;
        private set => SetAndRaise(
            QualityTextProperty,
            ref _qualityText,
            value);
    }

    public IBrush QualityBrush
    {
        get => _qualityBrush;
        private set => SetAndRaise(
            QualityBrushProperty,
            ref _qualityBrush,
            value);
    }

    public IndustrialLampColor LampColor
    {
        get => _lampColor;
        private set => SetAndRaise(
            LampColorProperty,
            ref _lampColor,
            value);
    }

    private void RefreshState()
    {
        (QualityText, QualityBrush, LampColor) = Quality switch
        {
            SignalQuality.Uncertain => (
                "UNCERTAIN",
                new SolidColorBrush(Color.Parse("#E3C83B")),
                IndustrialLampColor.Amber),
            SignalQuality.Bad => (
                "BAD",
                new SolidColorBrush(Color.Parse("#F14C4C")),
                IndustrialLampColor.Red),
            SignalQuality.Unavailable => (
                "UNAVAILABLE",
                new SolidColorBrush(Color.Parse("#7B7F80")),
                IndustrialLampColor.Blue),
            _ => (
                "GOOD",
                new SolidColorBrush(Color.Parse("#58D46C")),
                IndustrialLampColor.Green)
        };
    }
}
