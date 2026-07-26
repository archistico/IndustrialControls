using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Annunciatore legacy compatto con memoria latched, riconoscimento e reset.
/// </summary>
public sealed class AlarmAnnunciator : TemplatedControl
{
    private static readonly Color AdvisoryColor =
        Color.Parse("#57A8E8");
    private static readonly Color CautionColor =
        Color.Parse("#F2DD4B");
    private static readonly Color WarningColor =
        Color.Parse("#FFB238");
    private static readonly Color CriticalColor =
        Color.Parse("#F14C4C");

    private static readonly IBrush AdvisoryBrush =
        new SolidColorBrush(AdvisoryColor);
    private static readonly IBrush CautionBrush =
        new SolidColorBrush(CautionColor);
    private static readonly IBrush WarningBrush =
        new SolidColorBrush(WarningColor);
    private static readonly IBrush CriticalBrush =
        new SolidColorBrush(CriticalColor);

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<AlarmAnnunciator, string>(
            nameof(Text),
            string.Empty);

    public static readonly StyledProperty<AlarmPriority> PriorityProperty =
        AvaloniaProperty.Register<AlarmAnnunciator, AlarmPriority>(
            nameof(Priority),
            AlarmPriority.Warning);

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<AlarmAnnunciator, bool>(
            nameof(IsActive));

    public static readonly StyledProperty<bool> IsAcknowledgedProperty =
        AvaloniaProperty.Register<AlarmAnnunciator, bool>(
            nameof(IsAcknowledged));

    public static readonly StyledProperty<bool> IsLatchedProperty =
        AvaloniaProperty.Register<AlarmAnnunciator, bool>(
            nameof(IsLatched),
            true);

    public static readonly DirectProperty<AlarmAnnunciator, bool> HasLatchedAlarmProperty =
        AvaloniaProperty.RegisterDirect<AlarmAnnunciator, bool>(
            nameof(HasLatchedAlarm),
            control => control.HasLatchedAlarm);

    public static readonly DirectProperty<AlarmAnnunciator, bool> ShouldFlashProperty =
        AvaloniaProperty.RegisterDirect<AlarmAnnunciator, bool>(
            nameof(ShouldFlash),
            control => control.ShouldFlash);

    public static readonly DirectProperty<AlarmAnnunciator, AlarmIndicatorVisualState> VisualStateProperty =
        AvaloniaProperty.RegisterDirect<AlarmAnnunciator, AlarmIndicatorVisualState>(
            nameof(VisualState),
            control => control.VisualState);

    public static readonly DirectProperty<AlarmAnnunciator, Color> PriorityColorProperty =
        AvaloniaProperty.RegisterDirect<AlarmAnnunciator, Color>(
            nameof(PriorityColor),
            control => control.PriorityColor);

    public static readonly DirectProperty<AlarmAnnunciator, IBrush> PriorityBrushProperty =
        AvaloniaProperty.RegisterDirect<AlarmAnnunciator, IBrush>(
            nameof(PriorityBrush),
            control => control.PriorityBrush);

    public static readonly DirectProperty<AlarmAnnunciator, double> LampOpacityProperty =
        AvaloniaProperty.RegisterDirect<AlarmAnnunciator, double>(
            nameof(LampOpacity),
            control => control.LampOpacity);

    public static readonly DirectProperty<AlarmAnnunciator, string> StateTextProperty =
        AvaloniaProperty.RegisterDirect<AlarmAnnunciator, string>(
            nameof(StateText),
            control => control.StateText);

    private readonly DispatcherTimer _blinkTimer;

    private bool _hasLatchedAlarm;
    private bool _shouldFlash;
    private bool _previousIsActive;
    private bool _blinkPhase = true;
    private AlarmIndicatorVisualState _visualState =
        AlarmIndicatorVisualState.Clear;
    private Color _priorityColor = WarningColor;
    private IBrush _priorityBrush = WarningBrush;
    private double _lampOpacity = 0.16;
    private string _stateText = "CLEAR";

