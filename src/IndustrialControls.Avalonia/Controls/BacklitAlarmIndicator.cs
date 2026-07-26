using System;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Indicatore di allarme retroilluminato con ACK, rientro e memoria latched.
/// </summary>
public sealed class BacklitAlarmIndicator : TemplatedControl
{
    public static readonly StyledProperty<string> AlarmIdProperty =
        AvaloniaProperty.Register<BacklitAlarmIndicator, string>(
            nameof(AlarmId),
            string.Empty);

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<BacklitAlarmIndicator, string>(
            nameof(Text),
            string.Empty);

    public static readonly StyledProperty<string> SecondaryTextProperty =
        AvaloniaProperty.Register<BacklitAlarmIndicator, string>(
            nameof(SecondaryText),
            string.Empty);

    public static readonly StyledProperty<AlarmPriority> PriorityProperty =
        AvaloniaProperty.Register<BacklitAlarmIndicator, AlarmPriority>(
            nameof(Priority),
            AlarmPriority.Warning);

    public static readonly StyledProperty<bool> IsConditionActiveProperty =
        AvaloniaProperty.Register<BacklitAlarmIndicator, bool>(
            nameof(IsConditionActive));

    public static readonly StyledProperty<bool> IsAcknowledgedProperty =
        AvaloniaProperty.Register<BacklitAlarmIndicator, bool>(
            nameof(IsAcknowledged));

    public static readonly StyledProperty<bool> IsLatchedProperty =
        AvaloniaProperty.Register<BacklitAlarmIndicator, bool>(
            nameof(IsLatched),
            true);

    public static readonly StyledProperty<int> FlashIntervalMillisecondsProperty =
        AvaloniaProperty.Register<BacklitAlarmIndicator, int>(
            nameof(FlashIntervalMilliseconds),
            450,
            validate: value => value is >= 100 and <= 5000);

    public static readonly DirectProperty<BacklitAlarmIndicator, bool> HasLatchedAlarmProperty =
        AvaloniaProperty.RegisterDirect<BacklitAlarmIndicator, bool>(
            nameof(HasLatchedAlarm),
            control => control.HasLatchedAlarm);

    public static readonly DirectProperty<BacklitAlarmIndicator, bool> ShouldFlashProperty =
        AvaloniaProperty.RegisterDirect<BacklitAlarmIndicator, bool>(
            nameof(ShouldFlash),
            control => control.ShouldFlash);

    public static readonly DirectProperty<BacklitAlarmIndicator, bool> IsIlluminatedProperty =
        AvaloniaProperty.RegisterDirect<BacklitAlarmIndicator, bool>(
            nameof(IsIlluminated),
            control => control.IsIlluminated);

    public static readonly DirectProperty<BacklitAlarmIndicator, double> EffectiveOpacityProperty =
        AvaloniaProperty.RegisterDirect<BacklitAlarmIndicator, double>(
            nameof(EffectiveOpacity),
            control => control.EffectiveOpacity);

    public static readonly DirectProperty<BacklitAlarmIndicator, string> StateTextProperty =
        AvaloniaProperty.RegisterDirect<BacklitAlarmIndicator, string>(
            nameof(StateText),
            control => control.StateText);

    public static readonly DirectProperty<BacklitAlarmIndicator, AlarmIndicatorVisualState> VisualStateProperty =
        AvaloniaProperty.RegisterDirect<BacklitAlarmIndicator, AlarmIndicatorVisualState>(
            nameof(VisualState),
            control => control.VisualState);

    public static readonly DirectProperty<BacklitAlarmIndicator, IBrush> DisplayBrushProperty =
        AvaloniaProperty.RegisterDirect<BacklitAlarmIndicator, IBrush>(
            nameof(DisplayBrush),
            control => control.DisplayBrush);

    public static readonly DirectProperty<BacklitAlarmIndicator, IBrush> DisplayForegroundBrushProperty =
        AvaloniaProperty.RegisterDirect<BacklitAlarmIndicator, IBrush>(
            nameof(DisplayForegroundBrush),
            control => control.DisplayForegroundBrush);

