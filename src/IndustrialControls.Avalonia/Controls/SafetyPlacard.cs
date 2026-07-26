using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Pannello statico di sicurezza con icona, testo e viti agli angoli.
/// </summary>
public sealed class SafetyPlacard : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SafetyPlacard, string>(
            nameof(Title), "WARNING");

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<SafetyPlacard, string>(
            nameof(Text), string.Empty);

    public static readonly StyledProperty<SafetyPlacardLevel> LevelProperty =
        AvaloniaProperty.Register<SafetyPlacard, SafetyPlacardLevel>(
            nameof(Level), SafetyPlacardLevel.Warning);

    public static readonly StyledProperty<SafetyPlacardIcon> IconProperty =
        AvaloniaProperty.Register<SafetyPlacard, SafetyPlacardIcon>(
            nameof(Icon), SafetyPlacardIcon.Warning);

    public static readonly StyledProperty<bool> ShowFastenersProperty =
        AvaloniaProperty.Register<SafetyPlacard, bool>(
            nameof(ShowFasteners), true);

    public static readonly DirectProperty<SafetyPlacard, string> IconGlyphProperty =
        AvaloniaProperty.RegisterDirect<SafetyPlacard, string>(
            nameof(IconGlyph), control => control.IconGlyph);

    public static readonly DirectProperty<SafetyPlacard, IBrush> HeaderBrushProperty =
        AvaloniaProperty.RegisterDirect<SafetyPlacard, IBrush>(
            nameof(HeaderBrush), control => control.HeaderBrush);

    public static readonly DirectProperty<SafetyPlacard, IBrush> HeaderForegroundBrushProperty =
        AvaloniaProperty.RegisterDirect<SafetyPlacard, IBrush>(
            nameof(HeaderForegroundBrush), control => control.HeaderForegroundBrush);

    public static readonly DirectProperty<SafetyPlacard, IBrush> BodyBrushProperty =
        AvaloniaProperty.RegisterDirect<SafetyPlacard, IBrush>(
            nameof(BodyBrush), control => control.BodyBrush);

    private string _iconGlyph = "!";
    private IBrush _headerBrush =
        new SolidColorBrush(Color.Parse("#E67E22"));
    private IBrush _headerForegroundBrush =
        new SolidColorBrush(Color.Parse("#111315"));
    private IBrush _bodyBrush =
        new SolidColorBrush(Color.Parse("#F1EEE1"));

    static SafetyPlacard()
    {
        LevelProperty.Changed.AddClassHandler<SafetyPlacard>(
            (control, _) => control.RefreshAppearance());
        IconProperty.Changed.AddClassHandler<SafetyPlacard>(
            (control, _) => control.RefreshAppearance());
    }

    public SafetyPlacard() => RefreshAppearance();

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public SafetyPlacardLevel Level
    {
        get => GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

    public SafetyPlacardIcon Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public bool ShowFasteners
    {
        get => GetValue(ShowFastenersProperty);
        set => SetValue(ShowFastenersProperty, value);
    }

    public string IconGlyph
    {
        get => _iconGlyph;
        private set => SetAndRaise(
            IconGlyphProperty,
            ref _iconGlyph,
            value);
    }

    public IBrush HeaderBrush
    {
        get => _headerBrush;
        private set => SetAndRaise(
            HeaderBrushProperty,
            ref _headerBrush,
            value);
    }

    public IBrush HeaderForegroundBrush
    {
        get => _headerForegroundBrush;
        private set => SetAndRaise(
            HeaderForegroundBrushProperty,
            ref _headerForegroundBrush,
            value);
    }

    public IBrush BodyBrush
    {
        get => _bodyBrush;
        private set => SetAndRaise(
            BodyBrushProperty,
            ref _bodyBrush,
            value);
    }

    private void RefreshAppearance()
    {
        IconGlyph = Icon switch
        {
            SafetyPlacardIcon.ElectricalHazard => "⚡",
            SafetyPlacardIcon.Radiation => "☢",
            SafetyPlacardIcon.HotSurface => "♨",
            SafetyPlacardIcon.Mandatory => "●",
            SafetyPlacardIcon.Information => "i",
            _ => "!"
        };

        var headerColor = Level switch
        {
            SafetyPlacardLevel.Information => Color.Parse("#2878A8"),
            SafetyPlacardLevel.Notice => Color.Parse("#3A8A4E"),
            SafetyPlacardLevel.Caution => Color.Parse("#F2DD4B"),
            SafetyPlacardLevel.Danger => Color.Parse("#B91F28"),
            _ => Color.Parse("#E67E22")
        };

        HeaderBrush = new SolidColorBrush(headerColor);
        HeaderForegroundBrush = new SolidColorBrush(
            Level is SafetyPlacardLevel.Information or
                SafetyPlacardLevel.Notice or
                SafetyPlacardLevel.Danger
                ? Color.Parse("#F8F8F0")
                : Color.Parse("#111315"));

        BodyBrush = new SolidColorBrush(
            Level == SafetyPlacardLevel.Danger
                ? Color.Parse("#F4E6E6")
                : Color.Parse("#F1EEE1"));
    }
}
