using Avalonia.Media;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M8LampsAndLedVisualContractTests
{
    [Fact]
    public void Marquee_BeginsWithOneBlankViewport()
    {
        var marquee = new LedMarqueeDisplay
        {
            IsRunning = false,
            VisibleCharacters = 12,
            AutoFitVisibleCharacters = false,
            Text = "ALARM"
        };

        Assert.Equal(
            12,
            marquee.EffectiveVisibleCharacters);

        Assert.Equal(
            new string(' ', 12),
            marquee.DisplayText);
    }

    [Fact]
    public void Marquee_UsesAutomaticWidthFittingByDefault()
    {
        var marquee = new LedMarqueeDisplay();

        Assert.True(
            marquee.AutoFitVisibleCharacters);

        Assert.Equal(
            marquee.VisibleCharacters,
            marquee.EffectiveVisibleCharacters);
    }

    [Theory]
    [InlineData(AlarmPriority.Advisory, "#FF57A8E8")]
    [InlineData(AlarmPriority.Caution, "#FFF2DD4B")]
    [InlineData(AlarmPriority.Warning, "#FFFFB238")]
    [InlineData(AlarmPriority.Critical, "#FFF14C4C")]
    public void LegacyAlarmAnnunciator_UsesCleanPriorityPalette(
        AlarmPriority priority,
        string expectedColor)
    {
        var annunciator = new AlarmAnnunciator
        {
            Priority = priority
        };

        Assert.Equal(
            Color.Parse(expectedColor),
            annunciator.PriorityColor);

        Assert.NotNull(
            annunciator.PriorityBrush);
    }

    [Fact]
    public void LegacyAlarmAnnunciator_ExposesReadableStateText()
    {
        var annunciator = new AlarmAnnunciator();

        Assert.Equal(
            "CLEAR",
            annunciator.StateText);

        annunciator.Activate();

        Assert.Equal(
            "NEW ALARM",
            annunciator.StateText);

        annunciator.Acknowledge();

        Assert.Equal(
            "ACK / ACTIVE",
            annunciator.StateText);
    }

    [Fact]
    public void Theme_UsesCircularLegacyAlarmLensAndHandCursors()
    {
        var themePath = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "Industrial90.axaml");

        var theme =
            File.ReadAllText(themePath);

        Assert.Contains(
            "<Ellipse Margin=\"6\"",
            theme);

        Assert.DoesNotContain(
            "Background=\"#302116\"",
            theme);

        Assert.DoesNotContain(
            "Foreground=\"#E9D8B6\"",
            theme);

        Assert.True(
            CountOccurrences(
                theme,
                "<Setter Property=\"Cursor\" Value=\"Hand\" />") >= 6);
    }

    private static int CountOccurrences(
        string text,
        string value)
    {
        var count = 0;
        var offset = 0;

        while (true)
        {
            offset = text.IndexOf(
                value,
                offset,
                StringComparison.Ordinal);

            if (offset < 0)
            {
                return count;
            }

            count++;
            offset += value.Length;
        }
    }
}
