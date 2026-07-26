using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Renderer vettoriale interno per l'interruttore ON/OFF a bilanciere.
/// </summary>
public sealed class RockerSwitchDial : Control
{
    public static readonly StyledProperty<bool> IsOnProperty =
        AvaloniaProperty.Register<RockerSwitchDial, bool>(nameof(IsOn));

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<RockerSwitchDial, bool>(nameof(IsInterlocked));

    static RockerSwitchDial()
    {
        AffectsRender<RockerSwitchDial>(IsOnProperty, IsInterlockedProperty);
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

        var plate = new Rect(8, 8, Bounds.Width - 16, Bounds.Height - 16);
        if (plate.Width <= 10 || plate.Height <= 10)
        {
            return;
        }

        var frameBrush = new SolidColorBrush(Color.Parse("#2E4761"));
        var framePen = new Pen(new SolidColorBrush(Color.Parse("#9AB0BD")), 4);
        var recessBrush = new SolidColorBrush(Color.Parse("#243341"));
        var rockerBodyBrush = new SolidColorBrush(
            Color.Parse(IsInterlocked ? "#73766A" : "#8C8F42"));
        var activeFaceBrush = new SolidColorBrush(
            Color.Parse(IsInterlocked ? "#8A8C85" : "#B4B85A"));
        var passiveFaceBrush = new SolidColorBrush(
            Color.Parse(IsInterlocked ? "#676A62" : "#747738"));
        var symbolBrush = new SolidColorBrush(Color.Parse("#1C2630"));

        context.DrawRectangle(frameBrush, framePen, plate);

        var recess = plate.Deflate(10);
        context.DrawRectangle(recessBrush, new Pen(new SolidColorBrush(Color.Parse("#14202A")), 2), recess);

        var rocker = recess.Deflate(8);
        context.DrawRectangle(rockerBodyBrush, new Pen(new SolidColorBrush(Color.Parse("#22313B")), 2), rocker);

        var top = new Rect(rocker.X, rocker.Y, rocker.Width, rocker.Height / 2.0);
        var bottom = new Rect(rocker.X, rocker.Y + (rocker.Height / 2.0), rocker.Width, rocker.Height / 2.0);

        context.DrawRectangle(IsOn ? activeFaceBrush : passiveFaceBrush, null, top);
        context.DrawRectangle(IsOn ? passiveFaceBrush : activeFaceBrush, null, bottom);

        var topInset = IsOn ? 2.0 : 8.0;
        var bottomInset = IsOn ? 8.0 : 2.0;

        context.DrawRectangle(null, new Pen(new SolidColorBrush(Color.Parse("#CFD5C8")), 1),
            new Rect(top.X + 6, top.Y + topInset, top.Width - 12, top.Height - 10));
        context.DrawRectangle(null, new Pen(new SolidColorBrush(Color.Parse("#CFD5C8")), 1),
            new Rect(bottom.X + 6, bottom.Y + bottomInset, bottom.Width - 12, bottom.Height - 10));

        // "I" in alto, "O" in basso
        var lineX = top.Center.X;
        context.DrawLine(
            new Pen(symbolBrush, 3),
            new Point(lineX, top.Y + 14 + topInset),
            new Point(lineX, top.Y + top.Height - 14));

        context.DrawEllipse(
            null,
            new Pen(symbolBrush, 3),
            new Point(bottom.Center.X, bottom.Center.Y + ((bottomInset - 5) / 2.0)),
            9,
            9);
    }
}
