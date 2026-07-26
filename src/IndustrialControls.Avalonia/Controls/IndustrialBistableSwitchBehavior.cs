using System;
using Avalonia.Controls.Primitives;

namespace IndustrialControls.Avalonia.Controls;

internal readonly record struct IndustrialBistableSwitchState(
    bool IsOn,
    string StateText,
    string StatusText);

internal static class IndustrialBistableSwitchBehavior
{
    public static IndustrialBistableSwitchState Evaluate(
        bool? isChecked,
        string onCaption,
        string offCaption,
        bool isInterlocked,
        string interlockReason)
    {
        var isOn = isChecked == true;

        return new IndustrialBistableSwitchState(
            isOn,
            isOn
                ? onCaption
                : offCaption,
            isInterlocked
                ? string.Concat(
                    "INTERLOCK — ",
                    interlockReason)
                : "SWITCHING AVAILABLE");
    }

    public static bool TryToggle(
        ToggleButton control,
        bool isInterlocked)
    {
        ArgumentNullException.ThrowIfNull(control);

        if (isInterlocked)
        {
            return false;
        }

        control.IsChecked =
            control.IsChecked != true;

        return true;
    }

    public static bool CanInvoke(bool isInterlocked) =>
        !isInterlocked;

    public static string BuildAutomationHelp(
        IndustrialBistableSwitchState state) =>
        string.Concat(
            "State ",
            state.StateText,
            "; ",
            state.StatusText);
}
