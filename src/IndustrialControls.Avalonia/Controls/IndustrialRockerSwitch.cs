using Avalonia;
using Avalonia.Controls.Primitives;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Interruttore ON/OFF a bilanciere con simbologia I/O e interlock.
/// </summary>
public sealed class IndustrialRockerSwitch : ToggleButton
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<IndustrialRockerSwitch, string>(nameof(Title), string.Empty);

    public static readonly StyledProperty<string> OnCaptionProperty =
        AvaloniaProperty.Register<IndustrialRockerSwitch, string>(nameof(OnCaption), "ON");

    public static readonly StyledProperty<string> OffCaptionProperty =
        AvaloniaProperty.Register<IndustrialRockerSwitch, string>(nameof(OffCaption), "OFF");

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<IndustrialRockerSwitch, bool>(nameof(IsInterlocked));

    public static readonly StyledProperty<string> InterlockReasonProperty =
        AvaloniaProperty.Register<IndustrialRockerSwitch, string>(
            nameof(InterlockReason), "SWITCHING NOT PERMITTED");

    public static readonly DirectProperty<IndustrialRockerSwitch, bool> IsOnProperty =
        AvaloniaProperty.RegisterDirect<IndustrialRockerSwitch, bool>(
            nameof(IsOn), control => control.IsOn);

    public static readonly DirectProperty<IndustrialRockerSwitch, string> StateTextProperty =
        AvaloniaProperty.RegisterDirect<IndustrialRockerSwitch, string>(
            nameof(StateText), control => control.StateText);

    public static readonly DirectProperty<IndustrialRockerSwitch, string> StatusTextProperty =
        AvaloniaProperty.RegisterDirect<IndustrialRockerSwitch, string>(
            nameof(StatusText), control => control.StatusText);

    private bool _isOn;
    private string _stateText = "OFF";
    private string _statusText = "SWITCHING AVAILABLE";

    static IndustrialRockerSwitch()
    {
        TitleProperty.Changed.AddClassHandler<IndustrialRockerSwitch>(
            (control, _) => control.RefreshState());
        IsCheckedProperty.Changed.AddClassHandler<IndustrialRockerSwitch>(
            (control, _) => control.RefreshState());
        OnCaptionProperty.Changed.AddClassHandler<IndustrialRockerSwitch>(
            (control, _) => control.RefreshState());
        OffCaptionProperty.Changed.AddClassHandler<IndustrialRockerSwitch>(
            (control, _) => control.RefreshState());
        IsInterlockedProperty.Changed.AddClassHandler<IndustrialRockerSwitch>(
            (control, _) => control.RefreshState());
        InterlockReasonProperty.Changed.AddClassHandler<IndustrialRockerSwitch>(
            (control, _) => control.RefreshState());
    }

    public IndustrialRockerSwitch()
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
            "IndustrialRockerSwitch");
    }
}
