using System.Text.Json;
using System.Windows.Input;
using Avalonia;
using IndustrialControls.Avalonia.Controls;
using IndustrialControls.Avalonia.Diagrams;

namespace IndustrialControls.Avalonia.Tests;

public sealed class IndustrialDiagramTests
{
    [Fact]
    public void BatchedSnapshotPublishesOnceAndNeverCreatesLocalAlarmMemory()
    {
        var document = CreateDocument();
        var changes = 0;
        document.Changed += (_, _) => changes++;
        var node = document.Nodes[0];
        using (document.BeginUpdate())
        {
            node.AlarmSeverity = DiagramAlarmSeverity.Critical;
            node.AlarmText = "PUMP TRIP";
            node.IsAlarmAcknowledged = true;
            using (document.BeginUpdate()) node.State = DiagramState.Trip;
            Assert.Equal(0, changes);
        }
        Assert.Equal(1, changes);
        using (document.BeginUpdate()) { node.AlarmSeverity = DiagramAlarmSeverity.None; node.AlarmText = ""; }
        Assert.Equal(DiagramAlarmSeverity.None, node.AlarmSeverity);
        Assert.True(node.IsAlarmAcknowledged);
        Assert.Equal(2, changes);
    }

    [Fact]
    public void RemovedElementsAreUnsubscribedAndUnchangedPropertiesDoNotNotify()
    {
        var document = CreateDocument();
        var changes = 0;
        document.Changed += (_, _) => changes++;
        var node = document.Nodes[0];
        node.Label = node.Label;
        Assert.Equal(0, changes);
        document.Nodes.Clear();
        changes = 0;
        node.ValueText = "STALE";
        Assert.Equal(0, changes);
    }

    [Fact]
    public void JsonRoundTripPreservesTopologyRoutesGroupsAlarmAndFill()
    {
        var document = CreateDocument();
        document.Groups.Add(new DiagramGroup("primary") { X = -100, Label = "PRIMARY" });
        document.Nodes[0].GroupId = "primary";
        document.Nodes[0].FillFraction = .62;
        document.Nodes[0].AlarmSeverity = DiagramAlarmSeverity.Warning;
        document.Nodes[0].IsAlarmAcknowledged = true;
        document.Texts.Add(new DiagramText("note") { Label = "MEASURED", FontSize = 18 });
        document.Connections[0].Waypoints.Add(new DiagramPoint(-30, 220));
        var restored = DiagramJson.Deserialize(DiagramJson.Serialize(document));
        Assert.Empty(restored.Validate());
        Assert.Equal(2, restored.Nodes.Count);
        Assert.Equal("primary", Assert.Single(restored.Groups).Id);
        Assert.Equal(.62, restored.Nodes[0].FillFraction);
        Assert.True(restored.Nodes[0].IsAlarmAcknowledged);
        Assert.Equal(DiagramAlarmSeverity.Warning, restored.Nodes[0].AlarmSeverity);
        Assert.Equal(new DiagramPoint(-30, 220), Assert.Single(restored.Connections[0].Waypoints));
        Assert.Equal("MEASURED", Assert.Single(restored.Texts).Label);
        var changed = false;
        restored.Changed += (_, _) => changed = true;
        restored.Nodes[0].ValueText = "RESTORED";
        Assert.True(changed);
    }

    [Fact]
    public void InvalidTopologyAndUnsupportedSchemasAreReported()
    {
        var document = CreateDocument();
        document.Nodes.Add(new DiagramNode("a"));
        document.Connections[0].TargetId = "missing";
        Assert.Equal(2, document.Validate().Count);
        Assert.Throws<JsonException>(() => DiagramJson.Serialize(document));
        Assert.Throws<JsonException>(() => DiagramJson.Deserialize("{\"SchemaVersion\":2,\"Document\":{}}"));
        Assert.Throws<JsonException>(() => DiagramJson.Deserialize("{\"SchemaVersion\":1,\"Document\":{},\"Unexpected\":42}"));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void NonFiniteGeometryIsRejected(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiagramNode("a") { X = value });
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiagramPoint(value, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiagramConnection("e") { FlowSpeed = value });
    }

