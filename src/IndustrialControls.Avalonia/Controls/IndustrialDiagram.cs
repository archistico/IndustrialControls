using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using IndustrialControls.Avalonia.Diagrams;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>A native, interactive process/architecture diagram with externally owned process and alarm state.</summary>
public sealed partial class IndustrialDiagram : Control
{
    public static readonly StyledProperty<DiagramDocument?> DocumentProperty = AvaloniaProperty.Register<IndustrialDiagram, DiagramDocument?>(nameof(Document));
    public static readonly StyledProperty<double> ZoomProperty = AvaloniaProperty.Register<IndustrialDiagram, double>(nameof(Zoom), 1, validate: v => double.IsFinite(v) && v is >= .1 and <= 4);
    public static readonly StyledProperty<Vector> PanOffsetProperty = AvaloniaProperty.Register<IndustrialDiagram, Vector>(nameof(PanOffset), validate: v => double.IsFinite(v.X) && double.IsFinite(v.Y));
    public static readonly StyledProperty<string?> SelectedElementIdProperty = AvaloniaProperty.Register<IndustrialDiagram, string?>(nameof(SelectedElementId), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<bool> IsEditingEnabledProperty = AvaloniaProperty.Register<IndustrialDiagram, bool>(nameof(IsEditingEnabled));
    public static readonly StyledProperty<bool> ShowGridProperty = AvaloniaProperty.Register<IndustrialDiagram, bool>(nameof(ShowGrid), true);
    public static readonly StyledProperty<bool> IsAnimationEnabledProperty = AvaloniaProperty.Register<IndustrialDiagram, bool>(nameof(IsAnimationEnabled), true);
    public static readonly StyledProperty<double> AnimationTimeProperty = AvaloniaProperty.Register<IndustrialDiagram, double>(nameof(AnimationTime), validate: v => double.IsFinite(v) && v >= 0);
    public static readonly StyledProperty<ICommand?> ElementInvokedCommandProperty = AvaloniaProperty.Register<IndustrialDiagram, ICommand?>(nameof(ElementInvokedCommand));
    public static readonly StyledProperty<IDiagramSymbolRenderer?> SymbolRendererProperty = AvaloniaProperty.Register<IndustrialDiagram, IDiagramSymbolRenderer?>(nameof(SymbolRenderer));

    private readonly DispatcherTimer _animationTimer;
    private readonly Stopwatch _animationClock = new();
    private DiagramDocument? _subscribedDocument;
    private long _routeRevision = -1;
    private DiagramDocument? _routeDocument;
    private readonly Dictionary<string, DiagramRoute> _routes = new(StringComparer.Ordinal);
    private Point? _dragOrigin;
    private Vector _panOrigin;
    private DiagramNode? _dragNode;
    private Point _nodeOrigin;
    private IPointer? _capturedPointer;
    private double _previousAnimationSeconds;
    private readonly List<Visual> _visibilityAncestors = new();

    static IndustrialDiagram()
    {
        AffectsRender<IndustrialDiagram>(DocumentProperty, ZoomProperty, PanOffsetProperty, ShowGridProperty, SelectedElementIdProperty, AnimationTimeProperty, SymbolRendererProperty, IsAnimationEnabledProperty, IsFocusedProperty);
        DocumentProperty.Changed.AddClassHandler<IndustrialDiagram>((c, _) => c.OnDocumentChanged());
        SelectedElementIdProperty.Changed.AddClassHandler<IndustrialDiagram>((c, _) => c.OnSelectionChanged());
        IsAnimationEnabledProperty.Changed.AddClassHandler<IndustrialDiagram>((c, _) => c.UpdateAnimation());
        IsVisibleProperty.Changed.AddClassHandler<IndustrialDiagram>((c, _) =>
        {
            c.UpdateAnimation();
            if (c.VisualRoot is not null) Dispatcher.UIThread.Post(c.UpdateAnimation);
        });
        IsEnabledProperty.Changed.AddClassHandler<IndustrialDiagram>((c, _) => { if (!c.IsEnabled) c.EndDrag(); c.UpdateAnimation(); });
        IsEditingEnabledProperty.Changed.AddClassHandler<IndustrialDiagram>((c, _) => { if (!c.IsEditingEnabled) c.EndDrag(); });
    }

    public IndustrialDiagram()
    {
        Focusable = true;
        ClipToBounds = true;
        LostFocus += (_, _) => EndDrag();
        _animationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(40) };
        _animationTimer.Tick += (_, _) =>
        {
            var now = _animationClock.Elapsed.TotalSeconds;
            SetCurrentValue(AnimationTimeProperty, AnimationTime + now - _previousAnimationSeconds);
            _previousAnimationSeconds = now;
            if (!IsEffectivelyVisible) UpdateAnimation();
        };
        Document = new DiagramDocument();
        RefreshAccessibility();
    }