    static AlarmAnnunciator()
    {
        TextProperty.Changed.AddClassHandler<AlarmAnnunciator>(
            (control, _) => control.RefreshAutomationMetadata());

        PriorityProperty.Changed.AddClassHandler<AlarmAnnunciator>(
            (control, _) => control.RefreshPriority());

        IsActiveProperty.Changed.AddClassHandler<AlarmAnnunciator>(
            (control, _) => control.OnConditionChanged());

        IsAcknowledgedProperty.Changed.AddClassHandler<AlarmAnnunciator>(
            (control, _) => control.RefreshOperationalState());

        IsLatchedProperty.Changed.AddClassHandler<AlarmAnnunciator>(
            (control, _) => control.OnLatchingChanged());

        IsEnabledProperty.Changed.AddClassHandler<AlarmAnnunciator>(
            (control, _) => control.RefreshOperationalState());
    }

    public AlarmAnnunciator()
    {
        _blinkTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(450)
        };

        _blinkTimer.Tick += OnBlinkTimerTick;
        _previousIsActive = IsActive;

        RefreshPriority();
        RefreshOperationalState();
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public AlarmPriority Priority
    {
        get => GetValue(PriorityProperty);
        set => SetValue(PriorityProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    public bool IsAcknowledged
    {
        get => GetValue(IsAcknowledgedProperty);
        set => SetValue(IsAcknowledgedProperty, value);
    }

    public bool IsLatched
    {
        get => GetValue(IsLatchedProperty);
        set => SetValue(IsLatchedProperty, value);
    }

    public bool HasLatchedAlarm
    {
        get => _hasLatchedAlarm;
        private set => SetAndRaise(
            HasLatchedAlarmProperty,
            ref _hasLatchedAlarm,
            value);
    }

    public bool ShouldFlash
    {
        get => _shouldFlash;
        private set => SetAndRaise(
            ShouldFlashProperty,
            ref _shouldFlash,
            value);
    }

    public AlarmIndicatorVisualState VisualState
    {
        get => _visualState;
        private set => SetAndRaise(
            VisualStateProperty,
            ref _visualState,
            value);
    }

    /// <summary>
    /// Dispatcher-independent logical priority color.
    /// </summary>
    public Color PriorityColor
    {
        get => _priorityColor;
        private set => SetAndRaise(
            PriorityColorProperty,
            ref _priorityColor,
            value);
    }

    public IBrush PriorityBrush
    {
        get => _priorityBrush;
        private set => SetAndRaise(
            PriorityBrushProperty,
            ref _priorityBrush,
            value);
    }

    public double LampOpacity
    {
        get => _lampOpacity;
        private set => SetAndRaise(
            LampOpacityProperty,
            ref _lampOpacity,
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

    public void Activate()
    {
        HasLatchedAlarm = true;
        IsAcknowledged = false;

        if (!IsActive)
        {
            IsActive = true;
        }
        else
        {
            RefreshOperationalState();
        }
    }

    public void Acknowledge() => TryAcknowledge();

    public bool TryAcknowledge()
    {
        if ((!IsActive && !HasLatchedAlarm) ||
            IsAcknowledged)
        {
            return false;
        }

        IsAcknowledged = true;
        return true;
    }

    public void Clear() => IsActive = false;

    public void Reset() => TryReset();

    public bool TryReset()
    {
        if (IsActive ||
            !HasLatchedAlarm ||
            !IsAcknowledged)
        {
            return false;
        }

        HasLatchedAlarm = false;
        IsAcknowledged = false;
        RefreshOperationalState();
        return true;
    }

    private void OnConditionChanged()
    {
        var isActive = IsActive;

        if (isActive && !_previousIsActive)
        {
            HasLatchedAlarm = true;

            if (IsAcknowledged)
            {
                IsAcknowledged = false;
            }
        }
        else if (!isActive &&
                 _previousIsActive &&
                 !IsLatched)
        {
            HasLatchedAlarm = false;

            if (IsAcknowledged)
            {
                IsAcknowledged = false;
            }
        }

        _previousIsActive = isActive;
        RefreshOperationalState();
    }

    private void OnLatchingChanged()
    {
        if (!IsLatched &&
            !IsActive)
        {
            HasLatchedAlarm = false;

            if (IsAcknowledged)
            {
                IsAcknowledged = false;
            }
        }

        RefreshOperationalState();
    }

    private void RefreshPriority()
    {
        (PriorityColor, PriorityBrush) = Priority switch
        {
            AlarmPriority.Advisory => (
                AdvisoryColor,
                AdvisoryBrush),
            AlarmPriority.Caution => (
                CautionColor,
                CautionBrush),
            AlarmPriority.Critical => (
                CriticalColor,
                CriticalBrush),
            _ => (
                WarningColor,
                WarningBrush)
        };

        RefreshAutomationMetadata();
    }

    private void RefreshOperationalState()
    {
        VisualState = CalculateVisualState();

        StateText = VisualState switch
        {
            AlarmIndicatorVisualState.NewAlarm =>
                "NEW ALARM",
            AlarmIndicatorVisualState.AcknowledgedActive =>
                "ACK / ACTIVE",
            AlarmIndicatorVisualState.ReturnedUnacknowledged =>
                "RETURNED / ACK",
            AlarmIndicatorVisualState.ReadyToReset =>
                "READY TO RESET",
            AlarmIndicatorVisualState.Disabled =>
                "UNAVAILABLE",
            _ =>
                "CLEAR"
        };

        UpdateBlinkSchedule(
            VisualState is
                AlarmIndicatorVisualState.NewAlarm or
                AlarmIndicatorVisualState.ReturnedUnacknowledged);

        RefreshLampOpacity();
        RefreshAutomationMetadata();
    }

    private AlarmIndicatorVisualState CalculateVisualState()
    {
        if (!IsEnabled)
        {
            return AlarmIndicatorVisualState.Disabled;
        }

        if (IsActive)
        {
            return IsAcknowledged
                ? AlarmIndicatorVisualState.AcknowledgedActive
                : AlarmIndicatorVisualState.NewAlarm;
        }

        if (HasLatchedAlarm)
        {
            return IsAcknowledged
                ? AlarmIndicatorVisualState.ReadyToReset
                : AlarmIndicatorVisualState.ReturnedUnacknowledged;
        }

        return AlarmIndicatorVisualState.Clear;
    }

    private void UpdateBlinkSchedule(bool shouldFlash)
    {
        if (ShouldFlash == shouldFlash)
        {
            return;
        }

        ShouldFlash = shouldFlash;
        _blinkTimer.Stop();
        _blinkPhase = true;

        if (ShouldFlash)
        {
            _blinkTimer.Start();
        }
    }

    private void OnBlinkTimerTick(
        object? sender,
        EventArgs e)
    {
        _blinkPhase = !_blinkPhase;
        RefreshLampOpacity();
    }

    private void RefreshLampOpacity()
    {
        LampOpacity = VisualState switch
        {
            AlarmIndicatorVisualState.Clear =>
                0.16,
            AlarmIndicatorVisualState.Disabled =>
                0.08,
            AlarmIndicatorVisualState.AcknowledgedActive =>
                0.88,
            AlarmIndicatorVisualState.ReadyToReset =>
                0.34,
            _ when ShouldFlash && !_blinkPhase =>
                0.20,
            _ =>
                1.0
        };
    }

    private void RefreshAutomationMetadata()
    {
        IndustrialAutomationMetadata.Apply(
            this,
            Text,
            string.Concat(
                StateText,
                "; priority ",
                Priority),
            "LegacyAlarmAnnunciator");
    }
}
