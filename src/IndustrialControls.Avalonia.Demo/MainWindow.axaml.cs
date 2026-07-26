using Avalonia.Controls;
using Avalonia.Interactivity;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Demo;

public sealed partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void OnRaiseAlarms(object? sender, RoutedEventArgs e)
    {
        AlarmPanel.Activate("STEAM_LOW");
        AlarmPanel.Activate("VACUUM_LOW");
        AlarmPanel.Activate("PUMP_TRIP");
        AlarmPanel.Activate("FEED_LOW");
        AlarmPanel.Activate("GRID_DEVIATION");
        AlarmPanel.Activate("OIL_PRESSURE");

        UpdateStatus("NEW ALARMS — ACK REQUIRED", LedDisplayColor.Red);
    }

    private void OnAcknowledgeAll(object? sender, RoutedEventArgs e)
    {
        var count = AlarmPanel.AcknowledgeAll();

        UpdateStatus(
            count > 0
                ? $"ACKNOWLEDGED {count} ALARM INDICATORS"
                : "NO ALARMS TO ACKNOWLEDGE",
            count > 0
                ? LedDisplayColor.Amber
                : LedDisplayColor.Green);
    }

    private void OnClearConditions(object? sender, RoutedEventArgs e)
    {
        var count = AlarmPanel.ClearAllConditions();

        UpdateStatus(
            count > 0
                ? $"CONDITIONS RETURNED — {AlarmPanel.LatchedAlarmCount} LATCHED"
                : "NO ACTIVE CONDITIONS",
            AlarmPanel.LatchedAlarmCount > 0
                ? LedDisplayColor.Amber
                : LedDisplayColor.Green);
    }

    private void OnResetAll(object? sender, RoutedEventArgs e)
    {
        var count = AlarmPanel.ResetAll();

        UpdateStatus(
            count > 0
                ? $"RESET {count} LATCHED ALARMS"
                : "RESET NOT PERMITTED OR NOTHING LATCHED",
            AlarmPanel.LatchedAlarmCount == 0
                ? LedDisplayColor.Green
                : LedDisplayColor.Red);
    }

    private void UpdateStatus(
        string text,
        LedDisplayColor color)
    {
        StatusDisplay.Text = text;
        StatusDisplay.LedColor = color;
    }
}
