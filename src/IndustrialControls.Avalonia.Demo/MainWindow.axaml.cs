using Avalonia.Controls;
using Avalonia.Interactivity;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Demo;

public sealed partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void OnApplyInterlock(object? sender, RoutedEventArgs e)
    {
        SetInterlockState(true);

        InterlockState.IsInterlocked = true;
        InterlockState.SatisfiedPermissiveCount = 2;
        InterlockState.RequiredPermissiveCount = 4;
        InterlockState.Reason =
            "SYNCHRONISM AND PROTECTION PERMISSIVES NOT SATISFIED";

        StatusDisplay.Text =
            "OPERATOR COMMANDS INTERLOCKED - CORRECT PERMISSIVES";
        StatusDisplay.LedColor = LedDisplayColor.Red;
    }

    private void OnClearInterlock(object? sender, RoutedEventArgs e)
    {
        SetInterlockState(false);

        InterlockState.IsInterlocked = false;
        InterlockState.SatisfiedPermissiveCount = 4;
        InterlockState.RequiredPermissiveCount = 4;
        InterlockState.Reason =
            "SPEED, VOLTAGE, PHASE AND PROTECTION PERMISSIVES SATISFIED";

        StatusDisplay.Text = "ALL OPERATOR CONTROLS AVAILABLE";
        StatusDisplay.LedColor = LedDisplayColor.Green;
    }

    private void SetInterlockState(bool isInterlocked)
    {
        FeedwaterSlider.IsInterlocked = isInterlocked;
        ValveSlider.IsInterlocked = isInterlocked;
        LoadKnob.IsInterlocked = isInterlocked;
        ModeSelector.IsInterlocked = isInterlocked;
        BreakerSwitch.IsInterlocked = isInterlocked;
        PumpSwitch.IsInterlocked = isInterlocked;
        SpeedTrimSwitch.IsInterlocked = isInterlocked;
    }
}
