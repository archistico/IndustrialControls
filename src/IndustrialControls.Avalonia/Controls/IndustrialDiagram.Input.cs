using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using IndustrialControls.Avalonia.Diagrams;

namespace IndustrialControls.Avalonia.Controls;

public sealed partial class IndustrialDiagram
{
    public bool TryMoveNode(string nodeId, double x, double y)
    {
        if (!IsEditingEnabled || !IsEffectivelyEnabled || Document?.FindNode(nodeId) is not { IsVisible: true } node) return false;
        DiagramElement.Finite(x); DiagramElement.Finite(y);
        using (Document.BeginUpdate()) { node.X = x; node.Y = y; }
        return true;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        var buttons = e.GetCurrentPoint(this).Properties;
        if (!buttons.IsLeftButtonPressed && !buttons.IsMiddleButtonPressed) return;
        Focus();
        var position = e.GetPosition(this);
        var element = HitTestElement(position);
        if (buttons.IsLeftButtonPressed)
        {
            SelectElement(element?.Id);
            if (e.ClickCount == 2 && element is not null) { InvokeSelectedElement(); e.Handled = true; return; }
        }
        _dragNode = buttons.IsLeftButtonPressed && IsEditingEnabled ? element as DiagramNode : null;
        if (_dragNode is not null || buttons.IsMiddleButtonPressed || element is null)
        {
            _dragOrigin = position;
            _panOrigin = PanOffset;
            _nodeOrigin = _dragNode is null ? default : new Point(_dragNode.X, _dragNode.Y);
            _capturedPointer = e.Pointer;
            e.Pointer.Capture(this);
        }
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var position = e.GetPosition(this);
        if (_dragOrigin is { } origin)
        {
            var delta = position - origin;
            if (_dragNode is not null) TryMoveNode(_dragNode.Id, _nodeOrigin.X + delta.X / Zoom, _nodeOrigin.Y + delta.Y / Zoom);
            else PanOffset = _panOrigin + delta;
            e.Handled = true;
        }
        else
        {
            var element = HitTestElement(position);
            ToolTip.SetTip(this, element is null ? null : string.Join("\n", new[] {
                $"{element.Label} · {element.Id}", element.Description,
                element is DiagramNode n ? n.ValueText : element is DiagramConnection c ? c.ValueText : string.Empty,
                element.AlarmSeverity == DiagramAlarmSeverity.None ? string.Empty
                    : $"{element.AlarmSeverity} · {(element.IsAlarmAcknowledged ? "ACK" : "UNACK")} · {element.AlarmText}",
            }.Where(s => s.Length > 0)));
        }
    }
    protected override void OnPointerReleased(PointerReleasedEventArgs e) { EndDrag(); base.OnPointerReleased(e); }
    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e) { EndDrag(); base.OnPointerCaptureLost(e); }
    private void EndDrag()
    {
        var pointer = _capturedPointer;
        _capturedPointer = null;
        _dragOrigin = null;
        _dragNode = null;
        pointer?.Capture(null);
    }
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        EndDrag();
        ZoomAt(e.GetPosition(this), Math.Pow(1.15, Math.Clamp(e.Delta.Y, -5, 5)));
        e.Handled = true;
        base.OnPointerWheelChanged(e);
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
        switch (e.Key)
        {
            case Key.Home: FitToContent(); break;
            case Key.Add: case Key.OemPlus: ZoomAt(center, 1.2); break;
            case Key.Subtract: case Key.OemMinus: ZoomAt(center, 1 / 1.2); break;
            case Key.Escape: EndDrag(); SelectElement(null); break;
            case Key.Enter: InvokeSelectedElement(); break;
            case Key.Right: case Key.Down: SelectAdjacent(1); break;
            case Key.Left: case Key.Up: SelectAdjacent(-1); break;
            default: base.OnKeyDown(e); return;
        }
        e.Handled = true;
    }
    private void SelectAdjacent(int direction)
    {
        var nodes = Document?.Nodes.Where(n => n.IsVisible).ToArray() ?? [];
        if (nodes.Length == 0) return;
        var index = Array.FindIndex(nodes, n => n.Id == SelectedElementId);
        var next = index < 0 ? direction > 0 ? 0 : nodes.Length - 1 : (index + direction + nodes.Length) % nodes.Length;
        SelectElement(nodes[next].Id);
        var point = ToViewport(nodes[next].Bounds.Center);
        if (!new Rect(Bounds.Size).Deflate(30).Contains(point))
            PanOffset = new Vector(Bounds.Width / 2 - nodes[next].Bounds.Center.X * Zoom, Bounds.Height / 2 - nodes[next].Bounds.Center.Y * Zoom);
    }
}
