using System;
using System.Collections.ObjectModel;
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
            nameof(Title), "ALARM INDICATOR PANEL");

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
            nameof(PanelPadding), new Thickness(14));

    public static readonly StyledProperty<bool> ShowFastenersProperty =
        AvaloniaProperty.Register<AlarmIndicatorPanel, bool>(
            nameof(ShowFasteners), true);

    public static readonly DirectProperty<
        AlarmIndicatorPanel,
        ObservableCollection<BacklitAlarmIndicator>> IndicatorsProperty =
        AvaloniaProperty.RegisterDirect<
            AlarmIndicatorPanel,
            ObservableCollection<BacklitAlarmIndicator>>(
                nameof(Indicators),
                control => control.Indicators);

    public AlarmIndicatorPanel()
    {
        Indicators = new ObservableCollection<BacklitAlarmIndicator>();
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
    /// Raccolta logica degli indicatori. Non dipende da ItemsControl.Items
    /// e può essere usata nei test senza accesso al dispatcher Avalonia.
    /// </summary>
    public ObservableCollection<BacklitAlarmIndicator> Indicators { get; }

    public int ActiveConditionCount =>
        Indicators.Count(indicator => indicator.IsConditionActive);

    public int LatchedAlarmCount =>
        Indicators.Count(indicator => indicator.HasLatchedAlarm);

    public int UnacknowledgedCount =>
        Indicators.Count(indicator =>
            (indicator.IsConditionActive || indicator.HasLatchedAlarm) &&
            !indicator.IsAcknowledged);

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

        foreach (var indicator in Indicators.Where(item => item.IsConditionActive))
        {
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
}
