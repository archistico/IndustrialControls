using Avalonia;
using Avalonia.Controls.Primitives;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Targhetta incisa da pannello industriale.
/// </summary>
public sealed class EngravedLabel : TemplatedControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<EngravedLabel, string>(nameof(Text), string.Empty);

    public static readonly StyledProperty<EngravedLabelVariant> VariantProperty =
        AvaloniaProperty.Register<EngravedLabel, EngravedLabelVariant>(
            nameof(Variant),
            EngravedLabelVariant.Black);

    public static readonly StyledProperty<bool> ShowFastenersProperty =
        AvaloniaProperty.Register<EngravedLabel, bool>(nameof(ShowFasteners), false);

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public EngravedLabelVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public bool ShowFasteners
    {
        get => GetValue(ShowFastenersProperty);
        set => SetValue(ShowFastenersProperty, value);
    }
}
