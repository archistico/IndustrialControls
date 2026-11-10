using System.Globalization;
using Avalonia;
using Avalonia.Media;
using IndustrialControls.Avalonia.Diagrams;

namespace IndustrialControls.Avalonia.Controls;

public sealed partial class IndustrialDiagram
{
    private static readonly IBrush CanvasBrush = Brush("#101820"), GridBrush = Brush("#25333E"), CardBrush = Brush("#182530");
    private static readonly IBrush GroupBrush = Brush("#131F29"), BorderBrush = Brush("#354C5B"), TextBrush = Brush("#E3EDF3"), MutedBrush = Brush("#8FA8B8");
    private static readonly IBrush SelectionBrush = Brush("#F2DC8B"), WaterBrush = Brush("#56BED7"), SteamBrush = Brush("#EDBD78");
    private static readonly IBrush GreenBrush = Brush("#73CDA9"), RedBrush = Brush("#F17C79"), PurpleBrush = Brush("#B4A1E4"), GreyBrush = Brush("#697D89");
    private static readonly Pen GridPen = new(GridBrush, 1), CardPen = new(BorderBrush, 1), SelectedPen = new(SelectionBrush, 2);
    private readonly Dictionary<(string Text, double Size, IBrush Brush, double Width, double Height), FormattedText> _textCache = new();
    private static IBrush Brush(string color) => new SolidColorBrush(Color.Parse(color));

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        context.DrawRectangle(CanvasBrush, null, new Rect(Bounds.Size));
        if (Bounds.Width <= 0 || Bounds.Height <= 0) return;
        using var clip = context.PushClip(new Rect(Bounds.Size));
        if (IsFocused) context.DrawRectangle(null, SelectedPen, new Rect(Bounds.Size).Deflate(2), 5, 5);
        if (ShowGrid) DrawGrid(context);
        if (Document is null) return;
        EnsureRoutes();
        using var transform = context.PushTransform(new Matrix(Zoom, 0, 0, Zoom, PanOffset.X, PanOffset.Y));
        var viewport = new Rect(ToDiagram(default), ToDiagram(new Point(Bounds.Width, Bounds.Height))).Inflate(60 / Zoom);
        foreach (var group in Document.Groups)
        {
            if (!group.IsVisible || !group.Bounds.Intersects(viewport)) continue;
            var pen = group.Id == SelectedElementId ? SelectedPen : new Pen(BorderBrush, 1, new DashStyle(new double[] { 7, 5 }, 0));
            context.DrawRectangle(GroupBrush, pen, group.Bounds, 10, 10);
            DrawText(context, group.Label.ToUpperInvariant(), new Point(group.X + 14, group.Y + 12), 11, MutedBrush, group.Width - 28);
            DrawAlarm(context, group, new Point(group.Bounds.Right - 12, group.Y + 16));
        }
        foreach (var link in Document.Connections)
            if (link.IsVisible && _routes.TryGetValue(link.Id, out var route) && route.Bounds.Inflate(50).Intersects(viewport)) DrawConnection(context, link, route);
        foreach (var node in Document.Nodes)
            if (node.IsVisible && node.Bounds.Intersects(viewport)) DrawNode(context, node);
        foreach (var label in Document.Texts)
        {
            if (!label.IsVisible || !label.Bounds.Intersects(viewport)) continue;
            if (label.Id == SelectedElementId) context.DrawRectangle(null, SelectedPen, label.Bounds, 3, 3);
            if (label.Label.Length > 0) context.DrawText(Text(label.Label, label.FontSize, StateBrush(label.State), label.Width, label.Height), new Point(label.X, label.Y));
            DrawAlarm(context, label, new Point(label.Bounds.Right, label.Y));
        }
    }

    private void DrawGrid(DrawingContext context)
    {
        var step = 24 * Zoom;
        while (step < 12) step *= 2;
        var startX = ((PanOffset.X % step) + step) % step;
        var startY = ((PanOffset.Y % step) + step) % step;
        for (var x = startX; x < Bounds.Width; x += step)
            for (var y = startY; y < Bounds.Height; y += step) context.DrawRectangle(GridBrush, null, new Rect(x, y, 1, 1));
    }

    private void DrawConnection(DrawingContext context, DiagramConnection link, DiagramRoute route)
    {
        if (route.Length < .001) return;
        var color = link.State == DiagramState.Unavailable ? GreyBrush : MediumBrush(link.Medium);
        var dash = link.Medium == DiagramMedium.Signal || link.State == DiagramState.Unavailable ? new DashStyle(new double[] { 5, 4 }, 0) : null;
        var shadow = new Pen(CanvasBrush, link.Thickness + 5);
        var pen = new Pen(color, link.Thickness, dash);
        for (var i = 1; i < route.Points.Length; i++)
        {
            context.DrawLine(shadow, route.Points[i - 1], route.Points[i]);
            context.DrawLine(link.Id == SelectedElementId ? new Pen(SelectionBrush, link.Thickness + 3) : pen, route.Points[i - 1], route.Points[i]);
        }
        if (link.FlowDirection != DiagramFlowDirection.None && link.State != DiagramState.Unavailable)
        {
            const double spacing = 52;
            var phase = (AnimationTime * link.FlowSpeed) % spacing;
            for (var distance = phase + 8; distance < route.Length - 8; distance += spacing)
            {
                if (distance > spacing * 200) break;
                var at = route.At(link.FlowDirection == DiagramFlowDirection.Reverse ? route.Length - distance : distance);
                context.DrawEllipse(TextBrush, null, at.Point, 2.4, 2.4);
            }
        }
        if (link.IsDirected)
        {
            var reverse = link.FlowDirection == DiagramFlowDirection.Reverse;
            var at = route.At(reverse ? Math.Min(16, route.Length / 2) : Math.Max(route.Length - 16, route.Length / 2));
            DrawArrow(context, at.Point, reverse ? -at.Direction : at.Direction, color);
        }
        var text = link.Label + (link.ValueText.Length > 0 ? "  ·  " + link.ValueText : string.Empty);
        if (text.Length > 0)
        {
            var at = route.At(route.Length / 2).Point + new Vector(link.LabelOffsetX, link.LabelOffsetY);
            var formatted = Text(text, 10, color, 360);
            var bounds = new Rect(at.X - formatted.Width / 2 - 6, at.Y - 4, formatted.Width + 12, formatted.Height + 8);
            context.DrawRectangle(CanvasBrush, null, bounds, 3, 3);
            context.DrawText(formatted, new Point(bounds.X + 6, bounds.Y + 4));
            DrawAlarm(context, link, new Point(bounds.Right + 8, bounds.Center.Y));
        }
        else DrawAlarm(context, link, route.At(route.Length / 2).Point);
    }

    private void DrawNode(DrawingContext context, DiagramNode node)
    {
        var bounds = node.Bounds;
        var accent = StateBrush(node.State);
        if (node.Symbol == DiagramSymbol.Junction)
        {
            context.DrawEllipse(node.Id == SelectedElementId ? SelectionBrush : accent, null, bounds.Center, 5, 5);
            DrawText(context, node.Label, new Point(bounds.Right + 6, bounds.Y), 10, MutedBrush, 140);
            DrawAlarm(context, node, bounds.TopRight);
            return;
        }
        context.DrawRectangle(BrushShadow, null, bounds.Translate(new Vector(0, 4)), 8, 8);
        context.DrawRectangle(CardBrush, node.Id == SelectedElementId ? SelectedPen : CardPen, bounds, 8, 8);
        context.DrawRectangle(accent, null, new Rect(bounds.X, bounds.Y + 12, 3, Math.Max(1, bounds.Height - 24)), 1, 1);
        var icon = new Rect(bounds.X + 12, bounds.Y + 12, 36, 36);
        DrawSymbol(context, node, icon, accent);
        DrawText(context, node.Label, new Point(bounds.X + 58, bounds.Y + 12), 12, TextBrush, Math.Max(10, bounds.Width - 72));
        DrawText(context, node.Id, new Point(bounds.X + 58, bounds.Y + 30), 9, MutedBrush, Math.Max(10, bounds.Width - 72));
        DrawText(context, node.ValueText, new Point(bounds.X + 14, bounds.Y + 57), 18, node.State == DiagramState.Unavailable ? GreyBrush : TextBrush, Math.Max(10, bounds.Width - 28));
        var state = node.State.ToString().ToUpperInvariant();
        DrawText(context, node.DetailText.Length > 0 ? node.DetailText : state, new Point(bounds.X + 14, bounds.Bottom - 20), 9, accent, Math.Max(10, bounds.Width - 28));
        DrawAlarm(context, node, new Point(bounds.Right - 11, bounds.Y + 10));
    }
    private static readonly IBrush BrushShadow = Brush("#090F14");
    private void DrawAlarm(DrawingContext context, DiagramElement element, Point center)
    {
        if (element.AlarmSeverity == DiagramAlarmSeverity.None) return;
        var color = element.AlarmSeverity == DiagramAlarmSeverity.Critical ? RedBrush : element.AlarmSeverity == DiagramAlarmSeverity.Advisory ? WaterBrush : SteamBrush;
        var flash = !IsAnimationEnabled || element.IsAlarmAcknowledged || AnimationTime % 1 < .65;
        context.DrawEllipse(CanvasBrush, new Pen(color, 2), center, 8, 8);
        if (flash) context.DrawEllipse(color, null, center, 5, 5);
        DrawText(context, element.IsAlarmAcknowledged ? "✓" : "!", new Point(center.X - 3, center.Y - 7), 10, flash ? CanvasBrush : color, 12);
    }
    private static void DrawArrow(DrawingContext context, Point at, Vector direction, IBrush brush)
    {
        var side = new Vector(-direction.Y, direction.X);
        var pen = new Pen(brush, 2);
        context.DrawLine(pen, at - direction * 8 + side * 4, at);
        context.DrawLine(pen, at - direction * 8 - side * 4, at);
    }
    private void DrawText(DrawingContext context, string text, Point at, double size, IBrush brush, double width)
    {
        if (string.IsNullOrEmpty(text)) return;
        context.DrawText(Text(text, size, brush, width), at);
    }
    private FormattedText Text(string text, double size, IBrush brush, double width, double? height = null)
    {
        var key = (text, size, brush, Math.Max(1, width), Math.Max(1, height ?? size * 1.6));
        if (_textCache.TryGetValue(key, out var result)) return result;
        if (_textCache.Count >= 1024) _textCache.Clear();
        result = new FormattedText(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, Typeface.Default, size, brush)
        { MaxTextWidth = key.Item4, MaxTextHeight = key.Item5, Trimming = TextTrimming.CharacterEllipsis };
        _textCache[key] = result;
        return result;
    }
    private static IBrush StateBrush(DiagramState state) => state switch
    {
        DiagramState.Active => GreenBrush, DiagramState.Warning => SteamBrush, DiagramState.Trip => RedBrush,
        DiagramState.Unavailable => GreyBrush, _ => MutedBrush,
    };
    private static IBrush MediumBrush(DiagramMedium medium) => medium switch
    {
        DiagramMedium.Water => WaterBrush, DiagramMedium.Steam => SteamBrush, DiagramMedium.Gas => GreenBrush,
        DiagramMedium.Electrical => SelectionBrush, DiagramMedium.Signal => PurpleBrush, _ => MutedBrush,
    };
}
