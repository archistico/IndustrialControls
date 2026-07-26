using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M2ControlContractTests
{
    [Fact]
    public void IndustrialLamp_HasStableDefaults()
    {
        var lamp = new IndustrialLamp();

        Assert.False(lamp.IsOn);
        Assert.Equal(IndustrialLampColor.Red, lamp.LampColor);
        Assert.Equal(IndustrialLampShape.Round, lamp.Shape);
        Assert.Equal(IndustrialLampState.Normal, lamp.State);
        Assert.Equal(0.85, lamp.GlowIntensity);
        Assert.True(lamp.ShowLabel);
    }

    [Theory]
    [InlineData(IndustrialLampColor.Red)]
    [InlineData(IndustrialLampColor.Amber)]
    [InlineData(IndustrialLampColor.Yellow)]
    [InlineData(IndustrialLampColor.Green)]
    [InlineData(IndustrialLampColor.Blue)]
    [InlineData(IndustrialLampColor.White)]
    public void IndustrialLamp_SupportsEveryFunctionalColor(IndustrialLampColor color)
    {
        var lamp = new IndustrialLamp
        {
            LampColor = color,
            IsOn = true
        };

        Assert.Equal(color, lamp.LampColor);
        Assert.True(lamp.IsOn);
        Assert.NotNull(lamp.ActiveBrush);
        Assert.NotNull(lamp.InactiveBrush);
        Assert.True(lamp.EffectiveOpacity >= 0.45);
    }

    [Fact]
    public void IndustrialLamp_RejectsGlowOutsideSupportedRange()
    {
        var lamp = new IndustrialLamp();

        Assert.Throws<ArgumentException>(() => lamp.GlowIntensity = 1.1);
        Assert.Throws<ArgumentException>(() => lamp.GlowIntensity = -0.1);
    }

    [Fact]
    public void IlluminatedPushButton_HasIndependentLampAndMechanicalState()
    {
        var button = new IlluminatedPushButton
        {
            IsLampOn = true,
            IsLatched = false,
            LampColor = IndustrialLampColor.Green
        };

        Assert.True(button.IsLampOn);
        Assert.False(button.IsLatched);
        Assert.Equal(IndustrialLampColor.Green, button.LampColor);
    }

    [Fact]
    public void IlluminatedPushButton_HasStableDefaults()
    {
        var button = new IlluminatedPushButton();

        Assert.False(button.IsLampOn);
        Assert.False(button.IsLatched);
        Assert.Equal(IndustrialLampColor.Green, button.LampColor);
        Assert.Equal(IndustrialLampState.Normal, button.LampState);
        Assert.Equal(IlluminatedPushButtonMode.Momentary, button.ActionMode);
        Assert.Null(button.SecondaryCaption);
    }
}
