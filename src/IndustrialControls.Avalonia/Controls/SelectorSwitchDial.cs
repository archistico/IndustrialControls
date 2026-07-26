using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Renderer vettoriale interno per selettori rotativi e comandi a ritorno a molla.
/// </summary>
public sealed class SelectorSwitchDial : Control
{
    public static readonly StyledProperty<int> PositionCountProperty =
        AvaloniaProperty.Register<SelectorSwitchDial, int>(nameof(PositionCount), 3);

    public static readonly StyledProperty<int> PositionProperty =
        AvaloniaProperty.Register<SelectorSwitchDial, int>(nameof(Position), 0);

    public static readonly StyledProperty<string> PositionLabelsProperty =
        AvaloniaProperty.Register<SelectorSwitchDial, string>(
            nameof(PositionLabels), "OFF|AUTO|MANUAL");

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<SelectorSwitchDial, bool>(nameof(IsInterlocked));

    static SelectorSwitchDial()
    {
        AffectsRender<SelectorSwitchDial>(
            PositionCountProperty,
            PositionProperty,
            PositionLabelsProperty,
            IsInterlockedProperty);
    }

    public int PositionCount
    {
        get => GetValue(PositionCountProperty);
        set => SetValue(PositionCountProperty, value);
    }

    public int Position
    {
        get => GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public string PositionLabels
    {
        get => GetValue(PositionLabelsProperty);
        set => SetValue(PositionLabelsProperty, value);
    }

    public bool IsInterlocked
    {
        get => GetValue(IsInterlockedProperty);
        set => SetValue(IsInterlockedProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width <= 20 || Bounds.Height <= 20)
        {
            return;
        }

        var count = Math.Clamp(PositionCount, 2, 5);
        var position = Math.Clamp(Position, 0, count - 1);
        var labels = PositionLabels.Split(
            '|',
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var center = new Point(Bounds.Width / 2.0, (Bounds.Height / 2.0) + 12.0);
        var plateRadius = Math.Max(
            56.0,
            Math.Min((Bounds.Width / 2.0) - 32.0, (Bounds.Height / 2.0) - 26.0));

        context.DrawEllipse(
            new SolidColorBrush(Color.Parse("#141718")),
            new Pen(new SolidColorBrush(Color.Parse("#080A0B")), 5),
            center,
            plateRadius,
            plateRadius);

        context.DrawEllipse(
            null,
            new Pen(new SolidColorBrush(Color.Parse("#7B8286")), 1),
            center,
            plateRadius - 5,
            plateRadius - 5);

        for (var index = 0; index < count; index++)
        {
            var angle = -60.0 + (index * (120.0 / (count - 1)));
            var active = index == position;
            var tickBrush = new SolidColorBrush(
                Color.Parse(active && !IsInterlocked ? "#F1F1DF" : "#888E91"));

            context.DrawLine(
                new Pen(tickBrush, active ? 4 : 2),
                Polar(center, plateRadius - 22, angle),
                Polar(center, plateRadius - 8, angle));

            var label = index < labels.Length
                ? labels[index]
                : (index + 1).ToString(CultureInfo.InvariantCulture);

            var formatted = new FormattedText(
                label,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                Typeface.Default,
                9,
                tickBrush);

            // Etichette all'esterno del quadrante con un margine più ampio.
            var labelPosition = Polar(center, plateRadius + 18, angle);
            context.DrawText(
                formatted,
                new Point(
                    labelPosition.X - (formatted.Width / 2.0),
                    labelPosition.Y - (formatted.Height / 2.0)));
        }

        var handleAngle = -60.0 + (position * (120.0 / (count - 1)));
        var handleBrush = new SolidColorBrush(
            Color.Parse(IsInterlocked ? "#666B6E" : "#B8BDC0"));

        context.DrawEllipse(
            new SolidColorBrush(Color.Parse("#44494C")),
            new Pen(new SolidColorBrush(Color.Parse("#0A0B0C")), 4),
            center,
            34,
            34);

        context.DrawLine(
            new Pen(new SolidColorBrush(Color.Parse("#090A0B")), 12),
            center,
            Polar(center, 52, handleAngle));

        context.DrawLine(
            new Pen(handleBrush, 8),
            center,
            Polar(center, 52, handleAngle));

        context.DrawEllipse(
            handleBrush,
            new Pen(new SolidColorBrush(Color.Parse("#111315")), 2),
            center,
            10,
            10);
    }

    private static Point Polar(Point center, double radius, double angleDegrees)
    {
        var radians = (angleDegrees - 90.0) * Math.PI / 180.0;
        return new Point(
            center.X + (Math.Cos(radians) * radius),
            center.Y + (Math.Sin(radians) * radius));
    }
}