    public DiagramDocument? Document { get => GetValue(DocumentProperty); set => SetValue(DocumentProperty, value); }
    public double Zoom { get => GetValue(ZoomProperty); set => SetCurrentValue(ZoomProperty, value); }
    public Vector PanOffset { get => GetValue(PanOffsetProperty); set => SetCurrentValue(PanOffsetProperty, value); }
    public string? SelectedElementId { get => GetValue(SelectedElementIdProperty); set => SetCurrentValue(SelectedElementIdProperty, value); }
    public bool IsEditingEnabled { get => GetValue(IsEditingEnabledProperty); set => SetValue(IsEditingEnabledProperty, value); }
    public bool ShowGrid { get => GetValue(ShowGridProperty); set => SetValue(ShowGridProperty, value); }
    public bool IsAnimationEnabled { get => GetValue(IsAnimationEnabledProperty); set => SetValue(IsAnimationEnabledProperty, value); }
    public double AnimationTime { get => GetValue(AnimationTimeProperty); set => SetValue(AnimationTimeProperty, value); }
    public ICommand? ElementInvokedCommand { get => GetValue(ElementInvokedCommandProperty); set => SetValue(ElementInvokedCommandProperty, value); }
    public IDiagramSymbolRenderer? SymbolRenderer { get => GetValue(SymbolRendererProperty); set => SetValue(SymbolRendererProperty, value); }
    public event EventHandler<DiagramElementEventArgs>? SelectionChanged;
    public event EventHandler<DiagramElementEventArgs>? ElementInvoked;

