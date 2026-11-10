using System.Text.Json.Serialization;

namespace IndustrialControls.Avalonia.Diagrams;

public sealed class DiagramNode : DiagramPositionedElement
{
    private DiagramSymbol _symbol;
    private string _symbolKey = string.Empty;
    private string _valueText = string.Empty, _detailText = string.Empty, _groupId = string.Empty;
    private double? _fillFraction;
    [JsonConstructor] public DiagramNode(string id) : base(id) { }
    public DiagramSymbol Symbol { get => _symbol; set => Set(ref _symbol, Defined(value)); }
    public string SymbolKey { get => _symbolKey; set => Set(ref _symbolKey, value ?? string.Empty); }
    public string ValueText { get => _valueText; set => Set(ref _valueText, value ?? string.Empty); }
    public string DetailText { get => _detailText; set => Set(ref _detailText, value ?? string.Empty); }
    public string GroupId { get => _groupId; set => Set(ref _groupId, value ?? string.Empty); }
    /// <summary>Optional host-supplied vessel level; null means no level is displayed.</summary>
    public double? FillFraction
    {
        get => _fillFraction;
        set => Set(ref _fillFraction, value is null || double.IsFinite(value.Value) && value is >= 0 and <= 1
            ? value : throw new ArgumentOutOfRangeException(nameof(value)));
    }
}

public sealed class DiagramGroup : DiagramPositionedElement
{
    [JsonConstructor] public DiagramGroup(string id) : base(id) { Width = 500; Height = 300; }
}

public sealed class DiagramText : DiagramPositionedElement
{
    private double _fontSize = 14;
    [JsonConstructor] public DiagramText(string id) : base(id) { Width = 240; Height = 40; }
    public double FontSize { get => _fontSize; set => Set(ref _fontSize, Positive(value)); }
}
