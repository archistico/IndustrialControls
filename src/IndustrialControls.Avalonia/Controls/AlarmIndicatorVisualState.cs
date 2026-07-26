namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Stato visuale derivato di un indicatore di allarme retroilluminato.
/// </summary>
public enum AlarmIndicatorVisualState
{
    Clear,
    NewAlarm,
    AcknowledgedActive,
    ReturnedUnacknowledged,
    ReadyToReset,
    Disabled
}
