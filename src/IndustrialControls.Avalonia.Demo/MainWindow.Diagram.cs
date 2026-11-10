using Avalonia;
using Avalonia.Interactivity;
using IndustrialControls.Avalonia.Controls;
using IndustrialControls.Avalonia.Diagrams;

namespace IndustrialControls.Avalonia.Demo;

public sealed partial class MainWindow
{
    private DiagramDocument _plantDocument = null!;
    private bool _pumpFault;
    private void InitializePlantDiagram()
    {
        _plantDocument = PlantDiagramSample.Create();
        PlantDiagram.Document = _plantDocument;
        PlantDiagram.SelectionChanged += (_, e) => UpdateDiagramInspector(e.Element);
        PlantDiagram.ElementInvoked += (_, e) => UpdateDiagramInspector(e.Element);
        Opened += (_, _) => PlantDiagram.FitToContent();
    }
    private void UpdateDiagramInspector(DiagramElement? item)
    {
        DiagramSelection.Text = item is null ? "Seleziona un componente o una connessione" : $"{item.Label}  /  {item.Id}";
        DiagramDetails.Text = item switch
        {
            DiagramNode node => $"{node.State} · {node.ValueText}\n{node.DetailText}\n{node.Description}",
            DiagramConnection link => $"{link.Medium} · {link.SourceId} → {link.TargetId}\n{link.Label} · {link.ValueText}\nFLOW {link.FlowDirection}",
            null => "Ruota per lo zoom. Trascina lo sfondo per spostarti. Doppio clic o Invio per i dettagli.",
            _ => item.Description,
        };
        DiagramAlarmDetails.Text = item?.AlarmSeverity is > DiagramAlarmSeverity.None
            ? $"{item.AlarmSeverity} · {(item.IsAlarmAcknowledged ? "ACK" : "UNACK")}\n{item.AlarmText}" : "NESSUN ALLARME";
    }
    private void OnDiagramFit(object? sender, RoutedEventArgs e) => PlantDiagram.FitToContent();
    private void OnDiagramZoomIn(object? sender, RoutedEventArgs e) => PlantDiagram.ZoomAt(new Point(PlantDiagram.Bounds.Width / 2, PlantDiagram.Bounds.Height / 2), 1.2);
    private void OnDiagramZoomOut(object? sender, RoutedEventArgs e) => PlantDiagram.ZoomAt(new Point(PlantDiagram.Bounds.Width / 2, PlantDiagram.Bounds.Height / 2), 1 / 1.2);
    private void OnDiagramFault(object? sender, RoutedEventArgs e)
    {
        _pumpFault = !_pumpFault;
        using (_plantDocument.BeginUpdate())
        {
            var pump = _plantDocument.FindNode("MCP-01")!;
            pump.State = _pumpFault ? DiagramState.Trip : DiagramState.Active;
            pump.ValueText = _pumpFault ? "0 kg/s" : "480 kg/s";
            pump.DetailText = _pumpFault ? "TRIP / COMMAND BLOCKED" : "RUNNING / 1480 rpm";
            pump.AlarmSeverity = _pumpFault ? DiagramAlarmSeverity.Critical : DiagramAlarmSeverity.None;
            pump.IsAlarmAcknowledged = false;
            pump.AlarmText = _pumpFault ? "Main circulation pump tripped" : string.Empty;
            var link = (DiagramConnection)_plantDocument.FindElement("pump-core")!;
            link.FlowDirection = _pumpFault ? DiagramFlowDirection.None : DiagramFlowDirection.Forward;
            link.ValueText = _pumpFault ? "0 kg/s" : "480 kg/s";
        }
        PlantDiagram.SelectElement("MCP-01");
        UpdateDiagramInspector(_plantDocument.FindNode("MCP-01"));
    }
    private void OnDiagramAcknowledge(object? sender, RoutedEventArgs e)
    {
        var selected = _plantDocument.FindElement(PlantDiagram.SelectedElementId);
        if (selected is null || selected.AlarmSeverity == DiagramAlarmSeverity.None) return;
        selected.IsAlarmAcknowledged = true;
        UpdateDiagramInspector(selected);
    }
    private void OnDiagramReload(object? sender, RoutedEventArgs e)
    {
        // Exercise the same versioned layout round trip used by a consuming application.
        _plantDocument = DiagramJson.Deserialize(DiagramJson.Serialize(_plantDocument));
        PlantDiagram.Document = _plantDocument;
        UpdateDiagramInspector(_plantDocument.FindElement(PlantDiagram.SelectedElementId));
    }
}
