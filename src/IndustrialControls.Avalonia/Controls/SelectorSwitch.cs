using System;
using System.Threading;
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
            nameof(PositionCount),
            3,
            validate: value => value is >= 2 and <= 5);

    public static readonly StyledProperty<int> PositionProperty =
        AvaloniaProperty.Register<SelectorSwitch, int>(
            nameof(Position),
            0,
            validate: value => value is >= 0 and <= 4);

    public static readonly StyledProperty<string> PositionLabelsProperty =
        AvaloniaProperty.Register<SelectorSwitch, string>(
            nameof(PositionLabels),
            "OFF|AUTO|MANUAL");

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SelectorSwitch, string>(
            nameof(Title),
            string.Empty);

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<SelectorSwitch, bool>(
            nameof(IsInterlocked));

    public static readonly StyledProperty<string> InterlockReasonProperty =
        AvaloniaProperty.Register<SelectorSwitch, string>(
            nameof(InterlockReason),
            "SELECTION NOT PERMITTED");

    public static readonly DirectProperty<SelectorSwitch, double> HandleAngleProperty =
        AvaloniaProperty.RegisterDirect<SelectorSwitch, double>(
            nameof(HandleAngle),
            control => control.HandleAngle);

    public static readonly DirectProperty<SelectorSwitch, string> SelectedLabelProperty =
        AvaloniaProperty.RegisterDirect<SelectorSwitch, string>(
            nameof(SelectedLabel),
            control => control.SelectedLabel);

    public static readonly DirectProperty<SelectorSwitch, string> StatusTextProperty =
        AvaloniaProperty.RegisterDirect<SelectorSwitch, string>(
            nameof(StatusText),
            control => control.StatusText);

    private readonly SynchronizationContext? _automationContext;
    private readonly SendOrPostCallback _flushAutomationMetadataCallback;

    private double _handleAngle = -60;
    private string _selectedLabel = "OFF";
    private string _statusText = "SELECTION AVAILABLE";
    private string[] _labels = Array.Empty<string>();
    private bool _automationRefreshPending;

    static SelectorSwitch()
    {
        PositionCountProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.OnPositionCountChanged());

        PositionProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.OnPositionChanged());

        TitleProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.RefreshState(true));

        PositionLabelsProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.RefreshLabelsAndState());

        IsInterlockedProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.RefreshState(true));

        InterlockReasonProperty.Changed.AddClassHandler<SelectorSwitch>(
            (control, _) => control.RefreshState(true));
    }

    public SelectorSwitch()
    {
        Focusable = true;
        _automationContext = SynchronizationContext.Current;
        _flushAutomationMetadataCallback =
            static state => ((SelectorSwitch)state!).FlushAutomationMetadata();

        RefreshLabelsAndState();
    }

    public int PositionCount
    {
        get => GetValue(PositionCountProperty);
        set => SetValue(PositionCountProperty, value);
    }

    public int Position
    {
        get => GetValue(PositionProperty);
        set => SetCurrentValue(
            PositionProperty,
            ClampPosition(value));
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
        private set => SetAndRaise(
            HandleAngleProperty,
            ref _handleAngle,
            value);
    }

    public string SelectedLabel
    {
        get => _selectedLabel;
        private set => SetAndRaise(
            SelectedLabelProperty,
            ref _selectedLabel,
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

    public bool Select(int position)
    {
        if (IsInterlocked)
        {
            return false;
        }

        Position = position;
        return true;
    }

    public bool SelectNext() =>
        Select(Math.Min(
            Position + 1,
            PositionCount - 1));

    public bool SelectPrevious() =>
        Select(Math.Max(
            Position - 1,
            0));

    protected override void OnPointerPressed(
        PointerPressedEventArgs e)
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

    private void OnPositionCountChanged()
    {
        if (NormalizePosition())
        {
            return;
        }

        RefreshState(true);
    }

    private void OnPositionChanged()
    {
        if (NormalizePosition())
        {
            return;
        }

        RefreshState(false);
    }

    private bool NormalizePosition()
    {
        var normalized = ClampPosition(Position);

        if (normalized == Position)
        {
            return false;
        }

        SetCurrentValue(
            PositionProperty,
            normalized);

        return true;
    }

    private int ClampPosition(int position) =>
        Math.Clamp(
            position,
            0,
            Math.Max(0, PositionCount - 1));

    private void RefreshLabelsAndState()
    {
        _labels = PositionLabels.Split(
            '|',
            StringSplitOptions.TrimEntries |
            StringSplitOptions.RemoveEmptyEntries);

        RefreshState(true);
    }

    private void RefreshState(
        bool refreshAutomationImmediately)
    {
        var count = Math.Max(
            2,
            PositionCount);

        HandleAngle =
            -60.0 +
            (Position * (120.0 / (count - 1)));

        SelectedLabel = Position < _labels.Length
            ? _labels[Position]
            : string.Concat(
                "POSITION ",
                Position + 1);

        StatusText = IsInterlocked
            ? string.Concat(
                "INTERLOCK — ",
                InterlockReason)
            : "SELECTION AVAILABLE";

        if (refreshAutomationImmediately)
        {
            RefreshAutomationMetadata();
        }
        else
        {
            RequestAutomationMetadataRefresh();
        }
    }

    private void RequestAutomationMetadataRefresh()
    {
        if (_automationRefreshPending)
        {
            return;
        }

        if (_automationContext is null)
        {
            RefreshAutomationMetadata();
            return;
        }

        _automationRefreshPending = true;
        _automationContext.Post(
            _flushAutomationMetadataCallback,
            this);
    }

    private void FlushAutomationMetadata()
    {
        _automationRefreshPending = false;
        RefreshAutomationMetadata();
    }

    private void RefreshAutomationMetadata()
    {
        IndustrialAutomationMetadata.Apply(
            this,
            Title,
            string.Concat(
                "Selected ",
                SelectedLabel,
                "; ",
                StatusText),
            "SelectorSwitch");
    }
}
