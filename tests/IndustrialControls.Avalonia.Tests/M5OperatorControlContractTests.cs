using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M5OperatorControlContractTests
{
    [Fact]
    public void IndustrialSlider_FormatsEngineeringValue()
    {
        var slider = new IndustrialSlider
        {
            Value = 14.2,
            Unit = "kg/s",
            DecimalPlaces = 1
        };

        Assert.Equal("14.2 kg/s", slider.FormattedValue);
        Assert.Equal("COMMAND AVAILABLE", slider.InterlockText);
    }

    [Fact]
    public void IndustrialSlider_InterlockDisablesInput()
    {
        var slider = new IndustrialSlider
        {
            IsInterlocked = true,
            InterlockReason = "AUTO MODE"
        };

        Assert.False(slider.IsEnabled);
        Assert.Equal("INTERLOCK — AUTO MODE", slider.InterlockText);
    }

    [Fact]
    public void RotaryKnob_ClampsAndIncrementsValue()
    {
        var knob = new RotaryKnob
        {
            Minimum = 0,
            Maximum = 10,
            Value = 9.9,
            SmallChange = 0.25
        };

        Assert.True(knob.Increase());
        Assert.Equal(10.0, knob.Value, 10);

        knob.Value = -1;
        Assert.Equal(0.0, knob.Value, 10);
    }

    [Fact]
    public void RotaryKnob_InterlockRejectsCommand()
    {
        var knob = new RotaryKnob
        {
            Value = 5,
            SmallChange = 1,
            IsInterlocked = true
        };

        Assert.False(knob.Increase());
        Assert.Equal(5.0, knob.Value, 10);
    }

    [Fact]
    public void RotaryKnob_ComputesCentralIndicatorAngle()
    {
        var knob = new RotaryKnob
        {
            Minimum = 0,
            Maximum = 100,
            Value = 50
        };

        Assert.Equal(0.0, knob.IndicatorAngle, 10);
    }

    [Fact]
    public void SelectorSwitch_UpdatesSelectedLabel()
    {
        var selector = new SelectorSwitch
        {
            PositionCount = 3,
            PositionLabels = "OFF|AUTO|MANUAL",
            Position = 1
        };

        Assert.Equal("AUTO", selector.SelectedLabel);
        Assert.True(selector.SelectNext());
        Assert.Equal(2, selector.Position);
        Assert.Equal("MANUAL", selector.SelectedLabel);
    }

    [Fact]
    public void SelectorSwitch_InterlockPreservesPosition()
    {
        var selector = new SelectorSwitch
        {
            PositionCount = 3,
            Position = 1,
            IsInterlocked = true
        };

        Assert.False(selector.SelectNext());
        Assert.Equal(1, selector.Position);
    }

    [Fact]
    public void IndustrialToggleSwitch_RespectsInterlock()
    {
        var toggle = new IndustrialToggleSwitch
        {
            IsChecked = false,
            IsInterlocked = true
        };

        Assert.False(toggle.TryToggle());
        Assert.False(toggle.IsOn);

        toggle.IsInterlocked = false;

        Assert.True(toggle.TryToggle());
        Assert.True(toggle.IsOn);
    }


    [Fact]
    public void IndustrialRockerSwitch_RespectsInterlock()
    {
        var rocker = new IndustrialRockerSwitch
        {
            IsChecked = false,
            IsInterlocked = true
        };

        Assert.False(rocker.TryToggle());
        Assert.False(rocker.IsOn);

        rocker.IsInterlocked = false;

        Assert.True(rocker.TryToggle());
        Assert.True(rocker.IsOn);
    }

    [Fact]
    public void SpringReturnSwitch_ReturnsToCenter()
    {
        var command = new SpringReturnSwitch();

        Assert.True(command.PressRight());
        Assert.Equal(SpringReturnPosition.Right, command.Position);
        Assert.Equal("RAISE", command.StateText);

        command.Release();

        Assert.Equal(SpringReturnPosition.Center, command.Position);
        Assert.Equal("HOLD", command.StateText);
    }

    [Fact]
    public void SpringReturnSwitch_InterlockRejectsMomentaryCommand()
    {
        var command = new SpringReturnSwitch
        {
            IsInterlocked = true
        };

        Assert.False(command.PressLeft());
        Assert.Equal(SpringReturnPosition.Center, command.Position);
    }

    [Theory]
    [InlineData(false, 4, 4, true, "PERMITTED")]
    [InlineData(true, 4, 4, false, "INTERLOCKED")]
    [InlineData(false, 2, 4, false, "INTERLOCKED")]
    public void InterlockIndicator_ComputesPermissiveState(
        bool isInterlocked,
        int satisfied,
        int required,
        bool expectedPermitted,
        string expectedText)
    {
        var indicator = new InterlockIndicator
        {
            IsInterlocked = isInterlocked,
            SatisfiedPermissiveCount = satisfied,
            RequiredPermissiveCount = required
        };

        Assert.Equal(expectedPermitted, indicator.IsPermitted);
        Assert.Equal(expectedText, indicator.StatusText);
        Assert.Equal($"{satisfied} / {required}", indicator.CountText);
    }
}
