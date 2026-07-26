using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

public sealed class RadialGaugeDial : Control
{
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<RadialGaugeDial, double>(nameof(Minimum));

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<RadialGaugeDial, double>(nameof(Maximum), 100.0);

    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<RadialGaugeDial, double>(nameof(Value));

    public static readonly StyledProperty<double> StartAngleProperty =
        AvaloniaProperty.Register<RadialGaugeDial, double>(nameof(StartAngle), -135.0);

    public static readonly StyledProperty<double> SweepAngleProperty =
        AvaloniaProperty.Register<RadialGaugeDial, double>(nameof(SweepAngle), 270.0);

    public static readonly StyledProperty<int> MajorTickCountProperty =
        AvaloniaProperty.Register<RadialGaugeDial, int>(nameof(MajorTickCount), 11);

    public static readonly StyledProperty<int> MinorTicksPerIntervalProperty =
        AvaloniaProperty.Register<RadialGaugeDial, int>(nameof(MinorTicksPerInterval), 4);

    public static readonly StyledProperty<int> ScaleDecimalPlacesProperty =
        AvaloniaProperty.Register<RadialGaugeDial, int>(nameof(ScaleDecimalPlaces));

    public static readonly StyledProperty<bool> ShowScaleLabelsProperty =
        AvaloniaProperty.Register<RadialGaugeDial, bool>(nameof(ShowScaleLabels), true);

    public static readonly StyledProperty<bool> ShowOperatingBandsProperty =
        AvaloniaProperty.Register<RadialGaugeDial, bool>(nameof(ShowOperatingBands), true);

    public static readonly StyledProperty<double> CautionLowProperty =
        AvaloniaProperty.Register<RadialGaugeDial, double>(nameof(CautionLow), double.NaN);

    public static readonly StyledProperty<double> CautionHighProperty =
        AvaloniaProperty.Register<RadialGaugeDial, double>(nameof(CautionHigh), double.NaN);

    public static readonly StyledProperty<double> WarningLowProperty =
        AvaloniaProperty.Register<RadialGaugeDial, double>(nameof(WarningLow), double.NaN);

    public static readonly StyledProperty<double> WarningHighProperty =
        AvaloniaProperty.Register<RadialGaugeDial, double>(nameof(WarningHigh), double.NaN);

    public static readonly StyledProperty<IBrush?> NeedleBrushProperty =
        AvaloniaProperty.Register<RadialGaugeDial, IBrush?>(nameof(NeedleBrush));

    static RadialGaugeDial()
    {
        AffectsRender<RadialGaugeDial>(
            MinimumProperty,
            MaximumProperty,
            ValueProperty,
            StartAngleProperty,
            SweepAngleProperty,
            MajorTickCountProperty,
            MinorTicksPerIntervalProperty,
            ScaleDecimalPlacesProperty,
            ShowScaleLabelsProperty,
            ShowOperatingBandsProperty,
            CautionLowProperty,
            CautionHighProperty,
            WarningLowProperty,
            WarningHighProperty,
            NeedleBrushProperty);
    }

    public double Minimum { get => GetValue(MinimumProperty); set => SetValue(MinimumProperty, value); }
    public double Maximum { get => GetValue(MaximumProperty); set => SetValue(MaximumProperty, value); }
    public double Value { get => GetValue(ValueProperty); set => SetValue(ValueProperty, value); }
    public double StartAngle { get => GetValue(StartAngleProperty); set => SetValue(StartAngleProperty, value); }
    public double SweepAngle { get => GetValue(SweepAngleProperty); set => SetValue(SweepAngleProperty, value); }
    public int MajorTickCount { get => GetValue(MajorTickCountProperty); set => SetValue(MajorTickCountProperty, value); }
    public int MinorTicksPerInterval { get => GetValue(MinorTicksPerIntervalProperty); set => SetValue(MinorTicksPerIntervalProperty, value); }
    public int ScaleDecimalPlaces { get => GetValue(ScaleDecimalPlacesProperty); set => SetValue(ScaleDecimalPlacesProperty, value); }
    public bool ShowScaleLabels { get => GetValue(ShowScaleLabelsProperty); set => SetValue(ShowScaleLabelsProperty, value); }
    public bool ShowOperatingBands { get => GetValue(ShowOperatingBandsProperty); set => SetValue(ShowOperatingBandsProperty, value); }
    public double CautionLow { get => GetValue(CautionLowProperty); set => SetValue(CautionLowProperty, value); }
    public double CautionHigh { get => GetValue(CautionHighProperty); set => SetValue(CautionHighProperty, value); }
    public double WarningLow { get => GetValue(WarningLowProperty); set => SetValue(WarningLowProperty, value); }
    public double WarningHigh { get => GetValue(WarningHighProperty); set => SetValue(WarningHighProperty, value); }
    public IBrush? NeedleBrush { get => GetValue(NeedleBrushProperty); set => SetValue(NeedleBrushProperty, value); }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var size = Math.Min(Bounds.Width, Bounds.Height);
        if (size <= 8)
        {
            return;
        }

