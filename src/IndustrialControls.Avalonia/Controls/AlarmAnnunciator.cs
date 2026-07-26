using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Annunciatore legacy compatto con lente circolare e priorità cromatica.
/// </summary>
public sealed class AlarmAnnunciator : TemplatedControl
{
    private static readonly IBrush AdvisoryBrush =
        new SolidColorBrush(Color.Parse("#57A8E8"));

    private static readonly IBrush CautionBrush =
        new SolidColorBrush(Color.Parse("#F2DD4B"));

    private static readonly IBrush WarningBrush =
        new SolidColorBrush(Color.Parse("#FFB238"));

    private static readonly IBrush CriticalBrush =
        new SolidColorBrush(Color.Parse("#F14C4C"));

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

    public static readonly DirectProperty<AlarmAnnunciator, bool> ShouldFlashProperty =
        AvaloniaProperty.RegisterDirect<AlarmAnnunciator, bool>(
            nameof(ShouldFlash),
            control => control.ShouldFlash);

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

    private bool _shouldFlash;
    private bool _blinkPhase = true;
    private Color _priorityColor = Color.Parse("#FFB238");
    private IBrush _priorityBrush = WarningBrush;
    private double _lampOpacity = 0.16;
    private string _stateText = "CLEAR";

    static AlarmAnnunciator()
    {
        TextProperty.Changed.AddClassHandler<AlarmAnnunciator>(
            (control, _) => control.RefreshState());

        PriorityProperty.Changed.AddClassHandler<AlarmAnnunciator>(
            (control, _) => control.RefreshState());

        IsActiveProperty.Changed.AddClassHandler<AlarmAnnunciator>(
            (control, _) => control.RefreshState());

        IsAcknowledgedProperty.Changed.AddClassHandler<AlarmAnnunciator>(
            (control, _) => control.RefreshState());
    }

    public AlarmAnnunciator()
    {
        _blinkTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(450)
        };

        _blinkTimer.Tick += OnBlinkTimerTick;
        RefreshState();
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

    public bool ShouldFlash
    {
        get => _shouldFlash;
        private set => SetAndRaise(
            ShouldFlashProperty,
            ref _shouldFlash,
            value);
    }

    /// <summary>
    /// Colore logico della priorità, leggibile senza accedere a un brush
    /// Avalonia vincolato al thread UI.
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
        IsActive = true;
        IsAcknowledged = false;
    }

    public void Acknowledge()
    {
        if (IsActive)
        {
            IsAcknowledged = true;
        }
    }

    public void Clear()
    {
        IsActive = false;

        if (!IsLatched)
        {
            IsAcknowledged = false;
        }
    }

    public void Reset()
    {
        if (!IsActive)
        {
            IsAcknowledged = false;
        }
    }

    private void RefreshState()
    {
        (PriorityColor, PriorityBrush) = Priority switch
        {
            AlarmPriority.Advisory => (
                Color.Parse("#57A8E8"),
                AdvisoryBrush),
            AlarmPriority.Caution => (
                Color.Parse("#F2DD4B"),
                CautionBrush),
            AlarmPriority.Critical => (
                Color.Parse("#F14C4C"),
                CriticalBrush),
            _ => (
                Color.Parse("#FFB238"),
                WarningBrush)
        };

        ShouldFlash =
            IsActive &&
            !IsAcknowledged;

        StateText = IsActive
            ? IsAcknowledged
                ? "ACK / ACTIVE"
                : "NEW ALARM"
            : IsAcknowledged
                ? "RETURNED / RESET"
                : "CLEAR";

        _blinkTimer.Stop();
        _blinkPhase = true;

        if (ShouldFlash)
        {
            _blinkTimer.Start();
        }

        RefreshLampOpacity();

        IndustrialAutomationMetadata.Apply(
            this,
            Text,
            string.Concat(
                StateText,
                "; priority ",
                Priority),
            "LegacyAlarmAnnunciator");
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
        LampOpacity = IsActive
            ? IsAcknowledged
                ? 0.88
                : _blinkPhase
                    ? 1.0
                    : 0.20
            : IsAcknowledged
                ? 0.34
                : 0.16;
    }
}
