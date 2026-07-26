using System;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia;
using Avalonia.Automation;

namespace IndustrialControls.Avalonia.Controls;

internal static class IndustrialAutomationMetadata
{
    private static readonly ConditionalWeakTable<StyledElement, MetadataState> States =
        new();

    public static void Apply(
        StyledElement element,
        string? name,
        string? helpText,
        string automationPrefix)
    {
        ArgumentNullException.ThrowIfNull(element);
        ArgumentException.ThrowIfNullOrWhiteSpace(automationPrefix);

        var state = States.GetOrCreateValue(element);
        var normalizedName = NormalizeName(name, element.GetType().Name);
        var normalizedHelpText = NormalizeOptionalText(helpText);

        if (!string.Equals(
                state.Name,
                normalizedName,
                StringComparison.Ordinal))
        {
            AutomationProperties.SetName(element, normalizedName);
            state.Name = normalizedName;
        }

        if (!string.Equals(
                state.HelpText,
                normalizedHelpText,
                StringComparison.Ordinal))
        {
            AutomationProperties.SetHelpText(element, normalizedHelpText);
            state.HelpText = normalizedHelpText;
        }

        if (!string.Equals(
                state.AutomationPrefix,
                automationPrefix,
                StringComparison.Ordinal) ||
            !string.Equals(
                state.AutomationName,
                normalizedName,
                StringComparison.Ordinal))
        {
            var automationId = CreateAutomationId(
                automationPrefix,
                normalizedName);

            if (!string.Equals(
                    state.AutomationId,
                    automationId,
                    StringComparison.Ordinal))
            {
                AutomationProperties.SetAutomationId(element, automationId);
                state.AutomationId = automationId;
            }

            state.AutomationPrefix = automationPrefix;
            state.AutomationName = normalizedName;
        }

        if (!state.AccessibilityViewApplied)
        {
            AutomationProperties.SetAccessibilityView(
                element,
                AccessibilityView.Control);
            state.AccessibilityViewApplied = true;
        }
    }

    public static void SetLiveRegion(
        StyledElement element,
        AutomationLiveSetting setting)
    {
        ArgumentNullException.ThrowIfNull(element);

        var state = States.GetOrCreateValue(element);
        if (state.LiveSettingApplied &&
            state.LiveSetting == setting)
        {
            return;
        }

        AutomationProperties.SetLiveSetting(element, setting);
        state.LiveSetting = setting;
        state.LiveSettingApplied = true;
    }

    private static string NormalizeName(
        string? value,
        string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return fallback;
        }

        return NeedsTrimming(value)
            ? value.Trim()
            : value;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return NeedsTrimming(value)
            ? value.Trim()
            : value;
    }

    private static bool NeedsTrimming(string value) =>
        value.Length > 0 &&
        (char.IsWhiteSpace(value[0]) ||
         char.IsWhiteSpace(value[^1]));

    private static string CreateAutomationId(
        string prefix,
        string name)
    {
        var builder = new StringBuilder(prefix.Length + name.Length + 1);
        builder.Append(prefix);
        builder.Append('.');

        foreach (var character in name)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(character);
            }
            else if (builder.Length > 0 &&
                     builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        return builder
            .ToString()
            .TrimEnd('-');
    }

    private sealed class MetadataState
    {
        public string? Name { get; set; }

        public string? HelpText { get; set; }

        public string? AutomationPrefix { get; set; }

        public string? AutomationName { get; set; }

        public string? AutomationId { get; set; }

        public bool AccessibilityViewApplied { get; set; }

        public bool LiveSettingApplied { get; set; }

        public AutomationLiveSetting LiveSetting { get; set; }
    }
}
