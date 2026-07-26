using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Targhetta dati statica con quattro fissaggi agli angoli.
/// </summary>
public sealed class BoltedDataPlate : ContentControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<BoltedDataPlate, string>(
            nameof(Title), string.Empty);

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<BoltedDataPlate, string>(
            nameof(Subtitle), string.Empty);

    public static readonly StyledProperty<string> IdentifierProperty =
        AvaloniaProperty.Register<BoltedDataPlate, string>(
            nameof(Identifier), string.Empty);

    public static readonly StyledProperty<DataPlateMaterial> MaterialProperty =
        AvaloniaProperty.Register<BoltedDataPlate, DataPlateMaterial>(
            nameof(Material), DataPlateMaterial.Aluminum);

    public static readonly StyledProperty<bool> ShowFastenersProperty =
        AvaloniaProperty.Register<BoltedDataPlate, bool>(
            nameof(ShowFasteners), true);

    public static readonly DirectProperty<BoltedDataPlate, IBrush> PlateBrushProperty =
        AvaloniaProperty.RegisterDirect<BoltedDataPlate, IBrush>(
            nameof(PlateBrush), control => control.PlateBrush);

    public static readonly DirectProperty<BoltedDataPlate, IBrush> PlateForegroundBrushProperty =
        AvaloniaProperty.RegisterDirect<BoltedDataPlate, IBrush>(
            nameof(PlateForegroundBrush), control => control.PlateForegroundBrush);

    public static readonly DirectProperty<BoltedDataPlate, IBrush> PlateEdgeBrushProperty =
        AvaloniaProperty.RegisterDirect<BoltedDataPlate, IBrush>(
            nameof(PlateEdgeBrush), control => control.PlateEdgeBrush);

    private IBrush _plateBrush =
        new SolidColorBrush(Color.Parse("#A8ADB0"));
    private IBrush _plateForegroundBrush =
        new SolidColorBrush(Color.Parse("#17191B"));
    private IBrush _plateEdgeBrush =
        new SolidColorBrush(Color.Parse("#686E72"));

    static BoltedDataPlate()
    {
        MaterialProperty.Changed.AddClassHandler<BoltedDataPlate>(
            (control, _) => control.RefreshAppearance());
    }

    public BoltedDataPlate() => RefreshAppearance();

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string Identifier
    {
        get => GetValue(IdentifierProperty);
        set => SetValue(IdentifierProperty, value);
    }

    public DataPlateMaterial Material
    {
        get => GetValue(MaterialProperty);
        set => SetValue(MaterialProperty, value);
    }

    public bool ShowFasteners
    {
        get => GetValue(ShowFastenersProperty);
        set => SetValue(ShowFastenersProperty, value);
    }

    public IBrush PlateBrush
    {
        get => _plateBrush;
        private set => SetAndRaise(
            PlateBrushProperty,
            ref _plateBrush,
            value);
    }

    public IBrush PlateForegroundBrush
    {
        get => _plateForegroundBrush;
        private set => SetAndRaise(
            PlateForegroundBrushProperty,
            ref _plateForegroundBrush,
            value);
    }

    public IBrush PlateEdgeBrush
    {
        get => _plateEdgeBrush;
        private set => SetAndRaise(
            PlateEdgeBrushProperty,
            ref _plateEdgeBrush,
            value);
    }

    private void RefreshAppearance()
    {
        var colors = Material switch
        {
            DataPlateMaterial.Brass => (
                Plate: Color.Parse("#B9A45B"),
                Foreground: Color.Parse("#201D12"),
                Edge: Color.Parse("#71612F")),
            DataPlateMaterial.Black => (
                Plate: Color.Parse("#181A1B"),
                Foreground: Color.Parse("#F1F1DF"),
                Edge: Color.Parse("#666D70")),
            DataPlateMaterial.Red => (
                Plate: Color.Parse("#741D20"),
                Foreground: Color.Parse("#F8F2E8"),
                Edge: Color.Parse("#361012")),
            _ => (
                Plate: Color.Parse("#A8ADB0"),
                Foreground: Color.Parse("#17191B"),
                Edge: Color.Parse("#686E72"))
        };

        PlateBrush = new SolidColorBrush(colors.Plate);
        PlateForegroundBrush = new SolidColorBrush(colors.Foreground);
        PlateEdgeBrush = new SolidColorBrush(colors.Edge);
    }
}