        var center = new Point(Bounds.Width / 2.0, Bounds.Height / 2.0);
        var outerRadius = (size / 2.0) - 5.0;
        var faceRadius = outerRadius - 7.0;
        var bandRadius = faceRadius - 12.0;
        var majorOuterRadius = bandRadius - 8.0;
        var majorInnerRadius = majorOuterRadius - 13.0;
        var minorInnerRadius = majorOuterRadius - 7.0;
        var labelRadius = majorInnerRadius - 17.0;
        var needleRadius = majorInnerRadius - 8.0;

        var darkPen = new Pen(new SolidColorBrush(Color.Parse("#090B0C")), 5);
        var edgePen = new Pen(new SolidColorBrush(Color.Parse("#6B7174")), 1);
        var scalePen = new Pen(new SolidColorBrush(Color.Parse("#D7D9D0")), 2);
        var minorPen = new Pen(new SolidColorBrush(Color.Parse("#92989A")), 1);
        var textBrush = new SolidColorBrush(Color.Parse("#E7E8DF"));
        var faceBrush = new SolidColorBrush(Color.Parse("#171A1B"));
        var hubBrush = new SolidColorBrush(Color.Parse("#AEB3B5"));
        var hubEdgePen = new Pen(new SolidColorBrush(Color.Parse("#111315")), 3);

        context.DrawEllipse(faceBrush, darkPen, center, outerRadius, outerRadius);
        context.DrawEllipse(null, edgePen, center, faceRadius, faceRadius);

        if (ShowOperatingBands)
        {
            DrawOperatingBands(context, center, bandRadius);
        }

        DrawTicksAndLabels(
            context,
            center,
            majorOuterRadius,
            majorInnerRadius,
            minorInnerRadius,
            labelRadius,
            scalePen,
            minorPen,
            textBrush);