    public Point ToDiagram(Point point) => new((point.X - PanOffset.X) / Zoom, (point.Y - PanOffset.Y) / Zoom);
    public Point ToViewport(Point point) => new(point.X * Zoom + PanOffset.X, point.Y * Zoom + PanOffset.Y);
    public void ZoomAt(Point viewportPoint, double factor)
    {
        if (!double.IsFinite(factor) || factor <= 0) throw new ArgumentOutOfRangeException(nameof(factor));
        var anchor = ToDiagram(viewportPoint);
        Zoom = Math.Clamp(Zoom * factor, .1, 4);
        PanOffset = new Vector(viewportPoint.X - anchor.X * Zoom, viewportPoint.Y - anchor.Y * Zoom);
    }
    public void FitToContent(double margin = 32)
    {
        if (!double.IsFinite(margin) || margin < 0) throw new ArgumentOutOfRangeException(nameof(margin));
        if (Document is null || Bounds.Width <= margin * 2 || Bounds.Height <= margin * 2) return;
        var content = Document.GetContentBounds();
        Zoom = Math.Clamp(Math.Min((Bounds.Width - 2 * margin) / content.Width, (Bounds.Height - 2 * margin) / content.Height), .1, 4);
        PanOffset = new Vector(Bounds.Width / 2 - content.Center.X * Zoom, Bounds.Height / 2 - content.Center.Y * Zoom);
    }
    public bool SelectElement(string? id)
    {
        if (id is not null && Document?.FindElement(id) is not { IsVisible: true }) return false;
        SelectedElementId = id;
        return true;
    }
    public bool InvokeSelectedElement()
    {
        if (!IsEffectivelyEnabled || Document?.FindElement(SelectedElementId) is not { IsVisible: true } element) return false;
        var command = ElementInvokedCommand;
        if (command is not null && !command.CanExecute(element)) return false;
        ElementInvoked?.Invoke(this, new DiagramElementEventArgs(element));
        command?.Execute(element);
        return true;
    }
    public IReadOnlyList<Point> GetConnectionRoute(string connectionId)
    {
        EnsureRoutes();
        return _routes.TryGetValue(connectionId, out var route) ? Array.AsReadOnly(route.Points) : Array.Empty<Point>();
    }
    public DiagramElement? HitTestElement(Point viewportPoint)
    {
        var point = ToDiagram(viewportPoint);
        if (Document is null) return null;
        foreach (var node in Document.Nodes.Reverse()) if (node.IsVisible && node.Bounds.Contains(point)) return node;
        foreach (var text in Document.Texts.Reverse()) if (text.IsVisible && text.Bounds.Contains(point)) return text;
        EnsureRoutes();
        foreach (var link in Document.Connections.Reverse())
            if (link.IsVisible && _routes.TryGetValue(link.Id, out var route))
            {
                if (route.IsNear(point, Math.Max(6 / Zoom, link.Thickness))) return link;
                var text = link.Label + (link.ValueText.Length > 0 ? "  ·  " + link.ValueText : string.Empty);
                var at = route.At(route.Length / 2).Point + new Vector(link.LabelOffsetX, link.LabelOffsetY);
                var width = Math.Min(360, text.Length * 6);
                if (text.Length > 0 && new Rect(at.X - width / 2 - 6, at.Y - 4, width + 12, 24).Contains(point)) return link;
            }
        return Document.Groups.Reverse().FirstOrDefault(g => g.IsVisible && new Rect(g.X, g.Y, g.Width, 30).Contains(point));
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Cursor ??= new Cursor(StandardCursorType.Cross);
        foreach (var ancestor in this.GetVisualAncestors())
        {
            _visibilityAncestors.Add(ancestor);
            ancestor.PropertyChanged += OnAncestorPropertyChanged;
        }
        SubscribeDocument();
        OnModelChanged(this, EventArgs.Empty);
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        EndDrag(); UnsubscribeDocument(); _animationTimer.Stop(); _animationClock.Stop();
        foreach (var ancestor in _visibilityAncestors) ancestor.PropertyChanged -= OnAncestorPropertyChanged;
        _visibilityAncestors.Clear();
        base.OnDetachedFromVisualTree(e);
    }
    private void OnAncestorPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == IsVisibleProperty || e.Property == IsEnabledProperty)
            Dispatcher.UIThread.Post(UpdateAnimation);
    }
    private void OnDocumentChanged()
    {
        EndDrag();
        UnsubscribeDocument();
        if (VisualRoot is not null) SubscribeDocument();
        if (SelectedElementId is not null && Document?.FindElement(SelectedElementId) is null) SelectedElementId = null;
        _routeRevision = -1;
        _textCache.Clear();
        RefreshAccessibility();
        UpdateAnimation();
    }
    private void SubscribeDocument()
    {
        if (_subscribedDocument == Document) return;
        UnsubscribeDocument();
        _subscribedDocument = Document;
        if (_subscribedDocument is not null) _subscribedDocument.Changed += OnModelChanged;
    }
    private void UnsubscribeDocument()
    {
        if (_subscribedDocument is not null) _subscribedDocument.Changed -= OnModelChanged;
        _subscribedDocument = null;
    }
    private void OnModelChanged(object? sender, EventArgs e)
    {
        if (SelectedElementId is not null && Document?.FindElement(SelectedElementId) is not { IsVisible: true }) SelectedElementId = null;
        InvalidateVisual();
        RefreshAccessibility();
        UpdateAnimation();
    }
    private void OnSelectionChanged()
    {
        RefreshAccessibility();
        SelectionChanged?.Invoke(this, new DiagramElementEventArgs(Document?.FindElement(SelectedElementId)));
    }
    private void RefreshAccessibility()
    {
        var item = Document?.FindElement(SelectedElementId);
        IndustrialAutomationMetadata.Apply(this, "Process diagram", item is null
            ? "Arrow keys select equipment. Enter opens details. Home fits the diagram. Plus/minus zoom."
            : $"{item.Label}; {item.State}; {(item is DiagramNode n ? n.ValueText : item is DiagramConnection c ? c.ValueText : string.Empty)}; "
                + $"{item.AlarmSeverity}; {(item.IsAlarmAcknowledged ? "ACK" : "UNACK")}; {item.AlarmText}; {item.Description}", "IndustrialDiagram");
    }
    private void UpdateAnimation()
    {
        if (_animationTimer is null) return;
        var animate = VisualRoot is not null && IsEffectivelyVisible && IsEffectivelyEnabled && IsAnimationEnabled && Document is not null
            && (Document.Connections.Any(c => c.IsVisible && c.FlowDirection != DiagramFlowDirection.None && c.FlowSpeed > 0 && c.State != DiagramState.Unavailable)
                || Document.Elements().Any(e => e.IsVisible && e.AlarmSeverity != DiagramAlarmSeverity.None && !e.IsAlarmAcknowledged));
        if (animate && !_animationTimer.IsEnabled) { _animationClock.Restart(); _previousAnimationSeconds = 0; _animationTimer.Start(); }
        else if (!animate) { _animationTimer.Stop(); _animationClock.Stop(); }
    }
    private void EnsureRoutes()
    {
        if (_routeDocument == Document && _routeRevision == (Document?.GeometryRevision ?? -1)) return;
        _routeDocument = Document; _routeRevision = Document?.GeometryRevision ?? -1;
        _routes.Clear();
        if (Document is null) return;
        var nodes = new Dictionary<string, DiagramNode>(StringComparer.Ordinal);
        foreach (var node in Document.Nodes) nodes.TryAdd(node.Id, node);
        foreach (var link in Document.Connections)
            if (nodes.TryGetValue(link.SourceId, out var source) && nodes.TryGetValue(link.TargetId, out var target) && source.IsVisible && target.IsVisible)
                _routes.TryAdd(link.Id, DiagramRoute.Create(link, source, target));
    }
}

public sealed class DiagramElementEventArgs(DiagramElement? element) : EventArgs
{
    public DiagramElement? Element { get; } = element;
}
