using Avalonia;
using Avalonia.Controls;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Cornice da incasso per strumenti, display e controlli.
/// </summary>
public sealed class InstrumentBezel : ContentControl
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<InstrumentBezel, string?>(nameof(Title));

    public static readonly StyledProperty<string?> UnitProperty =
        AvaloniaProperty.Register<InstrumentBezel, string?>(nameof(Unit));

    public static readonly StyledProperty<InstrumentBezelShape> ShapeProperty =
        AvaloniaProperty.Register<InstrumentBezel, InstrumentBezelShape>(
            nameof(Shape),
            InstrumentBezelShape.Rectangular);

    public static readonly StyledProperty<bool> ShowGlassProperty =
        AvaloniaProperty.Register<InstrumentBezel, bool>(nameof(ShowGlass), true);

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Unit
    {
        get => GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public InstrumentBezelShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public bool ShowGlass
    {
        get => GetValue(ShowGlassProperty);
        set => SetValue(ShowGlassProperty, value);
    }
}