        DrawNeedle(context, center, needleRadius);
        context.DrawEllipse(hubBrush, hubEdgePen, center, 9, 9);
        context.DrawEllipse(new SolidColorBrush(Color.Parse("#51575A")), null, center, 3, 3);
    }

    private void DrawOperatingBands(DrawingContext context, Point center, double radius)
    {
        var green = new SolidColorBrush(Color.Parse("#3AA655"));
        var yellow = new SolidColorBrush(Color.Parse("#E3C83B"));
        var red = new SolidColorBrush(Color.Parse("#D94A45"));

        var warningLow = NormalizeThreshold(WarningLow, Minimum);
        var cautionLow = NormalizeThreshold(CautionLow, warningLow);
        var cautionHigh = NormalizeThreshold(CautionHigh, Maximum);
        var warningHigh = NormalizeThreshold(WarningHigh, cautionHigh);

        warningLow = Math.Clamp(warningLow, Minimum, Maximum);
        cautionLow = Math.Clamp(cautionLow, warningLow, Maximum);
        cautionHigh = Math.Clamp(cautionHigh, cautionLow, Maximum);
        warningHigh = Math.Clamp(warningHigh, cautionHigh, Maximum);

        DrawBand(context, center, radius, Minimum, warningLow, red);
        DrawBand(context, center, radius, warningLow, cautionLow, yellow);
        DrawBand(context, center, radius, cautionLow, cautionHigh, green);
        DrawBand(context, center, radius, cautionHigh, warningHigh, yellow);
        DrawBand(context, center, radius, warningHigh, Maximum, red);
    }

    private double NormalizeThreshold(double threshold, double fallback) =>
        double.IsNaN(threshold) ? fallback : threshold;

    private void DrawBand(
        DrawingContext context,
        Point center,
        double radius,
        double fromValue,
        double toValue,
        IBrush brush)
    {
        if (toValue <= fromValue || Maximum <= Minimum)
        {
            return;
        }

        var fromAngle = AngleForValue(fromValue);
        var toAngle = AngleForValue(toValue);
        var steps = Math.Max(2, (int)Math.Ceiling(Math.Abs(toAngle - fromAngle) / 2.0));
        var pen = new Pen(brush, 9);

        for (var index = 0; index < steps; index++)
        {
            var angle1 = fromAngle + ((toAngle - fromAngle) * index / steps);
            var angle2 = fromAngle + ((toAngle - fromAngle) * (index + 1) / steps);
            context.DrawLine(pen, Polar(center, radius, angle1), Polar(center, radius, angle2));
        }
    }

    private void DrawTicksAndLabels(
        DrawingContext context,
        Point center,
        double majorOuterRadius,
        double majorInnerRadius,
        double minorInnerRadius,
        double labelRadius,
        Pen majorPen,
        Pen minorPen,
        IBrush textBrush)
    {
        var majorCount = Math.Max(2, MajorTickCount);
        var intervals = majorCount - 1;
        var minorCount = Math.Max(0, MinorTicksPerInterval);
        var totalSubdivisions = intervals * (minorCount + 1);

        for (var subdivision = 0; subdivision <= totalSubdivisions; subdivision++)
        {
            var fraction = totalSubdivisions == 0 ? 0.0 : subdivision / (double)totalSubdivisions;
            var angle = StartAngle + (SweepAngle * fraction);
            var isMajor = subdivision % (minorCount + 1) == 0;
            var innerRadius = isMajor ? majorInnerRadius : minorInnerRadius;

            context.DrawLine(
                isMajor ? majorPen : minorPen,
                Polar(center, innerRadius, angle),
                Polar(center, majorOuterRadius, angle));

            if (!isMajor || !ShowScaleLabels)
            {
                continue;
            }

            var value = Minimum + ((Maximum - Minimum) * fraction);
            var text = value.ToString(
                "F" + ScaleDecimalPlaces.ToString(CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture);

            var formatted = new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                Typeface.Default,
                10,
                textBrush);

            var position = Polar(center, labelRadius, angle);
            context.DrawText(
                formatted,
                new Point(position.X - (formatted.Width / 2.0), position.Y - (formatted.Height / 2.0)));
        }
    }

    private void DrawNeedle(DrawingContext context, Point center, double radius)
    {
        var angle = AngleForValue(Value);
        var tip = Polar(center, radius, angle);
        var tail = Polar(center, 15, angle + 180.0);
        var brush = NeedleBrush ?? new SolidColorBrush(Color.Parse("#F14C4C"));
        var shadowPen = new Pen(new SolidColorBrush(Color.Parse("#080909")), 6);
        var needlePen = new Pen(brush, 3);

        context.DrawLine(shadowPen, tail, tip);
        context.DrawLine(needlePen, tail, tip);
    }

    private double AngleForValue(double value)
    {
        if (Maximum <= Minimum)
        {
            return StartAngle;
        }

        var normalized = Math.Clamp((value - Minimum) / (Maximum - Minimum), 0.0, 1.0);
        return StartAngle + (normalized * SweepAngle);
    }

    private static Point Polar(Point center, double radius, double angleDegrees)
    {
        var radians = (angleDegrees - 90.0) * Math.PI / 180.0;
        return new Point(
            center.X + (Math.Cos(radians) * radius),
            center.Y + (Math.Sin(radians) * radius));
    }
}
