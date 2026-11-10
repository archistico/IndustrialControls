using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Avalonia;

namespace IndustrialControls.Avalonia.Diagrams;

/// <summary>Observable scene model. Mutate on the UI thread; batch host snapshots with BeginUpdate.</summary>
public sealed class DiagramDocument
{
    private readonly HashSet<DiagramElement> _subscribed = new();
    private int _updateDepth;
    private bool _pending;
    private bool _pendingGeometry;
    public DiagramDocument()
    {
        Nodes.CollectionChanged += OnCollectionChanged;
        Connections.CollectionChanged += OnCollectionChanged;
        Groups.CollectionChanged += OnCollectionChanged;
        Texts.CollectionChanged += OnCollectionChanged;
    }
    public ObservableCollection<DiagramNode> Nodes { get; } = new ItemCollection<DiagramNode>();
    public ObservableCollection<DiagramConnection> Connections { get; } = new ItemCollection<DiagramConnection>();
    public ObservableCollection<DiagramGroup> Groups { get; } = new ItemCollection<DiagramGroup>();
    public ObservableCollection<DiagramText> Texts { get; } = new ItemCollection<DiagramText>();
    [JsonIgnore] public long Revision { get; private set; }
    [JsonIgnore] public long GeometryRevision { get; private set; }
    public event EventHandler? Changed;

    public IDisposable BeginUpdate() { _updateDepth++; return new UpdateScope(this); }
    public DiagramElement? FindElement(string? id) => Elements().FirstOrDefault(e => e.Id == id);
    public DiagramNode? FindNode(string id) => Nodes.FirstOrDefault(n => n.Id == id);
    internal IEnumerable<DiagramElement> Elements() => Nodes.Cast<DiagramElement>().Concat(Connections).Concat(Groups).Concat(Texts);

    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var element in Elements())
            if (!ids.Add(element.Id)) errors.Add($"Duplicate element ID: {element.Id}");
        var nodeIds = Nodes.Select(n => n.Id).ToHashSet(StringComparer.Ordinal);
        var groupIds = Groups.Select(g => g.Id).ToHashSet(StringComparer.Ordinal);
        foreach (var node in Nodes)
            if (node.GroupId.Length > 0 && !groupIds.Contains(node.GroupId)) errors.Add($"Node {node.Id}: group {node.GroupId} not found.");
        foreach (var link in Connections)
        {
            if (!nodeIds.Contains(link.SourceId)) errors.Add($"Connection {link.Id}: source {link.SourceId} not found.");
            if (!nodeIds.Contains(link.TargetId)) errors.Add($"Connection {link.Id}: target {link.TargetId} not found.");
            if (link.SourceId == link.TargetId && link.Waypoints.Count == 0) errors.Add($"Connection {link.Id}: self-loop requires explicit waypoints.");
        }
        return errors;
    }

    public Rect GetContentBounds()
    {
        Rect? result = null;
        foreach (var item in Elements().OfType<DiagramPositionedElement>().Where(e => e.IsVisible))
            result = result?.Union(item.Bounds) ?? item.Bounds;
        foreach (var point in Connections.Where(c => c.IsVisible).SelectMany(c => c.Waypoints))
        {
            var bounds = new Rect(point.X, point.Y, 1, 1);
            result = result?.Union(bounds) ?? bounds;
        }
        return result ?? new Rect(0, 0, 1, 1);
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var current = Elements().ToHashSet();
        foreach (var old in _subscribed.Except(current).ToArray()) { old.PropertyChanged -= OnElementChanged; _subscribed.Remove(old); }
        foreach (var item in current.Except(_subscribed)) { item.PropertyChanged += OnElementChanged; _subscribed.Add(item); }
        MarkChanged(geometry: true);
    }
    private void OnElementChanged(object? sender, PropertyChangedEventArgs e)
    {
        var geometry = sender is DiagramNode && e.PropertyName is nameof(DiagramNode.X) or nameof(DiagramNode.Y)
                or nameof(DiagramNode.Width) or nameof(DiagramNode.Height) or nameof(DiagramNode.IsVisible)
            || sender is DiagramConnection && e.PropertyName is nameof(DiagramConnection.SourceId) or nameof(DiagramConnection.TargetId)
                or nameof(DiagramConnection.SourceAnchor) or nameof(DiagramConnection.TargetAnchor)
                or nameof(DiagramConnection.SourceOffset) or nameof(DiagramConnection.TargetOffset) or nameof(DiagramConnection.Waypoints);
        MarkChanged(geometry);
    }
    private void MarkChanged(bool geometry = false)
    {
        if (_updateDepth > 0) { _pending = true; _pendingGeometry |= geometry; return; }
        Revision++;
        if (geometry) GeometryRevision++;
        Changed?.Invoke(this, EventArgs.Empty);
    }
    private sealed class UpdateScope(DiagramDocument owner) : IDisposable
    {
        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (--owner._updateDepth == 0 && owner._pending)
            {
                var geometry = owner._pendingGeometry;
                owner._pending = false; owner._pendingGeometry = false; owner.MarkChanged(geometry);
            }
        }
    }
    private sealed class ItemCollection<T> : ObservableCollection<T> where T : class
    {
        protected override void InsertItem(int index, T item) { ArgumentNullException.ThrowIfNull(item); base.InsertItem(index, item); }
        protected override void SetItem(int index, T item) { ArgumentNullException.ThrowIfNull(item); base.SetItem(index, item); }
    }
}

/// <summary>Versioned scene persistence, including routes and host-supplied presentation state.</summary>
public static class DiagramJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        Converters = { new JsonStringEnumConverter() },
    };
    public static string Serialize(DiagramDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        ThrowIfInvalid(document);
        return JsonSerializer.Serialize(new DiagramFile(1, document), Options);
    }
    public static DiagramDocument Deserialize(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);
        if (json.Length > 8_000_000) throw new JsonException("Diagram exceeds the 8 MB document limit.");
        var file = JsonSerializer.Deserialize<DiagramFile>(json, Options) ?? throw new JsonException("Empty diagram.");
        if (file.SchemaVersion != 1 || file.Document is null) throw new JsonException("Unsupported diagram schema.");
        ThrowIfInvalid(file.Document);
        return file.Document;
    }
    private static void ThrowIfInvalid(DiagramDocument document)
    {
        var errors = document.Validate();
        if (errors.Count > 0) throw new JsonException(string.Join(Environment.NewLine, errors));
    }
    private sealed record DiagramFile(int SchemaVersion, DiagramDocument Document);
}
