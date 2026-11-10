using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace IndustrialControls.Avalonia.Diagrams;

public sealed record DiagramPoint
{
    [JsonConstructor]
    public DiagramPoint(double x, double y) { X = DiagramElement.Finite(x); Y = DiagramElement.Finite(y); }
    public double X { get; }
    public double Y { get; }
}

public sealed class DiagramConnection : DiagramElement
{
    private string _sourceId = string.Empty, _targetId = string.Empty, _valueText = string.Empty;
    private DiagramAnchor _sourceAnchor, _targetAnchor;
    private DiagramMedium _medium;
    private DiagramFlowDirection _flowDirection;
    private double _sourceOffset = .5, _targetOffset = .5, _thickness = 3, _flowSpeed = 44;
    private double _labelOffsetX, _labelOffsetY = -14;
    private bool _isDirected = true;

    [JsonConstructor]
    public DiagramConnection(string id, ObservableCollection<DiagramPoint>? waypoints = null) : base(id)
    {
        Waypoints = waypoints ?? new ObservableCollection<DiagramPoint>();
        Waypoints.CollectionChanged += (_, _) => Set(ref _routeRevision, _routeRevision + 1, nameof(Waypoints));
    }
    private long _routeRevision;
    public string SourceId { get => _sourceId; set => Set(ref _sourceId, value ?? string.Empty); }
    public string TargetId { get => _targetId; set => Set(ref _targetId, value ?? string.Empty); }
    public string ValueText { get => _valueText; set => Set(ref _valueText, value ?? string.Empty); }
    public DiagramAnchor SourceAnchor { get => _sourceAnchor; set => Set(ref _sourceAnchor, Defined(value)); }
    public DiagramAnchor TargetAnchor { get => _targetAnchor; set => Set(ref _targetAnchor, Defined(value)); }
    public double SourceOffset { get => _sourceOffset; set => Set(ref _sourceOffset, Fraction(value)); }
    public double TargetOffset { get => _targetOffset; set => Set(ref _targetOffset, Fraction(value)); }
    public DiagramMedium Medium { get => _medium; set => Set(ref _medium, Defined(value)); }
    public DiagramFlowDirection FlowDirection { get => _flowDirection; set => Set(ref _flowDirection, Defined(value)); }
    /// <summary>Visual speed in diagram units per second, independent of physical units and simulation time.</summary>
    public double FlowSpeed { get => _flowSpeed; set => Set(ref _flowSpeed, double.IsFinite(value) && value is >= 0 and <= 300 ? value : throw new ArgumentOutOfRangeException(nameof(value))); }
    public double Thickness { get => _thickness; set => Set(ref _thickness, double.IsFinite(value) && value is >= 1 and <= 16 ? value : throw new ArgumentOutOfRangeException(nameof(value))); }
    public double LabelOffsetX { get => _labelOffsetX; set => Set(ref _labelOffsetX, Finite(value)); }
    public double LabelOffsetY { get => _labelOffsetY; set => Set(ref _labelOffsetY, Finite(value)); }
    public bool IsDirected { get => _isDirected; set => Set(ref _isDirected, value); }
    public ObservableCollection<DiagramPoint> Waypoints { get; }
    private static double Fraction(double value)
        => double.IsFinite(value) && value is >= 0 and <= 1 ? value : throw new ArgumentOutOfRangeException(nameof(value));
}
