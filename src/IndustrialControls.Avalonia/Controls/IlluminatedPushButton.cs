using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Pulsante industriale con lampada separata dallo stato meccanico del comando.
/// </summary>
public sealed class IlluminatedPushButton : Button
{
    public static readonly StyledProperty<bool> IsLampOnProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, bool>(nameof(IsLampOn));

    public static readonly StyledProperty<IndustrialLampColor> LampColorProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, IndustrialLampColor>(
            nameof(LampColor),
            IndustrialLampColor.Green);

    public static readonly StyledProperty<IndustrialLampState> LampStateProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, IndustrialLampState>(
            nameof(LampState),
            IndustrialLampState.Normal);

    public static readonly StyledProperty<IlluminatedPushButtonMode> ActionModeProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, IlluminatedPushButtonMode>(
            nameof(ActionMode),
            IlluminatedPushButtonMode.Momentary);

    public static readonly StyledProperty<bool> IsLatchedProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, bool>(nameof(IsLatched));

    public static readonly StyledProperty<string?> SecondaryCaptionProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, string?>(nameof(SecondaryCaption));

    public static readonly StyledProperty<IBrush?> CaptionForegroundProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, IBrush?>(
            nameof(CaptionForeground),
            Brushes.White);

    public bool IsLampOn
    {
        get => GetValue(IsLampOnProperty);
        set => SetValue(IsLampOnProperty, value);
    }

    public IndustrialLampColor LampColor
    {
        get => GetValue(LampColorProperty);
        set => SetValue(LampColorProperty, value);
    }

    public IndustrialLampState LampState
    {
        get => GetValue(LampStateProperty);
        set => SetValue(LampStateProperty, value);
    }

    public IlluminatedPushButtonMode ActionMode
    {
        get => GetValue(ActionModeProperty);
        set => SetValue(ActionModeProperty, value);
    }

    public bool IsLatched
    {
        get => GetValue(IsLatchedProperty);
        set => SetValue(IsLatchedProperty, value);
    }

    public string? SecondaryCaption
    {
        get => GetValue(SecondaryCaptionProperty);
        set => SetValue(SecondaryCaptionProperty, value);
    }

    public IBrush? CaptionForeground
    {
        get => GetValue(CaptionForegroundProperty);
        set => SetValue(CaptionForegroundProperty, value);
    }

    public IlluminatedPushButton()
    {
        Focusable = true;
    }

    protected override void OnClick()
    {
        if (ActionMode == IlluminatedPushButtonMode.Toggle)
        {
            IsLatched = !IsLatched;
        }

        base.OnClick();
    }
}
