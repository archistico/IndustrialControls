using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class ControlContractTests
{
    [Fact]
    public void IndustrialPanel_HasStableDefaults()
    {
        var control = new IndustrialPanel();

        Assert.True(control.ShowFasteners);
        Assert.Equal(IndustrialPanelDepth.Raised, control.Depth);
        Assert.Null(control.Subtitle);
    }

    [Fact]
    public void InstrumentBezel_HasStableDefaults()
    {
        var control = new InstrumentBezel();

        Assert.True(control.ShowGlass);
        Assert.Equal(InstrumentBezelShape.Rectangular, control.Shape);
        Assert.Null(control.Title);
        Assert.Null(control.Unit);
    }

    [Fact]
    public void EngravedLabel_HasStableDefaults()
    {
        var control = new EngravedLabel();

        Assert.Equal(string.Empty, control.Text);
        Assert.Equal(EngravedLabelVariant.Black, control.Variant);
        Assert.False(control.ShowFasteners);
    }

    [Fact]
    public void StyledProperties_RoundTripValues()
    {
        var panel = new IndustrialPanel
        {
            Subtitle = "UNITÀ 2",
            ShowFasteners = false,
            Depth = IndustrialPanelDepth.Recessed
        };

        var bezel = new InstrumentBezel
        {
            Title = "PRESSIONE",
            Unit = "bar",
            Shape = InstrumentBezelShape.Circular,
            ShowGlass = false
        };

        var label = new EngravedLabel
        {
            Text = "ARRESTO",
            Variant = EngravedLabelVariant.Red,
            ShowFasteners = true
        };

        Assert.Equal("UNITÀ 2", panel.Subtitle);
        Assert.False(panel.ShowFasteners);
        Assert.Equal(IndustrialPanelDepth.Recessed, panel.Depth);

        Assert.Equal("PRESSIONE", bezel.Title);
        Assert.Equal("bar", bezel.Unit);
        Assert.Equal(InstrumentBezelShape.Circular, bezel.Shape);
        Assert.False(bezel.ShowGlass);

        Assert.Equal("ARRESTO", label.Text);
        Assert.Equal(EngravedLabelVariant.Red, label.Variant);
        Assert.True(label.ShowFasteners);
    }
}
