using System.Windows.Input;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M8FunctionalSafetyContractTests
{
    [Fact]
    public void DeviationGauge_DemoValuesUseDeviationForScaleAndStatus()
    {
        var gauge = new DeviationGauge
        {
            Minimum = -5,
            Maximum = 5,
            Value = 5.75,
            Setpoint = 5,
            Deadband = 0.2,
            Unit = "MWe",
            DecimalPlaces = 2
        };

        Assert.Equal(0.75, gauge.Deviation, 10);
        Assert.Equal(0.75, gauge.EffectiveDeviation, 10);
        Assert.Equal(0.575, gauge.NormalizedValue, 10);
        Assert.Equal(57.5, gauge.Percentage, 10);
        Assert.Equal(GaugeStatus.Normal, gauge.Status);
    }

    [Fact]
    public void DeviationGauge_DeadbandCentersTheIndicatorWithoutHidingRawDeviation()
    {
        var gauge = new DeviationGauge
        {
            Minimum = -5,
            Maximum = 5,
            Value = 5.1,
            Setpoint = 5,
            Deadband = 0.2,
            DecimalPlaces = 2
        };

        Assert.Equal(0.1, gauge.Deviation, 10);
        Assert.Equal(0.0, gauge.EffectiveDeviation, 10);
        Assert.Equal(0.5, gauge.NormalizedValue, 10);
        Assert.Equal(GaugeStatus.Normal, gauge.Status);
        Assert.Equal("+0.10", gauge.FormattedDeviation);
    }

    [Fact]
    public void DeviationGauge_ReportsOutOfRangeFromDeviation()
    {
        var gauge = new DeviationGauge
        {
            Minimum = -5,
            Maximum = 5,
            Value = 11,
            Setpoint = 5
        };

        Assert.Equal(6.0, gauge.EffectiveDeviation, 10);
        Assert.Equal(GaugeStatus.OutOfRange, gauge.Status);
        Assert.Equal(1.0, gauge.NormalizedValue, 10);
    }

    [Fact]
    public void LegacyAlarm_TransientUnacknowledgedConditionRemainsLatched()
    {
        var alarm = new AlarmAnnunciator
        {
            IsLatched = true
        };

        alarm.Activate();
        alarm.Clear();

        Assert.False(alarm.IsActive);
        Assert.True(alarm.HasLatchedAlarm);
        Assert.False(alarm.IsAcknowledged);
        Assert.True(alarm.ShouldFlash);
        Assert.Equal(
            AlarmIndicatorVisualState.ReturnedUnacknowledged,
            alarm.VisualState);
        Assert.Equal("RETURNED / ACK", alarm.StateText);
    }

    [Fact]
    public void LegacyAlarm_CanBeAcknowledgedAfterConditionReturns()
    {
        var alarm = new AlarmAnnunciator();

        alarm.Activate();
        alarm.Clear();

        Assert.True(alarm.TryAcknowledge());
        Assert.True(alarm.IsAcknowledged);
        Assert.False(alarm.ShouldFlash);
        Assert.Equal(
            AlarmIndicatorVisualState.ReadyToReset,
            alarm.VisualState);
        Assert.Equal("READY TO RESET", alarm.StateText);
    }

    [Fact]
    public void LegacyAlarm_ResetRequiresReturnedAndAcknowledgedCondition()
    {
        var alarm = new AlarmAnnunciator();

        alarm.Activate();

        Assert.False(alarm.TryReset());

        alarm.Acknowledge();

        Assert.False(alarm.TryReset());

        alarm.Clear();

        Assert.True(alarm.TryReset());
        Assert.False(alarm.HasLatchedAlarm);
        Assert.False(alarm.IsAcknowledged);
        Assert.Equal(
            AlarmIndicatorVisualState.Clear,
            alarm.VisualState);
    }

    [Fact]
    public void LegacyAlarm_NonLatchedConditionClearsImmediately()
    {
        var alarm = new AlarmAnnunciator
        {
            IsLatched = false
        };

        alarm.Activate();
        alarm.Clear();

        Assert.False(alarm.HasLatchedAlarm);
        Assert.False(alarm.IsAcknowledged);
        Assert.Equal(
            AlarmIndicatorVisualState.Clear,
            alarm.VisualState);
    }

    [Fact]
    public void SpringReturnSwitch_DirectAvaloniaInterlockReturnsToCenter()
    {
        var command = new SpringReturnSwitch();

        Assert.True(command.PressRight());
        Assert.Equal(
            SpringReturnPosition.Right,
            command.Position);

        command.SetValue(
            SpringReturnSwitch.IsInterlockedProperty,
            true);

        Assert.Equal(
            SpringReturnPosition.Center,
            command.Position);
        Assert.Equal(
            "INTERLOCK — MOMENTARY COMMAND NOT PERMITTED",
            command.StatusText);
    }

    [Fact]
    public void SpringReturnSwitch_SourceCapturesAndReleasesPointer()
    {
        var source = ReadAsset(
            "SpringReturnSwitch.cs");

        Assert.Contains(
            "e.Pointer.Capture(this);",
            source);
        Assert.Contains(
            "pointer.Capture(null);",
            source);
        Assert.Contains(
            "OnPointerCaptureLost",
            source);
    }

    [Fact]
    public void InterlockedControls_UseDirectPseudoClassCollectionMethods()
    {
        var springSource = ReadAsset(
            "SpringReturnSwitch.cs");

        var buttonSource = ReadAsset(
            "IlluminatedPushButton.cs");

        Assert.DoesNotContain(
            "PseudoClasses.Set(",
            springSource);

        Assert.DoesNotContain(
            "PseudoClasses.Set(",
            buttonSource);

        Assert.Contains(
            "PseudoClasses.Add(\":interlocked\")",
            springSource);

        Assert.Contains(
            "PseudoClasses.Remove(\":interlocked\")",
            springSource);

        Assert.Contains(
            "PseudoClasses.Add(\":interlocked\")",
            buttonSource);

        Assert.Contains(
            "PseudoClasses.Remove(\":interlocked\")",
            buttonSource);
    }

    [Fact]
    public void IlluminatedPushButton_InterlockBlocksClickAndCommand()
    {
        var command = new CountingCommand();
        var clickCount = 0;

        var button = new IlluminatedPushButton
        {
            Content = "START",
            Command = command,
            IsInterlocked = true,
            InterlockReason = "START PERMISSIVE MISSING"
        };

        button.Click += (_, _) => clickCount++;

        Assert.False(button.TryInvoke());
        Assert.Equal(0, clickCount);
        Assert.Equal(0, command.ExecutionCount);
        Assert.False(button.CanInvoke);
        Assert.Equal(
            "INTERLOCK — START PERMISSIVE MISSING",
            button.StatusText);
    }

    [Fact]
    public void IlluminatedPushButton_AcceptedToggleUsesNormalClickPath()
    {
        var command = new CountingCommand();
        var clickCount = 0;

        var button = new IlluminatedPushButton
        {
            Content = "ENABLE",
            Command = command,
            ActionMode = IlluminatedPushButtonMode.Toggle,
            IsInterlocked = false
        };

        button.Click += (_, _) => clickCount++;

        Assert.True(button.TryInvoke());
        Assert.True(button.IsLatched);
        Assert.Equal(1, clickCount);
        Assert.Equal(1, command.ExecutionCount);
        Assert.True(button.CanInvoke);
        Assert.Equal(
            "COMMAND AVAILABLE",
            button.StatusText);
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

    private sealed class CountingCommand : ICommand
    {
        public int ExecutionCount { get; private set; }

        public event EventHandler? CanExecuteChanged
        {
            add
            {
            }

            remove
            {
            }
        }

        public bool CanExecute(object? parameter) =>
            true;

        public void Execute(object? parameter) =>
            ExecutionCount++;
    }
}