    private readonly DispatcherTimer _blinkTimer;

    private bool _hasLatchedAlarm;
    private bool _shouldFlash;
    private bool _isIlluminated;
    private bool _blinkPhase = true;
    private bool _previousConditionActive;
    private double _effectiveOpacity = 0.18;
    private string _stateText = "CLEAR";
    private AlarmIndicatorVisualState _visualState =
        AlarmIndicatorVisualState.Clear;
    private IBrush _displayBrush =
        new SolidColorBrush(
            Color.Parse("#FFB238"));
    private IBrush _displayForegroundBrush =
        new SolidColorBrush(
            Color.Parse("#111315"));

    static BacklitAlarmIndicator()
    {
        AlarmIdProperty.Changed.AddClassHandler<BacklitAlarmIndicator>(
            (control, _) => control.RefreshAutomationMetadata());

        TextProperty.Changed.AddClassHandler<BacklitAlarmIndicator>(
            (control, _) => control.RefreshAutomationMetadata());

        SecondaryTextProperty.Changed.AddClassHandler<BacklitAlarmIndicator>(
            (control, _) => control.RefreshAutomationMetadata());

        IsConditionActiveProperty.Changed.AddClassHandler<BacklitAlarmIndicator>(
            (control, _) => control.OnConditionChanged());

        IsAcknowledgedProperty.Changed.AddClassHandler<BacklitAlarmIndicator>(
            (control, _) => control.RefreshOperationalState());

        IsLatchedProperty.Changed.AddClassHandler<BacklitAlarmIndicator>(
            (control, _) => control.RefreshOperationalState());

        PriorityProperty.Changed.AddClassHandler<BacklitAlarmIndicator>(
            (control, _) => control.OnPriorityChanged());

        FlashIntervalMillisecondsProperty.Changed.AddClassHandler<BacklitAlarmIndicator>(
            (control, _) => control.UpdateBlinkInterval());

        IsEnabledProperty.Changed.AddClassHandler<BacklitAlarmIndicator>(
            (control, _) => control.RefreshOperationalState());
    }

    public BacklitAlarmIndicator()
    {
        _blinkTimer = new DispatcherTimer();
        _blinkTimer.Tick += OnBlinkTimerTick;

        _previousConditionActive =
            IsConditionActive;

        UpdateBlinkInterval();
        RefreshOperationalState();
    }

