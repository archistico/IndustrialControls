using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M3ControlContractTests
{
    [Fact]
    public void LedMatrixDisplay_HasStableDefaults()
    {
        var control = new LedMatrixDisplay();

        Assert.Equal(string.Empty, control.Text);
        Assert.Equal(LedDisplayColor.Red, control.LedColor);
        Assert.Equal(LedMatrixSize.Font5x7, control.MatrixSize);
        Assert.Equal(0.9, control.Brightness);
        Assert.Equal(1, control.CharacterSpacing);
    }

    [Fact]
    public void LedMatrixDisplay_RejectsBrightnessOutsideRange()
    {
        var control = new LedMatrixDisplay();

        Assert.Throws<ArgumentException>(() => control.Brightness = -0.1);
        Assert.Throws<ArgumentException>(() => control.Brightness = 1.1);
    }

    [Fact]
    public void SevenSegmentDisplay_FormatsEngineeringValue()
    {
        var control = new SevenSegmentDisplay
        {
            Value = 5.25,
            DecimalPlaces = 2,
            Unit = "MWe"
        };

        Assert.Equal("5.25 MWe", control.Text);
    }

    [Fact]
    public void AlarmAnnunciator_ActivationRequiresAcknowledgement()
    {
        var alarm = new AlarmAnnunciator();

        alarm.Activate();

        Assert.True(alarm.IsActive);
        Assert.False(alarm.IsAcknowledged);
        Assert.True(alarm.ShouldFlash);

        alarm.Acknowledge();

        Assert.True(alarm.IsAcknowledged);
        Assert.False(alarm.ShouldFlash);
    }

    [Fact]
    public void AlarmAnnunciator_ResetOnlyClearsMemoryWhenInactive()
    {
        var alarm = new AlarmAnnunciator { IsLatched = true };

        alarm.Activate();
        alarm.Acknowledge();
        alarm.Clear();
        alarm.Reset();

        Assert.False(alarm.IsActive);
        Assert.False(alarm.IsAcknowledged);
    }

    [Fact]
    public void AlarmAnnunciatorPanel_HasStableColumnDefault()
    {
        var panel = new AlarmAnnunciatorPanel();

        Assert.Equal(2, panel.Columns);
    }
}
