using IndustrialControls.Avalonia.Diagrams;

namespace IndustrialControls.Avalonia.Demo;

/// <summary>Illustrative plant topology and synthetic readouts. No simulator dependency or plant calculations.</summary>
internal static class PlantDiagramSample
{
    public static DiagramDocument Create()
    {
        var document = new DiagramDocument();
        using var batch = document.BeginUpdate();
        document.Groups.Add(new DiagramGroup("primary") { Label = "01 / REACTOR & PRIMARY CIRCUIT", X = 30, Y = 60, Width = 460, Height = 570 });
        document.Groups.Add(new DiagramGroup("secondary") { Label = "02 / STEAM & FEEDWATER", X = 620, Y = 60, Width = 490, Height = 570 });
        document.Groups.Add(new DiagramGroup("electrical") { Label = "03 / GENERATION & AUXILIARIES", X = 1140, Y = 60, Width = 440, Height = 570 });
        document.Texts.Add(new DiagramText("title") { Label = "PLANT OVERVIEW", X = 30, Y = 4, Width = 500, FontSize = 24 });
        document.Texts.Add(new DiagramText("subtitle") { Label = "WATER  /  STEAM  /  SHAFT  /  ELECTRICITY  /  CONTROL SIGNALS", X = 720, Y = 15, Width = 860, FontSize = 11 });

        Add("R-01", "REACTOR", DiagramSymbol.Reactor, 55, 165, "2850 MWt", "CORE / POWER STABLE", "primary", .78);
        Add("SD-01", "STEAM DRUM", DiagramSymbol.SteamDrum, 285, 165, "6.8 MPa", "LEVEL 65%", "primary", .65);
        Add("MCP-01", "CIRCULATION", DiagramSymbol.Pump, 285, 455, "480 kg/s", "RUNNING / 1480 rpm", "primary");
        Add("PROT-01", "PROTECTION", DiagramSymbol.Controller, 55, 325, "READY", "PERMISSIVES SATISFIED", "primary");
        Add("T-01", "TURBINE", DiagramSymbol.Turbine, 655, 165, "3000 rpm", "GOVERNOR / AUTO", "secondary");
        Add("COND-01", "CONDENSER", DiagramSymbol.Condenser, 655, 455, "4.8 kPa", "VACUUM STABLE", "secondary");
        Add("FWP-01", "FEEDWATER", DiagramSymbol.Pump, 895, 455, "310 kg/s", "RUNNING / AUTO", "secondary");
        Add("GEN-01", "GENERATOR", DiagramSymbol.Generator, 1180, 165, "950 MWe", "SYNCHRONIZED / 50 Hz", "electrical");
        Add("BUS-01", "GRID", DiagramSymbol.ElectricalBus, 1400, 165, "400 kV", "BREAKER CLOSED", "electrical", width: 150);
        Add("CW-01", "COOLING WATER", DiagramSymbol.Tank, 1180, 455, "16 °C", "SUPPLY AVAILABLE", "electrical", .82);
        var valve = Add("BV-01", "BYPASS", DiagramSymbol.Valve, 1400, 455, "35%", "POSITION DEVIATION", "electrical", width: 150);
        valve.State = DiagramState.Warning;
        valve.AlarmSeverity = DiagramAlarmSeverity.Caution;
        valve.AlarmText = "Requested position differs from feedback";

        Connect("core-steam", "R-01", "SD-01", DiagramMedium.Steam, "TWO-PHASE", "480 kg/s").LabelOffsetY = -74;
        Connect("main-steam", "SD-01", "T-01", DiagramMedium.Steam, "MAIN STEAM", "310 kg/s");
        Connect("drum-pump", "SD-01", "MCP-01", DiagramMedium.Water, "RECIRCULATION", "480 kg/s", DiagramAnchor.Bottom, DiagramAnchor.Top).LabelOffsetX = 78;
        var recirculation = Connect("pump-core", "MCP-01", "R-01", DiagramMedium.Water, "PRIMARY RETURN", "480 kg/s", DiagramAnchor.Left, DiagramAnchor.Bottom);
        Route(recirculation, (250, 507), (250, 300), (145, 300));
        recirculation.LabelOffsetX = 100;
        recirculation.LabelOffsetY = 45;
        var protection = Connect("protection-signal", "PROT-01", "R-01", DiagramMedium.Signal, "PROTECTION", "", DiagramAnchor.Top, DiagramAnchor.Bottom);
        protection.LabelOffsetX = -10;
        protection.FlowDirection = DiagramFlowDirection.None;
        Connect("shaft", "T-01", "GEN-01", DiagramMedium.Mechanical, "SHAFT", "3000 rpm");
        Connect("grid-output", "GEN-01", "BUS-01", DiagramMedium.Electrical, "GRID", "950 MWe").LabelOffsetY = -74;
        Connect("exhaust", "T-01", "COND-01", DiagramMedium.Steam, "EXHAUST", "4.8 kPa", DiagramAnchor.Bottom, DiagramAnchor.Top).LabelOffsetX = 67;
        Connect("condensate", "COND-01", "FWP-01", DiagramMedium.Water, "CONDENSATE", "310 kg/s").LabelOffsetY = 68;
        var feed = Connect("feedwater", "FWP-01", "SD-01", DiagramMedium.Water, "FEEDWATER RETURN", "310 kg/s", DiagramAnchor.Bottom, DiagramAnchor.Right);
        feed.TargetOffset = .75;
        Route(feed, (985, 700), (560, 700), (560, 300), (505, 300), (505, 243));
        var cooling = Connect("cooling", "CW-01", "COND-01", DiagramMedium.Water, "COOLING WATER", "16 °C", DiagramAnchor.Bottom, DiagramAnchor.Bottom);
        Route(cooling, (1270, 665), (745, 665));
        cooling.LabelOffsetX = 50;
        return document;

        DiagramNode Add(string id, string label, DiagramSymbol symbol, double x, double y, string value, string detail, string group, double? level = null, double width = 180)
        {
            var node = new DiagramNode(id) { Label = label, Symbol = symbol, X = x, Y = y, Width = width,
                ValueText = value, DetailText = detail, GroupId = group, State = DiagramState.Active, FillFraction = level,
                Description = $"{label} · illustrative process readout" };
            document.Nodes.Add(node);
            return node;
        }
        DiagramConnection Connect(string id, string from, string to, DiagramMedium medium, string label, string value,
            DiagramAnchor source = DiagramAnchor.Right, DiagramAnchor target = DiagramAnchor.Left)
        {
            var link = new DiagramConnection(id) { SourceId = from, TargetId = to, SourceAnchor = source, TargetAnchor = target,
                Medium = medium, Label = label, ValueText = value, FlowDirection = DiagramFlowDirection.Forward };
            document.Connections.Add(link);
            return link;
        }
        static void Route(DiagramConnection link, params (double X, double Y)[] points)
        {
            foreach (var point in points) link.Waypoints.Add(new DiagramPoint(point.X, point.Y));
        }
    }
}