    public string AlarmId
    {
        get => GetValue(AlarmIdProperty);
        set => SetValue(AlarmIdProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string SecondaryText
    {
        get => GetValue(SecondaryTextProperty);
        set => SetValue(SecondaryTextProperty, value);
    }

    public AlarmPriority Priority
    {
        get => GetValue(PriorityProperty);
        set => SetValue(PriorityProperty, value);
    }

    public bool IsConditionActive
    {
        get => GetValue(IsConditionActiveProperty);
        set => SetValue(IsConditionActiveProperty, value);
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

    public int FlashIntervalMilliseconds
    {
        get => GetValue(FlashIntervalMillisecondsProperty);
        set => SetValue(
            FlashIntervalMillisecondsProperty,
            value);
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

    public bool IsIlluminated
    {
        get => _isIlluminated;
        private set => SetAndRaise(
            IsIlluminatedProperty,
            ref _isIlluminated,
            value);
    }

    public double EffectiveOpacity
    {
        get => _effectiveOpacity;
        private set => SetAndRaise(
            EffectiveOpacityProperty,
            ref _effectiveOpacity,
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

    public AlarmIndicatorVisualState VisualState
    {
        get => _visualState;
        private set => SetAndRaise(
            VisualStateProperty,
            ref _visualState,
            value);
    }

    public IBrush DisplayBrush
    {
        get => _displayBrush;
        private set => SetAndRaise(
            DisplayBrushProperty,
            ref _displayBrush,
            value);
    }

    public IBrush DisplayForegroundBrush
    {
        get => _displayForegroundBrush;
        private set => SetAndRaise(
            DisplayForegroundBrushProperty,
            ref _displayForegroundBrush,
            value);
    }

    public void Activate() =>
        IsConditionActive = true;

    public bool Acknowledge()
    {
        if (!IsConditionActive &&
            !HasLatchedAlarm)
        {
            return false;
        }

        IsAcknowledged = true;
        return true;
    }

    public void ClearCondition() =>
        IsConditionActive = false;

    public bool Reset()
    {
        if (IsConditionActive ||
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
        var isActive =
            IsConditionActive;

        if (isActive &&
            !_previousConditionActive)
        {
            HasLatchedAlarm = true;
            IsAcknowledged = false;
        }
        else if (!isActive &&
                 _previousConditionActive &&
                 !IsLatched)
        {
            HasLatchedAlarm = false;
            IsAcknowledged = false;
        }

        _previousConditionActive = isActive;
        RefreshOperationalState();
    }

    private void OnPriorityChanged()
    {
        RefreshVisualAppearance();
        RefreshAutomationMetadata();
    }

    private void RefreshOperationalState()
    {
        VisualState =
            CalculateVisualState();

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

        RefreshVisualAppearance();
        RefreshAutomationMetadata();
    }

    private AlarmIndicatorVisualState CalculateVisualState()
    {
        if (!IsEnabled)
        {
            return AlarmIndicatorVisualState.Disabled;
        }

        if (IsConditionActive)
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

    private void UpdateBlinkInterval()
    {
        _blinkTimer.Interval =
            TimeSpan.FromMilliseconds(
                FlashIntervalMilliseconds);
    }

    private void OnBlinkTimerTick(
        object? sender,
        EventArgs e)
    {
        _blinkPhase = !_blinkPhase;
        RefreshAppearance();
    }

    private void RefreshVisualAppearance()
    {
        var activeColor = Priority switch
        {
            AlarmPriority.Advisory =>
                Color.Parse("#57A8E8"),
            AlarmPriority.Caution =>
                Color.Parse("#F2DD4B"),
            AlarmPriority.Critical =>
                Color.Parse("#F14C4C"),
            _ =>
                Color.Parse("#FFB238")
        };

        DisplayBrush =
            new SolidColorBrush(
                activeColor);

        var foregroundColor = VisualState switch
        {
            AlarmIndicatorVisualState.Clear =>
                Color.Parse("#D9D2BC"),
            AlarmIndicatorVisualState.Disabled =>
                Color.Parse("#8C908A"),
            _ when Priority ==
                AlarmPriority.Critical =>
                    Color.Parse("#F7F7ED"),
            _ =>
                Color.Parse("#111315")
        };

        DisplayForegroundBrush =
            new SolidColorBrush(
                foregroundColor);

        RefreshAppearance();
    }

    private void RefreshAppearance()
    {
        EffectiveOpacity = VisualState switch
        {
            AlarmIndicatorVisualState.Clear =>
                0.18,
            AlarmIndicatorVisualState.Disabled =>
                0.10,
            AlarmIndicatorVisualState.AcknowledgedActive =>
                0.88,
            AlarmIndicatorVisualState.ReadyToReset =>
                0.52,
            _ when ShouldFlash &&
                !_blinkPhase =>
                    0.24,
            _ =>
                1.0
        };

        IsIlluminated =
            VisualState is not
                AlarmIndicatorVisualState.Clear and not
                AlarmIndicatorVisualState.Disabled &&
            (!ShouldFlash ||
             _blinkPhase);
    }

    private void RefreshAutomationMetadata()
    {
        IndustrialAutomationMetadata.Apply(
            this,
            Text,
            string.Concat(
                SecondaryText,
                "; ",
                StateText),
            string.IsNullOrWhiteSpace(AlarmId)
                ? "BacklitAlarm"
                : string.Concat(
                    "BacklitAlarm.",
                    AlarmId));

        IndustrialAutomationMetadata.SetLiveRegion(
            this,
            VisualState is
                AlarmIndicatorVisualState.NewAlarm or
                AlarmIndicatorVisualState.ReturnedUnacknowledged
                ? AutomationLiveSetting.Assertive
                : AutomationLiveSetting.Polite);
    }
}
