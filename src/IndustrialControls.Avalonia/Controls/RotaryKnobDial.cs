using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Renderer vettoriale interno per <see cref="RotaryKnob"/>.
/// </summary>
public sealed class RotaryKnobDial : Control
{
    public static readonly StyledProperty<double> IndicatorAngleProperty =
        AvaloniaProperty.Register<RotaryKnobDial, double>(nameof(IndicatorAngle), -135.0);

    public static readonly StyledProperty<int> TickCountProperty =
        AvaloniaProperty.Register<RotaryKnobDial, int>(nameof(TickCount), 11);

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<RotaryKnobDial, bool>(nameof(IsInterlocked));

    static RotaryKnobDial()
    {
        AffectsRender<RotaryKnobDial>(
            IndicatorAngleProperty,
            TickCountProperty,
            IsInterlockedProperty);
    }

    public double IndicatorAngle
    {
        get => GetValue(IndicatorAngleProperty);
        set => SetValue(IndicatorAngleProperty, value);
    }

    public int TickCount
    {
        get => GetValue(TickCountProperty);
        set => SetValue(TickCountProperty, value);
    }

    public bool IsInterlocked
    {
        get => GetValue(IsInterlockedProperty);
        set => SetValue(IsInterlockedProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var size = Math.Min(Bounds.Width, Bounds.Height);
        if (size <= 10)
        {
            return;
        }

        var center = new Point(Bounds.Width / 2.0, Bounds.Height / 2.0);
        var outerRadius = (size / 2.0) - 5.0;
        var indicatorRingOuter = outerRadius - 8.0;
        var indicatorRingInner = indicatorRingOuter - 10.0;
        var tickOuter = indicatorRingInner - 6.0;
        var tickInner = tickOuter - 10.0;
        var knobRadius = tickInner - 18.0;

        var outerBrush = new SolidColorBrush(Color.Parse("#111315"));
        var edgeBrush = new SolidColorBrush(Color.Parse("#85919A"));
        var knobBrush = new SolidColorBrush(
            Color.Parse(IsInterlocked ? "#313436" : "#5B6369"));
        var accentBrush = new SolidColorBrush(
            Color.Parse(IsInterlocked ? "#85898B" : "#E7E0C7"));
        var shadowBrush = new SolidColorBrush(Color.Parse("#070809"));

        context.DrawEllipse(
            outerBrush,
            new Pen(shadowBrush, 5),
            center,
            outerRadius,
            outerRadius);

        context.DrawEllipse(
            null,
            new Pen(edgeBrush, 1),
            center,
            outerRadius - 5,
            outerRadius - 5);

        DrawLevelIndicator(context, center, indicatorRingOuter, indicatorRingInner);

        var ticks = Math.Max(2, TickCount);
        for (var index = 0; index < ticks; index++)
        {
            var fraction = index / (double)(ticks - 1);
            var angle = -135.0 + (fraction * 270.0);
            var thickness = index is 0 || index == ticks - 1 ? 3 : 2;

            context.DrawLine(
                new Pen(new SolidColorBrush(Color.Parse("#81888D")), thickness),
                Polar(center, tickInner, angle),
                Polar(center, tickOuter, angle));
        }

        context.DrawEllipse(
            knobBrush,
            new Pen(shadowBrush, 4),
            center,
            knobRadius,
            knobRadius);

        context.DrawEllipse(
            null,
            new Pen(new SolidColorBrush(Color.Parse("#98A1A7")), 2),
            center,
            knobRadius - 5,
            knobRadius - 5);

        context.DrawLine(
            new Pen(shadowBrush, 10),
            Polar(center, 12, IndicatorAngle),
            Polar(center, knobRadius - 10, IndicatorAngle));

        context.DrawLine(
            new Pen(accentBrush, 6),
            Polar(center, 12, IndicatorAngle),
            Polar(center, knobRadius - 10, IndicatorAngle));

        context.DrawEllipse(
            new SolidColorBrush(Color.Parse("#B8C0C4")),
            new Pen(new SolidColorBrush(Color.Parse("#111315")), 2),
            center,
            8,
            8);
    }

    private void DrawLevelIndicator(
        DrawingContext context,
        Point center,
        double outerRadius,
        double innerRadius)
    {
        var activeNormalized = Math.Clamp((IndicatorAngle + 135.0) / 270.0, 0.0, 1.0);
        var segments = 34;
        var shadowPen = new Pen(new SolidColorBrush(Color.Parse("#0A0C0D")), 5);

        for (var index = 0; index < segments; index++)
        {
            var startFraction = index / (double)segments;
            var endFraction = (index + 1) / (double)segments;
            var midFraction = (startFraction + endFraction) / 2.0;
            var startAngle = -135.0 + (startFraction * 270.0);
            var endAngle = -135.0 + (endFraction * 270.0);
            var active = midFraction <= activeNormalized;

            var color = active
                ? (midFraction < 0.72
                    ? "#1FA356"
                    : midFraction < 0.9
                        ? "#E2C53C"
                        : "#E05045")
                : "#22313A";

            var pen = new Pen(new SolidColorBrush(Color.Parse(color)), 4);

            var startPoint = MidPoint(
                Polar(center, innerRadius, startAngle),
                Polar(center, outerRadius, startAngle));

            var endPoint = MidPoint(
                Polar(center, innerRadius, endAngle),
                Polar(center, outerRadius, endAngle));

            context.DrawLine(shadowPen, startPoint, endPoint);
            context.DrawLine(pen, startPoint, endPoint);
        }
    }

    private static Point MidPoint(Point a, Point b) =>
        new((a.X + b.X) / 2.0, (a.Y + b.Y) / 2.0);

    private static Point Polar(Point center, double radius, double angleDegrees)
    {
        var radians = (angleDegrees - 90.0) * Math.PI / 180.0;
        return new Point(
            center.X + (Math.Cos(radians) * radius),
            center.Y + (Math.Sin(radians) * radius));
    }
}
