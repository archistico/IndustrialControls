using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

public class LedMatrixDisplay : TemplatedControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<LedMatrixDisplay, string>(nameof(Text), string.Empty);

    public static readonly StyledProperty<LedDisplayColor> LedColorProperty =
        AvaloniaProperty.Register<LedMatrixDisplay, LedDisplayColor>(nameof(LedColor), LedDisplayColor.Red);

    public static readonly StyledProperty<LedMatrixSize> MatrixSizeProperty =
        AvaloniaProperty.Register<LedMatrixDisplay, LedMatrixSize>(nameof(MatrixSize), LedMatrixSize.Font5x7);

    public static readonly StyledProperty<double> BrightnessProperty =
        AvaloniaProperty.Register<LedMatrixDisplay, double>(
            nameof(Brightness), 0.9, validate: value => value is >= 0 and <= 1);

    public static readonly StyledProperty<int> CharacterSpacingProperty =
        AvaloniaProperty.Register<LedMatrixDisplay, int>(
            nameof(CharacterSpacing), 1, validate: value => value is >= 0 and <= 8);

    public static readonly DirectProperty<LedMatrixDisplay, IBrush> LedBrushProperty =
        AvaloniaProperty.RegisterDirect<LedMatrixDisplay, IBrush>(
            nameof(LedBrush), control => control.LedBrush);

    public static readonly DirectProperty<LedMatrixDisplay, double> EffectiveOpacityProperty =
        AvaloniaProperty.RegisterDirect<LedMatrixDisplay, double>(
            nameof(EffectiveOpacity), control => control.EffectiveOpacity);

    private IBrush _ledBrush = Brushes.Red;
    private double _effectiveOpacity = 0.9;

    static LedMatrixDisplay()
    {
        LedColorProperty.Changed.AddClassHandler<LedMatrixDisplay>((control, _) => control.RefreshVisualState());
        BrightnessProperty.Changed.AddClassHandler<LedMatrixDisplay>((control, _) => control.RefreshVisualState());
    }

    public LedMatrixDisplay() => RefreshVisualState();

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public LedDisplayColor LedColor
    {
        get => GetValue(LedColorProperty);
        set => SetValue(LedColorProperty, value);
    }

    public LedMatrixSize MatrixSize
    {
        get => GetValue(MatrixSizeProperty);
        set => SetValue(MatrixSizeProperty, value);
    }

    public double Brightness
    {
        get => GetValue(BrightnessProperty);
        set => SetValue(BrightnessProperty, value);
    }

    public int CharacterSpacing
    {
        get => GetValue(CharacterSpacingProperty);
        set => SetValue(CharacterSpacingProperty, value);
    }

    public IBrush LedBrush
    {
        get => _ledBrush;
        private set => SetAndRaise(LedBrushProperty, ref _ledBrush, value);
    }

    public double EffectiveOpacity
    {
        get => _effectiveOpacity;
        private set => SetAndRaise(EffectiveOpacityProperty, ref _effectiveOpacity, value);
    }

    private void RefreshVisualState()
    {
        LedBrush = new SolidColorBrush(LedColor switch
        {
            LedDisplayColor.Amber => Color.Parse("#FFB238"),
            LedDisplayColor.Green => Color.Parse("#58D46C"),
            LedDisplayColor.Blue => Color.Parse("#57A8E8"),
            LedDisplayColor.White => Color.Parse("#F1F1DF"),
            _ => Color.Parse("#F14C4C")
        });
        EffectiveOpacity = Math.Clamp(Brightness, 0, 1);
    }
}
