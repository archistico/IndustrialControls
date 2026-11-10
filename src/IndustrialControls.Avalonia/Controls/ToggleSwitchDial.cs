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

    private static readonly IBrush PlateBrush = Solid("#292C2F");
    private static readonly IBrush ScrewBrush = Solid("#858B8E");
    private static readonly IBrush NutBrush = Solid("#A8ADB0");
    private static readonly IBrush HandleBrush = Solid("#B8BDC0");
    private static readonly IBrush LockedHandleBrush = Solid("#666B6E");
    private static readonly IBrush ShadowBrush = Solid("#17191B");
    private static readonly IBrush SocketBrush = Solid("#111416");
    private static readonly IBrush PivotBrush = Solid("#44494C");
    private static readonly IBrush MutedBrush = Solid("#888E91");
    private static readonly IBrush OnBrush = Solid("#58D46C");
    private static readonly IBrush OffBrush = Solid("#F1F1DF");
    private static readonly Pen ScrewPen = new(ShadowBrush, 1);
    private static readonly Pen OutlinePen = new(ShadowBrush, 2);
    private static readonly Pen FramePen = new(ShadowBrush, 4);
    private static readonly Pen InnerEdgePen = new(Solid("#969DA1"), 1);

    static ToggleSwitchDial() => AffectsRender<ToggleSwitchDial>(IsOnProperty, IsInterlockedProperty);

    public bool IsOn { get => GetValue(IsOnProperty); set => SetValue(IsOnProperty, value); }
    public bool IsInterlocked { get => GetValue(IsInterlockedProperty); set => SetValue(IsInterlockedProperty, value); }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var scale = System.Math.Min(Bounds.Width, Bounds.Height) / 150;
        if (scale <= .05) return;

        // One proportioned mechanical assembly, centered at every supported size.
        using var transform = context.PushTransform(new Matrix(scale, 0, 0, scale,
            (Bounds.Width - 150 * scale) / 2, (Bounds.Height - 150 * scale) / 2));
        var pivot = new Point(75, 79);
        var handle = IsInterlocked ? LockedHandleBrush : HandleBrush;

        // Match the panel/bezel borders and the 9 px bolts of BoltedDataPlate.
        context.DrawRectangle(PlateBrush, FramePen, new Rect(18, 22, 114, 114), 4, 4);
        context.DrawRectangle(null, InnerEdgePen, new Rect(22, 26, 106, 106), 2, 2);
        DrawScrew(context, new Point(30, 34));
        DrawScrew(context, new Point(120, 34));
        DrawScrew(context, new Point(30, 124));
        DrawScrew(context, new Point(120, 124));

        // Flat fills and ordinary outlines, like the selector and rocker dials.
        context.DrawEllipse(SocketBrush, FramePen, pivot, 30, 30);
        var nut = new StreamGeometry();
        using (var geometry = nut.Open())
        {
            geometry.BeginFigure(new Point(63, 58), true);
            geometry.LineTo(new Point(87, 58));
            geometry.LineTo(new Point(99, 79));
            geometry.LineTo(new Point(87, 100));
            geometry.LineTo(new Point(63, 100));
            geometry.LineTo(new Point(51, 79));
            geometry.EndFigure(true);
        }
        context.DrawGeometry(NutBrush, OutlinePen, nut);
        context.DrawEllipse(SocketBrush, OutlinePen, pivot, 17, 17);
        context.DrawEllipse(PivotBrush, OutlinePen, pivot, 10, 10);

        // Same 12 px outline / 8 px stem used by SelectorSwitchDial.
        var tip = new Point(75, IsOn ? 39 : 117);
        context.DrawLine(new Pen(ShadowBrush, 12), pivot, tip);
        context.DrawLine(new Pen(handle, 8), pivot, tip);
        context.DrawRectangle(handle, OutlinePen, new Rect(66, IsOn ? 25 : 105, 18, 26), 4, 4);

        // Fixed position marks remain readable independently of the metal finish.
        var onMark = !IsInterlocked && IsOn ? OnBrush : MutedBrush;
        var offMark = !IsInterlocked && !IsOn ? OffBrush : MutedBrush;
        context.DrawLine(new Pen(onMark, 2), new Point(113, 58), new Point(113, 66));
        context.DrawEllipse(null, new Pen(offMark, 2), new Point(37, 99), 4, 4);
    }

    private static void DrawScrew(DrawingContext context, Point center)
    {
        context.DrawEllipse(ScrewBrush, ScrewPen, center, 4.5, 4.5);
    }
    private static IBrush Solid(string color) => new SolidColorBrush(Color.Parse(color));
}
