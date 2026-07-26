using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Comando momentaneo sinistra-centro-destra con ritorno automatico al centro.
/// </summary>
public sealed class SpringReturnSwitch : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SpringReturnSwitch, string>(
            nameof(Title),
            string.Empty);

    public static readonly StyledProperty<string> LeftCaptionProperty =
        AvaloniaProperty.Register<SpringReturnSwitch, string>(
            nameof(LeftCaption),
            "LOWER");

    public static readonly StyledProperty<string> CenterCaptionProperty =
        AvaloniaProperty.Register<SpringReturnSwitch, string>(
            nameof(CenterCaption),
            "HOLD");

    public static readonly StyledProperty<string> RightCaptionProperty =
        AvaloniaProperty.Register<SpringReturnSwitch, string>(
            nameof(RightCaption),
            "RAISE");

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<SpringReturnSwitch, bool>(
            nameof(IsInterlocked));

    public static readonly StyledProperty<string> InterlockReasonProperty =
        AvaloniaProperty.Register<SpringReturnSwitch, string>(
            nameof(InterlockReason),
            "MOMENTARY COMMAND NOT PERMITTED");

    public static readonly DirectProperty<SpringReturnSwitch, SpringReturnPosition> PositionProperty =
        AvaloniaProperty.RegisterDirect<SpringReturnSwitch, SpringReturnPosition>(
            nameof(Position),
            control => control.Position);

    public static readonly DirectProperty<SpringReturnSwitch, int> PositionIndexProperty =
        AvaloniaProperty.RegisterDirect<SpringReturnSwitch, int>(
            nameof(PositionIndex),
            control => control.PositionIndex);

    public static readonly DirectProperty<SpringReturnSwitch, string> PositionLabelsProperty =
        AvaloniaProperty.RegisterDirect<SpringReturnSwitch, string>(
            nameof(PositionLabels),
            control => control.PositionLabels);

    public static readonly DirectProperty<SpringReturnSwitch, string> StateTextProperty =
        AvaloniaProperty.RegisterDirect<SpringReturnSwitch, string>(
            nameof(StateText),
            control => control.StateText);

    public static readonly DirectProperty<SpringReturnSwitch, string> StatusTextProperty =
        AvaloniaProperty.RegisterDirect<SpringReturnSwitch, string>(
            nameof(StatusText),
            control => control.StatusText);

    private IPointer? _capturedPointer;
    private SpringReturnPosition _position =
        SpringReturnPosition.Center;
    private int _positionIndex = 1;
    private string _positionLabels =
        "LOWER|HOLD|RAISE";
    private string _stateText = "HOLD";
    private string _statusText =
        "COMMAND AVAILABLE";

    static SpringReturnSwitch()
    {
        TitleProperty.Changed.AddClassHandler<SpringReturnSwitch>(
            (control, _) => control.RefreshState());
        LeftCaptionProperty.Changed.AddClassHandler<SpringReturnSwitch>(
            (control, _) => control.RefreshState());
        CenterCaptionProperty.Changed.AddClassHandler<SpringReturnSwitch>(
            (control, _) => control.RefreshState());
        RightCaptionProperty.Changed.AddClassHandler<SpringReturnSwitch>(
            (control, _) => control.RefreshState());
        IsInterlockedProperty.Changed.AddClassHandler<SpringReturnSwitch>(
            (control, _) => control.OnInterlockChanged());
        InterlockReasonProperty.Changed.AddClassHandler<SpringReturnSwitch>(
            (control, _) => control.RefreshState());
    }

    public SpringReturnSwitch()
    {
        Focusable = true;
        RefreshState();
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string LeftCaption
    {
        get => GetValue(LeftCaptionProperty);
        set => SetValue(LeftCaptionProperty, value);
    }

    public string CenterCaption
    {
        get => GetValue(CenterCaptionProperty);
        set => SetValue(CenterCaptionProperty, value);
    }

    public string RightCaption
    {
        get => GetValue(RightCaptionProperty);
        set => SetValue(RightCaptionProperty, value);
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

    public SpringReturnPosition Position
    {
        get => _position;
        private set => SetAndRaise(
            PositionProperty,
            ref _position,
            value);
    }

    public int PositionIndex
    {
        get => _positionIndex;
        private set => SetAndRaise(
            PositionIndexProperty,
            ref _positionIndex,
            value);
    }

    public string PositionLabels
    {
        get => _positionLabels;
        private set => SetAndRaise(
            PositionLabelsProperty,
            ref _positionLabels,
            value);
    }

    public string StateText
    {
        get => _stateText;
        private set => SetAndRaise(
            StateTextProperty,
            ref _stateText,
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

    public bool PressLeft()
    {
        if (IsInterlocked)
        {
            return false;
        }

        SetPosition(SpringReturnPosition.Left);
        return true;
    }

    public bool PressRight()
    {
        if (IsInterlocked)
        {
            return false;
        }

        SetPosition(SpringReturnPosition.Right);
        return true;
    }

    public void Release() =>
        SetPosition(SpringReturnPosition.Center);

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (IsInterlocked)
        {
            base.OnKeyDown(e);
            return;
        }

        switch (e.Key)
        {
            case Key.Left:
            case Key.Down:
                PressLeft();
                e.Handled = true;
                break;

            case Key.Right:
            case Key.Up:
                PressRight();
                e.Handled = true;
                break;

            default:
                base.OnKeyDown(e);
                break;
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (e.Key is
            Key.Left or
            Key.Down or
            Key.Right or
            Key.Up)
        {
            Release();
            e.Handled = true;
            return;
        }

        base.OnKeyUp(e);
    }

    protected override void OnPointerPressed(
        PointerPressedEventArgs e)
    {
        if (IsInterlocked)
        {
            e.Handled = true;
            return;
        }

        var point = e.GetPosition(this);
        var accepted = point.X < Bounds.Width / 2.0
            ? PressLeft()
            : PressRight();

        if (accepted)
        {
            _capturedPointer = e.Pointer;
            e.Pointer.Capture(this);
            Focus();
        }

        e.Handled = true;
    }

    protected override void OnPointerReleased(
        PointerReleasedEventArgs e)
    {
        ReleasePointer(e.Pointer);
        e.Handled = true;
    }

    protected override void OnPointerCaptureLost(
        PointerCaptureLostEventArgs e)
    {
        _capturedPointer = null;
        Release();
        base.OnPointerCaptureLost(e);
    }

    private void OnInterlockChanged()
    {
        if (IsInterlocked)
        {
            var pointer = _capturedPointer;
            _capturedPointer = null;
            pointer?.Capture(null);
            Release();
        }
        else
        {
            RefreshState();
        }
    }

    private void ReleasePointer(IPointer pointer)
    {
        Release();

        if (ReferenceEquals(
                _capturedPointer,
                pointer))
        {
            _capturedPointer = null;
            pointer.Capture(null);
        }
    }

    private void SetPosition(
        SpringReturnPosition position)
    {
        Position = position;
        RefreshState();
    }

    private void RefreshState()
    {
        PositionLabels = string.Concat(
            LeftCaption,
            "|",
            CenterCaption,
            "|",
            RightCaption);

        PositionIndex = Position switch
        {
            SpringReturnPosition.Left => 0,
            SpringReturnPosition.Right => 2,
            _ => 1
        };

        StateText = Position switch
        {
            SpringReturnPosition.Left =>
                LeftCaption,
            SpringReturnPosition.Right =>
                RightCaption,
            _ =>
                CenterCaption
        };

        StatusText = IsInterlocked
            ? string.Concat(
                "INTERLOCK — ",
                InterlockReason)
            : Position == SpringReturnPosition.Center
                ? "SPRING RETURN READY"
                : "COMMAND HELD";

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
            string.Concat(
                "Position ",
                StateText,
                "; ",
                StatusText),
            "SpringReturnSwitch");
    }
}
