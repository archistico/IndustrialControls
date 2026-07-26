using Avalonia.Media;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M7AlarmAndStaticPanelContractTests
{
    [Fact]
    public void BacklitAlarmIndicator_HasStableDefaults()
    {
        var indicator = new BacklitAlarmIndicator();

        Assert.False(indicator.IsConditionActive);
        Assert.False(indicator.IsAcknowledged);
        Assert.True(indicator.IsLatched);
        Assert.False(indicator.HasLatchedAlarm);
        Assert.Equal(AlarmIndicatorVisualState.Clear, indicator.VisualState);
    }

    [Fact]
    public void BacklitAlarmIndicator_ClearStateUsesReadableForeground()
    {
        var indicator = new BacklitAlarmIndicator();

        var brush = Assert.IsType<SolidColorBrush>(indicator.DisplayForegroundBrush);

        Assert.Equal(Color.Parse("#FFD9D2BC"), brush.Color);
        Assert.Equal(0.18, indicator.EffectiveOpacity, 3);
    }

    [Fact]
    public void BacklitAlarmIndicator_ActivationCreatesNewAlarm()
    {
        var indicator = new BacklitAlarmIndicator();

        indicator.Activate();

        Assert.True(indicator.IsConditionActive);
        Assert.True(indicator.HasLatchedAlarm);
        Assert.False(indicator.IsAcknowledged);
        Assert.True(indicator.ShouldFlash);
        Assert.Equal(AlarmIndicatorVisualState.NewAlarm, indicator.VisualState);
    }

    [Fact]
    public void BacklitAlarmIndicator_AcknowledgeCreatesSteadyActiveState()
    {
        var indicator = new BacklitAlarmIndicator();

        indicator.Activate();

        Assert.True(indicator.Acknowledge());
        Assert.False(indicator.ShouldFlash);
        Assert.Equal(
            AlarmIndicatorVisualState.AcknowledgedActive,
            indicator.VisualState);
    }

    [Fact]
    public void BacklitAlarmIndicator_LatchedReturnRequiresReset()
    {
        var indicator = new BacklitAlarmIndicator
        {
            IsLatched = true
        };

        indicator.Activate();
        indicator.Acknowledge();
        indicator.ClearCondition();

        Assert.False(indicator.IsConditionActive);
        Assert.True(indicator.HasLatchedAlarm);
        Assert.Equal(
            AlarmIndicatorVisualState.ReadyToReset,
            indicator.VisualState);

        Assert.True(indicator.Reset());
        Assert.False(indicator.HasLatchedAlarm);
        Assert.Equal(AlarmIndicatorVisualState.Clear, indicator.VisualState);
    }

    [Fact]
    public void BacklitAlarmIndicator_ResetRequiresAcknowledgement()
    {
        var indicator = new BacklitAlarmIndicator();

        indicator.Activate();
        indicator.ClearCondition();

        Assert.False(indicator.Reset());
        Assert.True(indicator.HasLatchedAlarm);
        Assert.Equal(
            AlarmIndicatorVisualState.ReturnedUnacknowledged,
            indicator.VisualState);
    }

    [Fact]
    public void BacklitAlarmIndicator_NonLatchedAlarmClearsOnReturn()
    {
        var indicator = new BacklitAlarmIndicator
        {
            IsLatched = false
        };

        indicator.Activate();
        indicator.ClearCondition();

        Assert.False(indicator.HasLatchedAlarm);
        Assert.False(indicator.IsAcknowledged);
        Assert.Equal(AlarmIndicatorVisualState.Clear, indicator.VisualState);
    }

    [Fact]
    public void AlarmIndicatorPanel_AcknowledgesAndCountsIndicators()
    {
        var panel = new AlarmIndicatorPanel();
        var first = new BacklitAlarmIndicator { AlarmId = "A" };
        var second = new BacklitAlarmIndicator { AlarmId = "B" };

        panel.Indicators.Add(first);
        panel.Indicators.Add(second);

        Assert.True(panel.Activate("A"));
        Assert.True(panel.Activate("B"));
        Assert.Equal(2, panel.ActiveConditionCount);
        Assert.Equal(2, panel.UnacknowledgedCount);

        Assert.Equal(2, panel.AcknowledgeAll());
        Assert.Equal(0, panel.UnacknowledgedCount);
    }

    [Fact]
    public void AlarmIndicatorPanel_ClearsAndResetsLatchedIndicators()
    {
        var panel = new AlarmIndicatorPanel();
        var indicator = new BacklitAlarmIndicator
        {
            AlarmId = "A"
        };

        panel.Indicators.Add(indicator);
        panel.Activate("A");
        panel.AcknowledgeAll();

        Assert.Equal(1, panel.ClearAllConditions());
        Assert.Equal(1, panel.LatchedAlarmCount);
        Assert.Equal(1, panel.ResetAll());
        Assert.Equal(0, panel.LatchedAlarmCount);
    }

    [Fact]
    public void AlarmIndicatorPanel_RejectsUnknownAlarmId()
    {
        var panel = new AlarmIndicatorPanel();

        Assert.False(panel.Activate("UNKNOWN"));
    }

    [Theory]
    [InlineData(SafetyPlacardLevel.Information, "#FF2878A8")]
    [InlineData(SafetyPlacardLevel.Notice, "#FF3A8A4E")]
    [InlineData(SafetyPlacardLevel.Caution, "#FFF2DD4B")]
    [InlineData(SafetyPlacardLevel.Warning, "#FFE67E22")]
    [InlineData(SafetyPlacardLevel.Danger, "#FFB91F28")]
    public void SafetyPlacard_MapsLevelToHeaderColor(
        SafetyPlacardLevel level,
        string expectedColor)
    {
        var placard = new SafetyPlacard
        {
            Level = level
        };

        var brush = Assert.IsType<SolidColorBrush>(placard.HeaderBrush);

        Assert.Equal(Color.Parse(expectedColor), brush.Color);
    }

    [Theory]
    [InlineData(SafetyPlacardIcon.Information, "i")]
    [InlineData(SafetyPlacardIcon.Warning, "!")]
    [InlineData(SafetyPlacardIcon.ElectricalHazard, "⚡")]
    [InlineData(SafetyPlacardIcon.Radiation, "☢")]
    [InlineData(SafetyPlacardIcon.HotSurface, "♨")]
    [InlineData(SafetyPlacardIcon.Mandatory, "●")]
    public void SafetyPlacard_MapsIconToGlyph(
        SafetyPlacardIcon icon,
        string expectedGlyph)
    {
        var placard = new SafetyPlacard
        {
            Icon = icon
        };

        Assert.Equal(expectedGlyph, placard.IconGlyph);
    }

    [Theory]
    [InlineData(DataPlateMaterial.Aluminum, "#FFA8ADB0")]
    [InlineData(DataPlateMaterial.Brass, "#FFB9A45B")]
    [InlineData(DataPlateMaterial.Black, "#FF181A1B")]
    [InlineData(DataPlateMaterial.Red, "#FF741D20")]
    public void BoltedDataPlate_MapsMaterialToPlateColor(
        DataPlateMaterial material,
        string expectedColor)
    {
        var plate = new BoltedDataPlate
        {
            Material = material
        };

        var brush = Assert.IsType<SolidColorBrush>(plate.PlateBrush);

        Assert.Equal(Color.Parse(expectedColor), brush.Color);
    }

    [Fact]
    public void BoltedDataPlate_HasFastenersByDefault()
    {
        var plate = new BoltedDataPlate();

        Assert.True(plate.ShowFasteners);
        Assert.Equal(DataPlateMaterial.Aluminum, plate.Material);
    }
}
