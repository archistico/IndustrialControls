using System.Reflection;
using System.Threading;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Media;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M8ReactivePropertyContractTests
{
    [Fact]
    public void Oscilloscope_DirectCapacityChangeTrimsExistingSamples()
    {
        var scope = new OscilloscopeDisplay
        {
            MaxSamples = 20
        };

        for (var index = 0; index < 20; index++)
        {
            scope.AddSample(index);
        }

        scope.SetValue(
            OscilloscopeDisplay.MaxSamplesProperty,
            16);

        Assert.Equal(16, scope.SampleCount);
        Assert.Equal(4.0, scope.Samples[0], 10);
        Assert.Equal(19.0, scope.LastValue, 10);
    }

    [Fact]
    public void Selector_DirectPositionCountChangeNormalizesPosition()
    {
        var selector = new SelectorSwitch
        {
            PositionCount = 5,
            PositionLabels = "OFF|LOCAL|AUTO|REMOTE|TEST",
            Position = 4
        };

        selector.SetValue(
            SelectorSwitch.PositionCountProperty,
            3);

        Assert.Equal(3, selector.PositionCount);
        Assert.Equal(2, selector.Position);
        Assert.Equal("AUTO", selector.SelectedLabel);
    }

    [Fact]
    public void Selector_DirectOutOfRangePositionIsNormalized()
    {
        var selector = new SelectorSwitch
        {
            PositionCount = 3
        };

        selector.SetValue(
            SelectorSwitch.PositionProperty,
            4);

        Assert.Equal(2, selector.Position);
    }

    [Fact]
    public void RotaryKnob_DirectMaximumChangeCoercesValue()
    {
        var knob = new RotaryKnob
        {
            Minimum = 0,
            Maximum = 100,
            Value = 80,
            Unit = "%",
            DecimalPlaces = 1
        };

        knob.SetValue(
            RotaryKnob.MaximumProperty,
            50.0);

        Assert.Equal(50.0, knob.Value, 10);
        Assert.Equal(135.0, knob.IndicatorAngle, 10);
        Assert.Equal("50.0 %", knob.FormattedValue);
    }

    [Fact]
    public void RotaryKnob_DirectValueChangeIsCoerced()
    {
        var knob = new RotaryKnob
        {
            Minimum = 0,
            Maximum = 100
        };

        knob.SetValue(
            RotaryKnob.ValueProperty,
            250.0);

        Assert.Equal(100.0, knob.Value, 10);
    }

    [Fact]
    public void TrendCursorReadout_InvalidatesWhenTimeWindowChanges()
    {
        var trend = new TrendChart
        {
            TimeWindowSeconds = 20,
            CursorFraction = 0.5
        };

        var series = trend.AddSeries(
            "POWER",
            "MWe",
            Colors.Green);

        trend.AddSample(series, 80, 4.0);
        trend.AddSample(series, 90, 5.0);
        trend.AddSample(series, 100, 6.0);

        Assert.Contains(
            "POWER: 5 MWe",
            trend.CursorReadout);

        trend.TimeWindowSeconds = 40;

        Assert.Contains(
            "POWER: 4 MWe",
            trend.CursorReadout);
    }

    [Fact]
    public void AlarmPanelCountersRaiseObservablePropertyChanges()
    {
        var panel = new AlarmIndicatorPanel();
        var indicator = new BacklitAlarmIndicator
        {
            AlarmId = "A"
        };

        var changedProperties =
            new HashSet<AvaloniaProperty>();

        panel.PropertyChanged += (_, change) =>
            changedProperties.Add(
                change.Property);

        panel.Indicators.Add(indicator);
        indicator.Activate();

        Assert.Equal(1, panel.ActiveConditionCount);
        Assert.Equal(1, panel.LatchedAlarmCount);
        Assert.Equal(1, panel.UnacknowledgedCount);

        Assert.Contains(
            AlarmIndicatorPanel.ActiveConditionCountProperty,
            changedProperties);
        Assert.Contains(
            AlarmIndicatorPanel.LatchedAlarmCountProperty,
            changedProperties);
        Assert.Contains(
            AlarmIndicatorPanel.UnacknowledgedCountProperty,
            changedProperties);

        indicator.Acknowledge();

        Assert.Equal(0, panel.UnacknowledgedCount);

        indicator.ClearCondition();

        Assert.Equal(0, panel.ActiveConditionCount);
        Assert.Equal(1, panel.LatchedAlarmCount);

        indicator.Reset();

        Assert.Equal(0, panel.LatchedAlarmCount);
    }

    [Fact]
    public void AlarmPanelUnsubscribesRemovedIndicators()
    {
        var panel = new AlarmIndicatorPanel();
        var indicator = new BacklitAlarmIndicator();

        panel.Indicators.Add(indicator);
        panel.Indicators.Remove(indicator);

        indicator.Activate();

        Assert.Equal(0, panel.ActiveConditionCount);
        Assert.Equal(0, panel.LatchedAlarmCount);
        Assert.Equal(0, panel.UnacknowledgedCount);
    }

    [Fact]
    public void BacklitAlarmTextChangeDoesNotRestartBlinkPhase()
    {
        var indicator = new BacklitAlarmIndicator();

        indicator.Activate();

        var tickMethod = typeof(BacklitAlarmIndicator)
            .GetMethod(
                "OnBlinkTimerTick",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        Assert.NotNull(tickMethod);

        tickMethod.Invoke(
            indicator,
            new object?[]
            {
                null,
                EventArgs.Empty
            });

        Assert.Equal(
            0.24,
            indicator.EffectiveOpacity,
            3);

        indicator.SecondaryText =
            "UPDATED WITHOUT STATE CHANGE";

        Assert.Equal(
            0.24,
            indicator.EffectiveOpacity,
            3);
        Assert.True(indicator.ShouldFlash);
    }

    [Fact]
    public void AutomationMetadataUpdatesImmediatelyWithoutSynchronizationContext()
    {
        var previousContext =
            SynchronizationContext.Current;

        SynchronizationContext.SetSynchronizationContext(
            null);

        try
        {
            var gauge = new DigitalGauge
            {
                Title = "Power",
                Unit = "%",
                DecimalPlaces = 1
            };

            gauge.Value = 42;

            var gaugeHelp =
                AutomationProperties.GetHelpText(gauge) ??
                string.Empty;

            Assert.Contains(
                "42.0 %",
                gaugeHelp);

            var selector = new SelectorSwitch
            {
                Title = "Mode",
                PositionCount = 3,
                PositionLabels = "OFF|AUTO|MANUAL"
            };

            selector.Position = 2;

            var selectorHelp =
                AutomationProperties.GetHelpText(selector) ??
                string.Empty;

            Assert.Contains(
                "Selected MANUAL",
                selectorHelp);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(
                previousContext);
        }
    }

    [Fact]
    public void ValidationScriptsUseProjectOptionAndFailFast()
    {
        var powerShell =
            ReadAsset("validate.ps1");

        var command =
            ReadAsset("validate.cmd");

        Assert.Contains(
            "'--project'",
            powerShell);
        Assert.Contains(
            "$LASTEXITCODE -ne 0",
            powerShell);
        Assert.Contains(
            "dotnet test --project",
            command);
        Assert.DoesNotContain(
            "dotnet test tests\\",
            command);
        Assert.DoesNotContain(
            "M8 RC1 VALIDATION PASSED",
            powerShell);
        Assert.DoesNotContain(
            "M8 RC1 VALIDATION PASSED",
            command);
    }

    private static string ReadAsset(string fileName)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            fileName);

        Assert.True(
            File.Exists(path),
            $"Missing test asset: {path}");

        return File.ReadAllText(path);
    }
}
