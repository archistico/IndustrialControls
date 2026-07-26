using Avalonia;
using Avalonia.Controls.Primitives;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Incornicia un contenuto come un modulo montato su un pannello industriale.
/// </summary>
public sealed class IndustrialPanel : HeaderedContentControl
{
    public static readonly StyledProperty<string?> SubtitleProperty =
        AvaloniaProperty.Register<IndustrialPanel, string?>(nameof(Subtitle));

    public static readonly StyledProperty<bool> ShowFastenersProperty =
        AvaloniaProperty.Register<IndustrialPanel, bool>(nameof(ShowFasteners), true);

    public static readonly StyledProperty<IndustrialPanelDepth> DepthProperty =
        AvaloniaProperty.Register<IndustrialPanel, IndustrialPanelDepth>(
            nameof(Depth),
            IndustrialPanelDepth.Raised);

    public string? Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public bool ShowFasteners
    {
        get => GetValue(ShowFastenersProperty);
        set => SetValue(ShowFastenersProperty, value);
    }

    public IndustrialPanelDepth Depth
    {
        get => GetValue(DepthProperty);
        set => SetValue(DepthProperty, value);
    }
}