    [Fact]
    public void InvalidDimensionsFractionsEnumsAndNullItemsAreRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiagramNode("a") { Width = 0 });
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiagramNode("a") { FillFraction = 2 });
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiagramConnection("e") { SourceOffset = -.1 });
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiagramConnection("e") { Medium = (DiagramMedium)999 });
        Assert.Throws<ArgumentNullException>(() => new DiagramDocument().Nodes.Add(null!));
    }

    [Fact]
    public void ZoomPreservesPointerAnchorAndHitTestingUsesViewportCoordinates()
    {
        var diagram = new IndustrialDiagram { Document = CreateDocument(), PanOffset = new Vector(50, 30) };
        var pointer = new Point(100, 70);
        var anchor = diagram.ToDiagram(pointer);
        diagram.ZoomAt(pointer, 2);
        Assert.Equal(anchor, diagram.ToDiagram(pointer));
        Assert.Equal("a", diagram.HitTestElement(diagram.ToViewport(new Point(30, 30)))?.Id);
        Assert.Equal(new Point(30, 30), diagram.ToDiagram(diagram.ToViewport(new Point(30, 30))));
    }

    [Fact]
    public void FitToContentHandlesNegativeCoordinatesAndDoesNotAlterModel()
    {
        var document = CreateDocument();
        document.Nodes[0].X = -100;
        var diagram = new IndustrialDiagram { Document = document };
        diagram.Arrange(new Rect(0, 0, 800, 500));
        diagram.FitToContent();
        var content = document.GetContentBounds();
        Assert.InRange(diagram.ToViewport(content.TopLeft).X, 31.99, 800);
        Assert.InRange(diagram.ToViewport(content.BottomRight).X, 0, 768.01);
        Assert.Equal(-100, document.Nodes[0].X);
    }

    [Fact]
    public void AutomaticRoutesAreOrthogonalAndRecomputeAfterNodeMove()
    {
        var document = CreateDocument();
        var diagram = new IndustrialDiagram { Document = document, IsEditingEnabled = true };
        var before = diagram.GetConnectionRoute("e").ToArray();
        Assert.Equal(new Point(180, 52), before[0]);
        Assert.Equal(new Point(360, 52), before[^1]);
        for (var i = 1; i < before.Length; i++) Assert.True(before[i].X == before[i - 1].X || before[i].Y == before[i - 1].Y);
        Assert.True(diagram.TryMoveNode("b", 400, 100));
        var after = diagram.GetConnectionRoute("e");
        Assert.Equal(new Point(400, 152), after[^1]);
        Assert.Equal("e", diagram.HitTestElement(diagram.ToViewport(new Point(220, 52)))?.Id);
    }

    [Fact]
    public void ExplicitRoutesAndAnchorOffsetsArePreserved()
    {
        var document = CreateDocument();
        document.Connections[0].SourceOffset = .25;
        document.Connections[0].Waypoints.Add(new DiagramPoint(230, -50));
        var diagram = new IndustrialDiagram { Document = document };
        var route = diagram.GetConnectionRoute("e");
        Assert.Equal(new Point(180, 26), route[0]);
        Assert.Contains(new Point(230, -50), route);
        Assert.Equal(new Point(360, 52), route[^1]);
    }

    [Fact]
    public void EditingIsOptInAndProcessUpdatesDoNotChangeSelectionOrViewport()
    {
        var document = CreateDocument();
        var diagram = new IndustrialDiagram { Document = document, Zoom = 1.5, PanOffset = new Vector(20, 10) };
        Assert.False(diagram.TryMoveNode("a", 100, 100));
        Assert.True(diagram.SelectElement("a"));
        document.Nodes[0].ValueText = "100 MWe";
        document.Nodes[0].State = DiagramState.Trip;
        Assert.Equal("a", diagram.SelectedElementId);
        Assert.Equal(1.5, diagram.Zoom);
        Assert.Equal(new Vector(20, 10), diagram.PanOffset);
        Assert.Equal(0, document.Nodes[0].X);
    }

    [Fact]
    public void InvocationRespectsCommandAndDisabledStateAndDoesNotAcknowledgeAlarms()
    {
        var diagram = new IndustrialDiagram { Document = CreateDocument() };
        diagram.Document.Nodes[0].AlarmSeverity = DiagramAlarmSeverity.Critical;
        diagram.SelectElement("a");
        var command = new TestCommand();
        diagram.ElementInvokedCommand = command;
        Assert.False(diagram.InvokeSelectedElement());
        command.Allowed = true;
        Assert.True(diagram.InvokeSelectedElement());
        Assert.Equal("a", Assert.IsType<DiagramNode>(command.Parameter).Id);
        Assert.False(diagram.Document.Nodes[0].IsAlarmAcknowledged);
        diagram.IsEnabled = false;
        Assert.False(diagram.InvokeSelectedElement());
    }

    [Fact]
    public void LiveReadoutsAndAlarmsDoNotInvalidateGeometryButAnchorChangesDo()
    {
        var document = CreateDocument();
        var geometry = document.GeometryRevision;
        using (document.BeginUpdate())
        {
            document.Nodes[0].ValueText = "100 MWe";
            document.Nodes[0].AlarmSeverity = DiagramAlarmSeverity.Critical;
            document.Connections[0].ValueText = "100 kg/s";
            document.Connections[0].FlowDirection = DiagramFlowDirection.Reverse;
        }
        Assert.Equal(geometry, document.GeometryRevision);
        document.Connections[0].SourceOffset = .25;
        Assert.True(document.GeometryRevision > geometry);
        var diagram = new IndustrialDiagram { Document = document };
        Assert.Equal(new Point(180, 26), diagram.GetConnectionRoute("e")[0]);
    }

    [Fact]
    public void MissingConnectionsDoNotCrashAndTwoInstancesDoNotShareDocuments()
    {
        var a = new IndustrialDiagram();
        var b = new IndustrialDiagram();
        Assert.NotSame(a.Document, b.Document);
        a.Document!.Connections.Add(new DiagramConnection("broken"));
        Assert.Empty(a.GetConnectionRoute("broken"));
        Assert.False(a.SelectElement("missing"));
        Assert.Null(a.HitTestElement(new Point(1, 1)));
    }

    private static DiagramDocument CreateDocument()
    {
        var document = new DiagramDocument();
        using var batch = document.BeginUpdate();
        document.Nodes.Add(new DiagramNode("a") { Label = "Pump", Symbol = DiagramSymbol.Pump });
        document.Nodes.Add(new DiagramNode("b") { Label = "Tank", X = 360, Symbol = DiagramSymbol.Tank });
        document.Connections.Add(new DiagramConnection("e") { SourceId = "a", TargetId = "b", SourceAnchor = DiagramAnchor.Right, TargetAnchor = DiagramAnchor.Left });
        return document;
    }
    private sealed class TestCommand : ICommand
    {
        public bool Allowed { get; set; }
        public object? Parameter { get; private set; }
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => Allowed;
        public void Execute(object? parameter) => Parameter = parameter;
    }
}
