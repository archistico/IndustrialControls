using Avalonia;
using Avalonia.Controls.Primitives;

namespace IndustrialControls.Avalonia.Controls;

public sealed class AlarmAnnunciator : TemplatedControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<AlarmAnnunciator, string>(nameof(Text), string.Empty);

    public static readonly StyledProperty<AlarmPriority> PriorityProperty =
        AvaloniaProperty.Register<AlarmAnnunciator, AlarmPriority>(nameof(Priority), AlarmPriority.Warning);

    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<AlarmAnnunciator, bool>(nameof(IsActive));

    public static readonly StyledProperty<bool> IsAcknowledgedProperty =
        AvaloniaProperty.Register<AlarmAnnunciator, bool>(nameof(IsAcknowledged));

    public static readonly StyledProperty<bool> IsLatchedProperty =
        AvaloniaProperty.Register<AlarmAnnunciator, bool>(nameof(IsLatched), true);

    public static readonly DirectProperty<AlarmAnnunciator, bool> ShouldFlashProperty =
        AvaloniaProperty.RegisterDirect<AlarmAnnunciator, bool>(
            nameof(ShouldFlash), control => control.ShouldFlash);

    private bool _shouldFlash;

    static AlarmAnnunciator()
    {
        IsActiveProperty.Changed.AddClassHandler<AlarmAnnunciator>((control, _) => control.RefreshState());
        IsAcknowledgedProperty.Changed.AddClassHandler<AlarmAnnunciator>((control, _) => control.RefreshState());
    }

    public AlarmAnnunciator() => RefreshState();

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
        private set => SetAndRaise(ShouldFlashProperty, ref _shouldFlash, value);
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

    private void RefreshState() => ShouldFlash = IsActive && !IsAcknowledged;
}
