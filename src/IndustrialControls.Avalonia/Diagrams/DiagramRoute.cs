using Avalonia;

namespace IndustrialControls.Avalonia.Diagrams;

internal sealed class DiagramRoute
{
    private readonly double[] _distances;
    public DiagramRoute(IEnumerable<Point> points)
    {
        var compact = new List<Point>();
        foreach (var point in points)
            if (compact.Count == 0 || Between(point, compact[^1]).Length > .001) compact.Add(point);
        Points = compact.ToArray();
        _distances = new double[Points.Length];
        for (var i = 1; i < Points.Length; i++) _distances[i] = _distances[i - 1] + Between(Points[i], Points[i - 1]).Length;
        Length = _distances.Length == 0 ? 0 : _distances[^1];
        Bounds = Points.Length == 0 ? default : new Rect(
            new Point(Points.Min(p => p.X), Points.Min(p => p.Y)),
            new Point(Points.Max(p => p.X), Points.Max(p => p.Y)));
    }
    public Point[] Points { get; }
    public double Length { get; }
    public Rect Bounds { get; }

    public (Point Point, Vector Direction) At(double distance)
    {
        distance = Math.Clamp(distance, 0, Length);
        for (var i = 1; i < Points.Length; i++)
        {
            if (_distances[i] < distance) continue;
            var vector = Between(Points[i], Points[i - 1]);
            var length = vector.Length;
            if (length < .001) continue;
            var direction = vector / length;
            return (Points[i - 1] + direction * (distance - _distances[i - 1]), direction);
        }
        return (Points.Length > 0 ? Points[^1] : default, new Vector(1, 0));
    }

    public bool IsNear(Point point, double tolerance)
    {
        for (var i = 1; i < Points.Length; i++)
        {
            var a = Points[i - 1];
            var delta = Points[i] - a;
            var squaredLength = delta.X * delta.X + delta.Y * delta.Y;
            if (squaredLength < .000001) continue;
            var t = Math.Clamp(((point.X - a.X) * delta.X + (point.Y - a.Y) * delta.Y) / squaredLength, 0, 1);
            if (Between(point, a + delta * t).Length <= tolerance) return true;
        }
        return false;
    }

    public static DiagramRoute Create(DiagramConnection link, DiagramNode source, DiagramNode target)
    {
        var sourceSide = Resolve(link.SourceAnchor, source.Bounds.Center, target.Bounds.Center);
        var targetSide = Resolve(link.TargetAnchor, target.Bounds.Center, source.Bounds.Center);
        var start = Anchor(source.Bounds, sourceSide, link.SourceOffset);
        var end = Anchor(target.Bounds, targetSide, link.TargetOffset);
        if (link.Waypoints.Count > 0)
            return new DiagramRoute(new[] { start }.Concat(link.Waypoints.Select(p => new Point(p.X, p.Y))).Append(end));
        var a = start + Outward(sourceSide) * 20;
        var b = end + Outward(targetSide) * 20;
        var points = new List<Point> { start, a };
        var horizontalStart = sourceSide is DiagramAnchor.Left or DiagramAnchor.Right;
        var horizontalEnd = targetSide is DiagramAnchor.Left or DiagramAnchor.Right;
        if (horizontalStart && horizontalEnd)
        {
            var mid = (a.X + b.X) / 2;
            points.Add(new Point(mid, a.Y)); points.Add(new Point(mid, b.Y));
        }
        else if (!horizontalStart && !horizontalEnd)
        {
            var mid = (a.Y + b.Y) / 2;
            points.Add(new Point(a.X, mid)); points.Add(new Point(b.X, mid));
        }
        else points.Add(horizontalStart ? new Point(b.X, a.Y) : new Point(a.X, b.Y));
        points.Add(b); points.Add(end);
        return new DiagramRoute(points);
    }

    private static DiagramAnchor Resolve(DiagramAnchor anchor, Point origin, Point target)
    {
        if (anchor != DiagramAnchor.Auto) return anchor;
        var delta = target - origin;
        return Math.Abs(delta.X) >= Math.Abs(delta.Y)
            ? delta.X >= 0 ? DiagramAnchor.Right : DiagramAnchor.Left
            : delta.Y >= 0 ? DiagramAnchor.Bottom : DiagramAnchor.Top;
    }
    private static Point Anchor(Rect r, DiagramAnchor side, double offset) => side switch
    {
        DiagramAnchor.Left => new Point(r.Left, r.Top + r.Height * offset),
        DiagramAnchor.Right => new Point(r.Right, r.Top + r.Height * offset),
        DiagramAnchor.Top => new Point(r.Left + r.Width * offset, r.Top),
        _ => new Point(r.Left + r.Width * offset, r.Bottom),
    };
    private static Vector Outward(DiagramAnchor side) => side switch
    {
        DiagramAnchor.Left => new Vector(-1, 0), DiagramAnchor.Right => new Vector(1, 0),
        DiagramAnchor.Top => new Vector(0, -1), _ => new Vector(0, 1),
    };
    private static Vector Between(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
}
