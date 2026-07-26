using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Tests;

public sealed class M4GaugeContractTests
{
    [Fact]
    public void GaugeBase_NormalizesValue()
    {
        var gauge = new DigitalGauge
        {
            Minimum = 0,
            Maximum = 200,
            Value = 50
        };

        Assert.Equal(0.25, gauge.NormalizedValue, 10);
        Assert.Equal(25.0, gauge.Percentage, 10);
    }

    [Fact]
    public void GaugeBase_FormatsEngineeringValue()
    {
        var gauge = new DigitalGauge
        {
            Value = 6.85,
            DecimalPlaces = 2,
            Unit = "MPa"
        };

        Assert.Equal("6.85 MPa", gauge.FormattedValue);
    }

    [Theory]
    [InlineData(50, GaugeStatus.Normal)]
    [InlineData(75, GaugeStatus.Caution)]
    [InlineData(90, GaugeStatus.Warning)]
    [InlineData(110, GaugeStatus.OutOfRange)]
    public void GaugeBase_ComputesStatus(double value, GaugeStatus expected)
    {
        var gauge = new DigitalGauge
        {
            Minimum = 0,
            Maximum = 100,
            CautionHigh = 70,
            WarningHigh = 85,
            Value = value
        };

        Assert.Equal(expected, gauge.Status);
    }

    [Fact]
    public void GaugeBase_UnavailableOverridesValueStatus()
    {
        var gauge = new DigitalGauge
        {
            Minimum = 0,
            Maximum = 100,
            Value = 150,
            IsAvailable = false
        };

        Assert.Equal(GaugeStatus.Unavailable, gauge.Status);
    }

    [Fact]
    public void RadialGauge_ComputesNeedleAngle()
    {
        var gauge = new RadialGauge
        {
            Minimum = 0,
            Maximum = 100,
            StartAngle = -135,
            SweepAngle = 270,
            Value = 50
        };

        Assert.Equal(0.0, gauge.NeedleAngle, 10);
    }


    [Fact]
    public void RadialGauge_AnglesShareTheSameCentralScaleGeometry()
    {
        var gauge = new RadialGauge
        {
            Minimum = 0,
            Maximum = 100,
            StartAngle = -135,
            SweepAngle = 270
        };

        Assert.Equal(-135.0, gauge.GetAngleForValue(0), 10);
        Assert.Equal(0.0, gauge.GetAngleForValue(50), 10);
        Assert.Equal(135.0, gauge.GetAngleForValue(100), 10);
    }

    [Fact]
    public void RadialGauge_HasIndustrialScaleDefaults()
    {
        var gauge = new RadialGauge();

        Assert.Equal(11, gauge.MajorTickCount);
        Assert.Equal(4, gauge.MinorTicksPerInterval);
        Assert.True(gauge.ShowScaleLabels);
        Assert.True(gauge.ShowOperatingBands);
    }

    [Fact]
    public void DeviationGauge_ComputesSignedDeviation()
    {
        var gauge = new DeviationGauge
        {
            Value = 5.75,
            Setpoint = 5.0,
            DecimalPlaces = 2
        };

        Assert.Equal(0.75, gauge.Deviation, 10);
        Assert.Equal("+0.75", gauge.FormattedDeviation);
    }


    [Fact]
    public void DeviationGauge_FormattingIsCultureIndependent()
    {
        var previousCulture = System.Globalization.CultureInfo.CurrentCulture;

        try
        {
            System.Globalization.CultureInfo.CurrentCulture =
                System.Globalization.CultureInfo.GetCultureInfo("it-IT");

            var gauge = new DeviationGauge
            {
                Value = 5.75,
                Setpoint = 5.0,
                DecimalPlaces = 2
            };

            Assert.Equal("+0.75", gauge.FormattedDeviation);
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = previousCulture;
        }
    }

    [Fact]
    public void GaugeProperties_RejectInvalidCounts()
    {
        var radial = new RadialGauge();
        var digital = new DigitalGauge();

        Assert.Throws<ArgumentException>(() => radial.MajorTickCount = 1);
        Assert.Throws<ArgumentException>(() => digital.DecimalPlaces = 9);
    }
}
