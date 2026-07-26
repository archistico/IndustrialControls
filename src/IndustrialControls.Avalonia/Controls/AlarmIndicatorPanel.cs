using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Pannello industriale per matrici di indicatori di allarme retroilluminati.
/// </summary>
public sealed class AlarmIndicatorPanel : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<AlarmIndicatorPanel, string>(
            nameof(Title),
            "ALARM INDICATOR PANEL");

    public static readonly StyledProperty<int> ColumnsProperty =
        AvaloniaProperty.Register<AlarmIndicatorPanel, int>(
            nameof(Columns),
            3,
            validate: value => value is >= 1 and <= 8);

    public static readonly StyledProperty<double> HorizontalGapProperty =
        AvaloniaProperty.Register<AlarmIndicatorPanel, double>(
            nameof(HorizontalGap),
            10.0,
            validate: value => value is >= 0 and <= 100);

    public static readonly StyledProperty<double> VerticalGapProperty =
        AvaloniaProperty.Register<AlarmIndicatorPanel, double>(
            nameof(VerticalGap),
            10.0,
            validate: value => value is >= 0 and <= 100);

    public static readonly StyledProperty<Thickness> PanelPaddingProperty =
        AvaloniaProperty.Register<AlarmIndicatorPanel, Thickness>(
            nameof(PanelPadding),
            new Thickness(14));

    public static readonly StyledProperty<bool> ShowFastenersProperty =
        AvaloniaProperty.Register<AlarmIndicatorPanel, bool>(
            nameof(ShowFasteners),
            true);

    public static readonly DirectProperty<
        AlarmIndicatorPanel,
        ObservableCollection<BacklitAlarmIndicator>> IndicatorsProperty =
        AvaloniaProperty.RegisterDirect<
            AlarmIndicatorPanel,
            ObservableCollection<BacklitAlarmIndicator>>(
                nameof(Indicators),
                control => control.Indicators);

    public static readonly DirectProperty<AlarmIndicatorPanel, int> ActiveConditionCountProperty =
        AvaloniaProperty.RegisterDirect<AlarmIndicatorPanel, int>(
            nameof(ActiveConditionCount),
            control => control.ActiveConditionCount);

    public static readonly DirectProperty<AlarmIndicatorPanel, int> LatchedAlarmCountProperty =
        AvaloniaProperty.RegisterDirect<AlarmIndicatorPanel, int>(
            nameof(LatchedAlarmCount),
            control => control.LatchedAlarmCount);

    public static readonly DirectProperty<AlarmIndicatorPanel, int> UnacknowledgedCountProperty =
        AvaloniaProperty.RegisterDirect<AlarmIndicatorPanel, int>(
            nameof(UnacknowledgedCount),
            control => control.UnacknowledgedCount);

    private readonly HashSet<BacklitAlarmIndicator> _trackedIndicators = new();

    private int _activeConditionCount;
    private int _latchedAlarmCount;
    private int _unacknowledgedCount;

    public AlarmIndicatorPanel()
    {
        Indicators =
            new ObservableCollection<BacklitAlarmIndicator>();

        Indicators.CollectionChanged +=
            OnIndicatorsCollectionChanged;

        RefreshCounts();
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    public double HorizontalGap
    {
        get => GetValue(HorizontalGapProperty);
        set => SetValue(HorizontalGapProperty, value);
    }

    public double VerticalGap
    {
        get => GetValue(VerticalGapProperty);
        set => SetValue(VerticalGapProperty, value);
    }

    public Thickness PanelPadding
    {
        get => GetValue(PanelPaddingProperty);
        set => SetValue(PanelPaddingProperty, value);
    }

    public bool ShowFasteners
    {
        get => GetValue(ShowFastenersProperty);
        set => SetValue(ShowFastenersProperty, value);
    }

    /// <summary>
    /// Raccolta logica degli indicatori.
    /// </summary>
    public ObservableCollection<BacklitAlarmIndicator> Indicators { get; }

    public int ActiveConditionCount
    {
        get => _activeConditionCount;
        private set => SetAndRaise(
            ActiveConditionCountProperty,
            ref _activeConditionCount,
            value);
    }

    public int LatchedAlarmCount
    {
        get => _latchedAlarmCount;
        private set => SetAndRaise(
            LatchedAlarmCountProperty,
            ref _latchedAlarmCount,
            value);
    }

    public int UnacknowledgedCount
    {
        get => _unacknowledgedCount;
        private set => SetAndRaise(
            UnacknowledgedCountProperty,
            ref _unacknowledgedCount,
            value);
    }

    public bool Activate(string alarmId)
    {
        var indicator = Indicators.FirstOrDefault(item =>
            string.Equals(
                item.AlarmId,
                alarmId,
                StringComparison.OrdinalIgnoreCase));

        if (indicator is null)
        {
            return false;
        }

        indicator.Activate();
        return true;
    }

    public int AcknowledgeAll()
    {
        var count = 0;

        foreach (var indicator in Indicators)
        {
            if (indicator.Acknowledge())
            {
                count++;
            }
        }

        return count;
    }

    public int ClearAllConditions()
    {
        var count = 0;

        foreach (var indicator in Indicators)
        {
            if (!indicator.IsConditionActive)
            {
                continue;
            }

            indicator.ClearCondition();
            count++;
        }

        return count;
    }

    public int ResetAll()
    {
        var count = 0;

        foreach (var indicator in Indicators)
        {
            if (indicator.Reset())
            {
                count++;
            }
        }

        return count;
    }

    private void OnIndicatorsCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            RebuildIndicatorSubscriptions();
            RefreshCounts();
            return;
        }

        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is BacklitAlarmIndicator indicator &&
                    !Indicators.Contains(indicator))
                {
                    UntrackIndicator(indicator);
                }
            }
        }

        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is BacklitAlarmIndicator indicator)
                {
                    TrackIndicator(indicator);
                }
            }
        }

        RefreshCounts();
    }

    private void RebuildIndicatorSubscriptions()
    {
        foreach (var indicator in _trackedIndicators)
        {
            indicator.PropertyChanged -=
                OnIndicatorPropertyChanged;
        }

        _trackedIndicators.Clear();

        foreach (var indicator in Indicators)
        {
            TrackIndicator(indicator);
        }
    }

    private void TrackIndicator(
        BacklitAlarmIndicator indicator)
    {
        if (!_trackedIndicators.Add(indicator))
        {
            return;
        }

        indicator.PropertyChanged +=
            OnIndicatorPropertyChanged;
    }

    private void UntrackIndicator(
        BacklitAlarmIndicator indicator)
    {
        if (!_trackedIndicators.Remove(indicator))
        {
            return;
        }

        indicator.PropertyChanged -=
            OnIndicatorPropertyChanged;
    }

    private void OnIndicatorPropertyChanged(
        object? sender,
        AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property ==
                BacklitAlarmIndicator.IsConditionActiveProperty ||
            e.Property ==
                BacklitAlarmIndicator.HasLatchedAlarmProperty ||
            e.Property ==
                BacklitAlarmIndicator.IsAcknowledgedProperty)
        {
            RefreshCounts();
        }
    }

    private void RefreshCounts()
    {
        ActiveConditionCount = Indicators.Count(
            indicator =>
                indicator.IsConditionActive);

        LatchedAlarmCount = Indicators.Count(
            indicator =>
                indicator.HasLatchedAlarm);

        UnacknowledgedCount = Indicators.Count(
            indicator =>
                (indicator.IsConditionActive ||
                 indicator.HasLatchedAlarm) &&
                !indicator.IsAcknowledged);
    }
}
