using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Selettore rotativo da due a cinque posizioni.
/// </summary>
public sealed class SelectorSwitch : TemplatedControl
{
    public static readonly StyledProperty<int> PositionCountProperty =
        AvaloniaProperty.Register<SelectorSwitch, int>(
            nameof(PositionCount), 3, validate: value => value is >= 2 and <= 5);

    public static readonly StyledProperty<int> PositionProperty =
        AvaloniaProperty.Register<SelectorSwitch, int>(
            nameof(Position), 0, validate: value => value is >= 0 and <= 4);

    public static readonly StyledProperty<string> PositionLabelsProperty =
        AvaloniaProperty.Register<SelectorSwitch, string>(
            nameof(PositionLabels), "OFF|AUTO|MANUAL");

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SelectorSwitch, string>(nameof(Title), string.Empty);

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<SelectorSwitch, bool>(nameof(IsInterlocked));

    public static readonly StyledProperty<string> InterlockReasonProperty =
        AvaloniaProperty.Register<SelectorSwitch, string>(
            nameof(InterlockReason), "SELECTION NOT PERMITTED");

    public static readonly DirectProperty<SelectorSwitch, double> HandleAngleProperty =
        AvaloniaProperty.RegisterDirect<SelectorSwitch, double>(
            nameof(HandleAngle), control => control.HandleAngle);

    public static readonly DirectProperty<SelectorSwitch, string> SelectedLabelProperty =
        AvaloniaProperty.RegisterDirect<SelectorSwitch, string>(
            nameof(SelectedLabel), control => control.SelectedLabel);

    public static readonly DirectProperty<SelectorSwitch, string> StatusTextProperty =
        AvaloniaProperty.RegisterDirect<SelectorSwitch, string>(
            nameof(StatusText), control => control.StatusText);

    private double _handleAngle = -60;
    private string _selectedLabel = "OFF";
    private string _statusText = "SELECTION AVAILABLE";

    static SelectorSwitch()
    {
        PositionCountProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.RefreshState());
        PositionProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.RefreshState());
        PositionLabelsProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.RefreshState());
        IsInterlockedProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.RefreshState());
        InterlockReasonProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.RefreshState());
    }

    public SelectorSwitch()
    {
        Focusable = true;
        RefreshState();
    }

    public int PositionCount
    {
        get => GetValue(PositionCountProperty);
        set
        {
            SetValue(PositionCountProperty, value);
            if (Position >= value)
            {
                Position = value - 1;
            }
        }
    }

    public int Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(
            PositionProperty,
            Math.Clamp(value, 0, Math.Max(0, PositionCount - 1)));
    }

    public string PositionLabels
    {
        get => GetValue(PositionLabelsProperty);
        set => SetValue(PositionLabelsProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
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

    public double HandleAngle
    {
        get => _handleAngle;
        private set => SetAndRaise(HandleAngleProperty, ref _handleAngle, value);
    }

    public string SelectedLabel
    {
        get => _selectedLabel;
        private set => SetAndRaise(SelectedLabelProperty, ref _selectedLabel, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetAndRaise(StatusTextProperty, ref _statusText, value);
    }

    public bool Select(int position)
    {
        if (IsInterlocked)
        {
            return false;
        }

        Position = position;
        return true;
    }

    public bool SelectNext() => Select(Math.Min(Position + 1, PositionCount - 1));

    public bool SelectPrevious() => Select(Math.Max(Position - 1, 0));

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (IsInterlocked)
        {
            e.Handled = true;
            return;
        }

        var point = e.GetPosition(this);
        if (point.X < Bounds.Width / 2.0)
        {
            SelectPrevious();
        }
        else
        {
            SelectNext();
        }

        Focus();
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (IsInterlocked)
        {
            e.Handled = true;
            return;
        }

        switch (e.Key)
        {
            case Key.Left:
            case Key.Down:
                SelectPrevious();
                e.Handled = true;
                break;
            case Key.Right:
            case Key.Up:
                SelectNext();
                e.Handled = true;
                break;
            case Key.Home:
                Select(0);
                e.Handled = true;
                break;
            case Key.End:
                Select(PositionCount - 1);
                e.Handled = true;
                break;
            default:
                base.OnKeyDown(e);
                break;
        }
    }

    private void RefreshState()
    {
        var count = Math.Max(2, PositionCount);
        var safePosition = Math.Clamp(Position, 0, count - 1);
        HandleAngle = count == 1
            ? 0
            : -60.0 + (safePosition * (120.0 / (count - 1)));

        var labels = PositionLabels.Split(
            '|',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        SelectedLabel = safePosition < labels.Length
            ? labels[safePosition]
            : string.Concat("POSITION ", safePosition + 1);

        StatusText = IsInterlocked
            ? string.Concat("INTERLOCK — ", InterlockReason)
            : "SELECTION AVAILABLE";
    }
}
