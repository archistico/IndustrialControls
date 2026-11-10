using Avalonia;
using Avalonia.Media;
using IndustrialControls.Avalonia.Diagrams;

namespace IndustrialControls.Avalonia.Controls;

public sealed partial class IndustrialDiagram
{
    private void DrawSymbol(DrawingContext context, DiagramNode node, Rect icon, IBrush accent)
    {
        if (SymbolRenderer?.Render(context, node, icon, accent) == true) return;
        using var transform = context.PushTransform(Matrix.CreateTranslation(icon.X, icon.Y));
        var pen = new Pen(accent, 1.8);
        switch (node.Symbol)
        {
            case DiagramSymbol.Reactor:
                DrawVessel(context, new Rect(8, 1, 20, 34), 8, node.FillFraction, pen);
                for (var x = 13; x <= 23; x += 5) context.DrawLine(pen, new Point(x, 7), new Point(x, 27));
                context.DrawLine(pen, new Point(1, 11), new Point(8, 11));
                context.DrawLine(pen, new Point(28, 25), new Point(35, 25));
                break;
            case DiagramSymbol.Pump:
                context.DrawEllipse(null, pen, new Point(18, 18), 13, 13);
                Polygon(context, accent, null, new Point(12, 9), new Point(27, 18), new Point(12, 27));
                context.DrawLine(pen, new Point(1, 18), new Point(5, 18));
                context.DrawLine(pen, new Point(31, 18), new Point(36, 18));
                break;
            case DiagramSymbol.Valve:
                Polygon(context, null, pen, new Point(3, 9), new Point(18, 18), new Point(3, 27));
                Polygon(context, null, pen, new Point(33, 9), new Point(18, 18), new Point(33, 27));
                context.DrawLine(pen, new Point(18, 18), new Point(18, 3));
                context.DrawLine(pen, new Point(11, 3), new Point(25, 3));
                break;
            case DiagramSymbol.Tank:
                DrawVessel(context, new Rect(5, 3, 26, 30), 4, node.FillFraction, pen);
                context.DrawLine(pen, new Point(5, 10), new Point(31, 10));
                break;
            case DiagramSymbol.SteamDrum:
                DrawVessel(context, new Rect(1, 8, 34, 20), 10, node.FillFraction, pen);
                context.DrawLine(pen, new Point(18, 1), new Point(18, 8));
                context.DrawLine(pen, new Point(18, 28), new Point(18, 35));
                break;
            case DiagramSymbol.HeatExchanger:
                context.DrawEllipse(null, pen, new Point(18, 18), 15, 15);
                Polyline(context, pen, new Point(4, 18), new Point(10, 10), new Point(15, 26), new Point(21, 10), new Point(26, 26), new Point(32, 18));
                break;
            case DiagramSymbol.Turbine:
                Polygon(context, null, pen, new Point(4, 5), new Point(30, 12), new Point(30, 24), new Point(4, 31));
                context.DrawLine(pen, new Point(4, 18), new Point(36, 18));
                context.DrawLine(pen, new Point(12, 10), new Point(12, 26));
                context.DrawLine(pen, new Point(20, 12), new Point(20, 24));
                break;
            case DiagramSymbol.Generator:
                context.DrawEllipse(null, pen, new Point(18, 18), 15, 15);
                DrawText(context, "G", new Point(10, 6), 20, accent, 25);
                break;
            case DiagramSymbol.Condenser:
                context.DrawRectangle(null, pen, new Rect(3, 6, 30, 24), 3, 3);
                Polyline(context, pen, new Point(7, 12), new Point(29, 12), new Point(7, 18), new Point(29, 18), new Point(7, 24), new Point(29, 24));
                break;
            case DiagramSymbol.ElectricalBus:
                context.DrawLine(new Pen(accent, 3), new Point(12, 2), new Point(12, 34));
                context.DrawLine(new Pen(accent, 3), new Point(22, 2), new Point(22, 34));
                for (var y = 8; y <= 28; y += 10) context.DrawLine(pen, new Point(22, y), new Point(35, y));
                break;
            case DiagramSymbol.Sensor:
                context.DrawEllipse(null, pen, new Point(18, 18), 13, 13);
                context.DrawLine(pen, new Point(5, 18), new Point(31, 18));
                DrawText(context, "I", new Point(15, 2), 12, accent, 16);
                context.DrawLine(pen, new Point(18, 31), new Point(18, 36));
                break;
            case DiagramSymbol.Controller:
                context.DrawRectangle(null, pen, new Rect(2, 6, 32, 24), 3, 3);
                DrawText(context, "PID", new Point(6, 11), 12, accent, 30);
                break;
            default:
                context.DrawRectangle(null, pen, new Rect(4, 4, 28, 28), 4, 4);
                context.DrawLine(pen, new Point(10, 12), new Point(26, 12));
                context.DrawLine(pen, new Point(10, 19), new Point(26, 19));
                context.DrawLine(pen, new Point(10, 26), new Point(21, 26));
                break;
        }
    }
    private static void DrawVessel(DrawingContext context, Rect rect, double radius, double? fill, Pen pen)
    {
        if (fill.HasValue)
        {
            using var clip = context.PushClip(new RoundedRect(rect, radius));
            using var opacity = context.PushOpacity(.25);
            var height = rect.Height * fill.Value;
            context.DrawRectangle(WaterBrush, null, new Rect(rect.X, rect.Bottom - height, rect.Width, height));
        }
        context.DrawRectangle(null, pen, rect, radius, radius);
    }
    private static void Polyline(DrawingContext context, Pen pen, params Point[] points)
    {
        for (var i = 1; i < points.Length; i++) context.DrawLine(pen, points[i - 1], points[i]);
    }
    private static void Polygon(DrawingContext context, IBrush? fill, Pen? pen, params Point[] points)
    {
        var geometry = new StreamGeometry();
        using (var builder = geometry.Open())
        {
            builder.BeginFigure(points[0], fill is not null);
            for (var i = 1; i < points.Length; i++) builder.LineTo(points[i]);
            builder.EndFigure(true);
        }
        context.DrawGeometry(fill, pen, geometry);
    }
}
