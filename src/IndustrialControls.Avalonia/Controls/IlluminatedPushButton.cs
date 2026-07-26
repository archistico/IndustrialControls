using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Pulsante industriale illuminato con comando momentaneo o bistabile.
/// </summary>
public sealed class IlluminatedPushButton : Button
{
    private static readonly IBrush AvailableStatusBrush =
        new SolidColorBrush(Color.Parse("#A8ADA8"));

    private static readonly IBrush InterlockedStatusBrush =
        new SolidColorBrush(Color.Parse("#F2DD4B"));

    public static readonly StyledProperty<bool> IsLampOnProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, bool>(
            nameof(IsLampOn));

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
        AvaloniaProperty.Register<IlluminatedPushButton, bool>(
            nameof(IsLatched));

    public static readonly StyledProperty<string?> SecondaryCaptionProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, string?>(
            nameof(SecondaryCaption));

    public static readonly StyledProperty<IBrush?> CaptionForegroundProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, IBrush?>(
            nameof(CaptionForeground),
            Brushes.White);

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, bool>(
            nameof(IsInterlocked));

    public static readonly StyledProperty<string> InterlockReasonProperty =
        AvaloniaProperty.Register<IlluminatedPushButton, string>(
            nameof(InterlockReason),
            "COMMAND NOT PERMITTED");

    public static readonly DirectProperty<IlluminatedPushButton, bool> CanInvokeProperty =
        AvaloniaProperty.RegisterDirect<IlluminatedPushButton, bool>(
            nameof(CanInvoke),
            control => control.CanInvoke);

    public static readonly DirectProperty<IlluminatedPushButton, string> StatusTextProperty =
        AvaloniaProperty.RegisterDirect<IlluminatedPushButton, string>(
            nameof(StatusText),
            control => control.StatusText);

    public static readonly DirectProperty<IlluminatedPushButton, IBrush> StatusBrushProperty =
        AvaloniaProperty.RegisterDirect<IlluminatedPushButton, IBrush>(
            nameof(StatusBrush),
            control => control.StatusBrush);

    private bool _canInvoke = true;
    private string _statusText =
        "COMMAND AVAILABLE";
    private IBrush _statusBrush =
        AvailableStatusBrush;

    static IlluminatedPushButton()
    {
        IsInterlockedProperty.Changed.AddClassHandler<IlluminatedPushButton>(
            (control, _) => control.RefreshInterlockState());

        InterlockReasonProperty.Changed.AddClassHandler<IlluminatedPushButton>(
            (control, _) => control.RefreshInterlockState());

        ContentProperty.Changed.AddClassHandler<IlluminatedPushButton>(
            (control, _) => control.RefreshAutomationMetadata());

        SecondaryCaptionProperty.Changed.AddClassHandler<IlluminatedPushButton>(
            (control, _) => control.RefreshAutomationMetadata());
    }

    public IlluminatedPushButton()
    {
        Focusable = true;
        RefreshInterlockState();
    }

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

    public bool CanInvoke
    {
        get => _canInvoke;
        private set => SetAndRaise(
            CanInvokeProperty,
            ref _canInvoke,
            value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetAndRaise(
            StatusTextProperty,
            ref _statusText,
            value);
    }

    public IBrush StatusBrush
    {
        get => _statusBrush;
        private set => SetAndRaise(
            StatusBrushProperty,
            ref _statusBrush,
            value);
    }

    /// <summary>
    /// Attempts to invoke the button through the same path used by pointer
    /// and keyboard input.
    /// </summary>
    public bool TryInvoke()
    {
        if (IsInterlocked)
        {
            return false;
        }

        InvokeAcceptedCommand();
        return true;
    }

    protected override void OnClick()
    {
        if (IsInterlocked)
        {
            return;
        }

        InvokeAcceptedCommand();
    }

    private void InvokeAcceptedCommand()
    {
        if (ActionMode ==
            IlluminatedPushButtonMode.Toggle)
        {
            IsLatched = !IsLatched;
        }

        base.OnClick();
    }

    private void RefreshInterlockState()
    {
        CanInvoke = !IsInterlocked;

        StatusText = IsInterlocked
            ? string.Concat(
                "INTERLOCK — ",
                InterlockReason)
            : "COMMAND AVAILABLE";

        StatusBrush = IsInterlocked
            ? InterlockedStatusBrush
            : AvailableStatusBrush;

        if (IsInterlocked)
        {
            PseudoClasses.Add(":interlocked");
        }
        else
        {
            PseudoClasses.Remove(":interlocked");
        }

        RefreshAutomationMetadata();
    }

    private void RefreshAutomationMetadata()
    {
        var name = Content?.ToString();

        IndustrialAutomationMetadata.Apply(
            this,
            name,
            string.Concat(
                SecondaryCaption,
                string.IsNullOrWhiteSpace(
                    SecondaryCaption)
                    ? string.Empty
                    : "; ",
                StatusText),
            "IlluminatedPushButton");
    }
}
