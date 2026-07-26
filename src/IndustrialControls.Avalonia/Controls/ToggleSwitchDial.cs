using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Renderer vettoriale interno dell'interruttore a leva.
/// </summary>
public sealed class ToggleSwitchDial : Control
{
    public static readonly StyledProperty<bool> IsOnProperty =
        AvaloniaProperty.Register<ToggleSwitchDial, bool>(nameof(IsOn));

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<ToggleSwitchDial, bool>(nameof(IsInterlocked));

    static ToggleSwitchDial()
    {
        AffectsRender<ToggleSwitchDial>(IsOnProperty, IsInterlockedProperty);
    }

    public bool IsOn
    {
        get => GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }

    public bool IsInterlocked
    {
        get => GetValue(IsInterlockedProperty);
        set => SetValue(IsInterlockedProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var size = Bounds.Size;
        if (size.Width <= 20 || size.Height <= 20)
        {
            return;
        }

        var plate = new Rect(18, 22, size.Width - 36, size.Height - 36);
        var frameBrush = new SolidColorBrush(Color.Parse("#9DB5C3"));
        var plateBrush = new SolidColorBrush(Color.Parse("#284156"));
        var ringBrush = new SolidColorBrush(Color.Parse("#D5D7D0"));
        var socketBrush = new SolidColorBrush(Color.Parse("#22313F"));

        context.DrawRectangle(frameBrush, null, plate);

        var innerPlate = plate.Deflate(12);
        context.DrawRectangle(
            plateBrush,
            new Pen(new SolidColorBrush(Color.Parse("#1E2F3B")), 2),
            innerPlate);

        var center = innerPlate.Center;
        var ringRadius = System.Math.Min(innerPlate.Width, innerPlate.Height) / 2.8;

        context.DrawEllipse(
            null,
            new Pen(ringBrush, 3),
            center,
            ringRadius,
            ringRadius);

        context.DrawEllipse(
            socketBrush,
            new Pen(new SolidColorBrush(Color.Parse("#14202A")), 2),
            center,
            ringRadius - 5,
            ringRadius - 5);

        var stemColor = IsInterlocked
            ? "#7C7F80"
            : (IsOn ? "#F2A02C" : "#D35463");

        var headColor = IsInterlocked
            ? "#909395"
            : (IsOn ? "#FDB33F" : "#D96471");

        var stemBrush = new SolidColorBrush(Color.Parse(stemColor));
        var headBrush = new SolidColorBrush(Color.Parse(headColor));

        var headCenter = IsOn
            ? new Point(center.X, center.Y - 34)
            : new Point(center.X, center.Y + 34);

        context.DrawLine(
            new Pen(new SolidColorBrush(Color.Parse("#11202A")), 12),
            center,
            headCenter);

        context.DrawLine(
            new Pen(stemBrush, 8),
            center,
            headCenter);

        context.DrawEllipse(
            headBrush,
            new Pen(new SolidColorBrush(Color.Parse("#20313F")), 2),
            headCenter,
            15,
            15);

        context.DrawEllipse(
            new SolidColorBrush(Color.Parse("#163245")),
            new Pen(new SolidColorBrush(Color.Parse("#1A242C")), 1),
            headCenter,
            7,
            7);
    }
}
