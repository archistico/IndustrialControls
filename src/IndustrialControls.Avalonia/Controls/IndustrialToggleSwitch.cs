using Avalonia;
using Avalonia.Controls.Primitives;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Interruttore industriale bistabile con stato ON/OFF e interlock.
/// </summary>
public sealed class IndustrialToggleSwitch : ToggleButton
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<IndustrialToggleSwitch, string>(nameof(Title), string.Empty);

    public static readonly StyledProperty<string> OnCaptionProperty =
        AvaloniaProperty.Register<IndustrialToggleSwitch, string>(nameof(OnCaption), "ON");

    public static readonly StyledProperty<string> OffCaptionProperty =
        AvaloniaProperty.Register<IndustrialToggleSwitch, string>(nameof(OffCaption), "OFF");

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<IndustrialToggleSwitch, bool>(nameof(IsInterlocked));

    public static readonly StyledProperty<string> InterlockReasonProperty =
        AvaloniaProperty.Register<IndustrialToggleSwitch, string>(
            nameof(InterlockReason), "SWITCHING NOT PERMITTED");

    public static readonly DirectProperty<IndustrialToggleSwitch, bool> IsOnProperty =
        AvaloniaProperty.RegisterDirect<IndustrialToggleSwitch, bool>(
            nameof(IsOn), control => control.IsOn);

    public static readonly DirectProperty<IndustrialToggleSwitch, string> StateTextProperty =
        AvaloniaProperty.RegisterDirect<IndustrialToggleSwitch, string>(
            nameof(StateText), control => control.StateText);

    public static readonly DirectProperty<IndustrialToggleSwitch, string> StatusTextProperty =
        AvaloniaProperty.RegisterDirect<IndustrialToggleSwitch, string>(
            nameof(StatusText), control => control.StatusText);

    private bool _isOn;
    private string _stateText = "OFF";
    private string _statusText = "SWITCHING AVAILABLE";

    static IndustrialToggleSwitch()
    {
        TitleProperty.Changed.AddClassHandler<IndustrialToggleSwitch>(
            (control, _) => control.RefreshState());
        IsCheckedProperty.Changed.AddClassHandler<IndustrialToggleSwitch>(
            (control, _) => control.RefreshState());
        OnCaptionProperty.Changed.AddClassHandler<IndustrialToggleSwitch>(
            (control, _) => control.RefreshState());
        OffCaptionProperty.Changed.AddClassHandler<IndustrialToggleSwitch>(
            (control, _) => control.RefreshState());
        IsInterlockedProperty.Changed.AddClassHandler<IndustrialToggleSwitch>(
            (control, _) => control.RefreshState());
        InterlockReasonProperty.Changed.AddClassHandler<IndustrialToggleSwitch>(
            (control, _) => control.RefreshState());
    }

    public IndustrialToggleSwitch()
    {
        Focusable = true;
        RefreshState();
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string OnCaption
    {
        get => GetValue(OnCaptionProperty);
        set => SetValue(OnCaptionProperty, value);
    }

    public string OffCaption
    {
        get => GetValue(OffCaptionProperty);
        set => SetValue(OffCaptionProperty, value);
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

    public bool IsOn
    {
        get => _isOn;
        private set => SetAndRaise(IsOnProperty, ref _isOn, value);
    }

    public string StateText
    {
        get => _stateText;
        private set => SetAndRaise(StateTextProperty, ref _stateText, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetAndRaise(StatusTextProperty, ref _statusText, value);
    }

    public bool TryToggle()
    {
        if (IsInterlocked)
        {
            return false;
        }

        IsChecked = IsChecked != true;
        return true;
    }

    protected override void OnClick()
    {
        if (IsInterlocked)
        {
            return;
        }

        base.OnClick();
    }

    private void RefreshState()
    {
        IsOn = IsChecked == true;
        StateText = IsOn ? OnCaption : OffCaption;
        StatusText = IsInterlocked
            ? string.Concat("INTERLOCK — ", InterlockReason)
            : "SWITCHING AVAILABLE";

        if (IsInterlocked)
        {
            PseudoClasses.Add(":interlocked");
        }
        else
        {
            PseudoClasses.Remove(":interlocked");
        }

        IndustrialAutomationMetadata.Apply(
            this,
            Title,
            string.Concat("State ", StateText, "; ", StatusText),
            "IndustrialToggleSwitch");
    }
}
