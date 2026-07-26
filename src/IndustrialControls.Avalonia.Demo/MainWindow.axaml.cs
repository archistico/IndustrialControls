using Avalonia.Controls;
using Avalonia.Interactivity;

namespace IndustrialControls.Avalonia.Demo;

public sealed partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void OnLoadRaise(object? sender, RoutedEventArgs e)
    {
        PowerGauge.Value = System.Math.Min(PowerGauge.Maximum, PowerGauge.Value + 0.25);
        ValveGauge.Value = System.Math.Min(100, ValveGauge.Value + 2.5);
        DeviationGauge.Value = PowerGauge.Value;
        StatusDisplay.Text = $"LOAD RAISED - {PowerGauge.Value:0.00} MWe";
        StatusDisplay.LedColor = IndustrialControls.Avalonia.Controls.LedDisplayColor.Green;
    }

    private void OnLoadLower(object? sender, RoutedEventArgs e)
    {
        PowerGauge.Value = System.Math.Max(PowerGauge.Minimum, PowerGauge.Value - 0.25);
        ValveGauge.Value = System.Math.Max(0, ValveGauge.Value - 2.5);
        DeviationGauge.Value = PowerGauge.Value;
        StatusDisplay.Text = $"LOAD LOWERED - {PowerGauge.Value:0.00} MWe";
        StatusDisplay.LedColor = IndustrialControls.Avalonia.Controls.LedDisplayColor.Amber;
    }

    private void OnDisturb(object? sender, RoutedEventArgs e)
    {
        LevelGauge.Value = LevelGauge.Value > 25 ? 18 : 52;
        TemperatureGauge.Value = TemperatureGauge.Value < 320 ? 336 : 278.4;
        StatusDisplay.Text = LevelGauge.Value < 20
            ? "PROCESS DISTURBANCE - WARNING THRESHOLDS ACTIVE"
            : "PROCESS RESTORED - PARAMETERS NORMAL";
        StatusDisplay.LedColor = LevelGauge.Value < 20
            ? IndustrialControls.Avalonia.Controls.LedDisplayColor.Red
            : IndustrialControls.Avalonia.Controls.LedDisplayColor.Green;
    }
}
