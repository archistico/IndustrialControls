using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Avalonia;

namespace IndustrialControls.Avalonia.Diagrams;

public enum DiagramState { Normal, Active, Warning, Trip, Unavailable }
public enum DiagramAlarmSeverity { None, Advisory, Caution, Warning, Critical }
public enum DiagramSymbol { Equipment, Reactor, Pump, Valve, Tank, SteamDrum, HeatExchanger, Turbine, Generator, Condenser, ElectricalBus, Sensor, Controller, Junction }
public enum DiagramMedium { Water, Steam, Gas, Electrical, Signal, Mechanical, Generic }
public enum DiagramAnchor { Auto, Left, Top, Right, Bottom }
public enum DiagramFlowDirection { None, Forward, Reverse }

/// <summary>Presentation state supplied by the host. Alarm properties contain no local ACK/latch logic.</summary>
public abstract class DiagramElement : INotifyPropertyChanged
{
    private string _label = string.Empty;
    private string _description = string.Empty;
    private DiagramState _state;
    private DiagramAlarmSeverity _alarmSeverity;
    private bool _isAlarmAcknowledged;
    private string _alarmText = string.Empty;
    private bool _isVisible = true;

    protected DiagramElement(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        Id = id;
    }

    public string Id { get; }
    public string Label { get => _label; set => Set(ref _label, value ?? string.Empty); }
    public string Description { get => _description; set => Set(ref _description, value ?? string.Empty); }
    public DiagramState State { get => _state; set => Set(ref _state, Defined(value)); }
    public DiagramAlarmSeverity AlarmSeverity { get => _alarmSeverity; set => Set(ref _alarmSeverity, Defined(value)); }
    public bool IsAlarmAcknowledged { get => _isAlarmAcknowledged; set => Set(ref _isAlarmAcknowledged, value); }
    public string AlarmText { get => _alarmText; set => Set(ref _alarmText, value ?? string.Empty); }
    public bool IsVisible { get => _isVisible; set => Set(ref _isVisible, value); }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    internal static double Finite(double value)
        => double.IsFinite(value) ? value : throw new ArgumentOutOfRangeException(nameof(value), "A finite coordinate is required.");
    internal static double Positive(double value)
        => Finite(value) > 0 ? value : throw new ArgumentOutOfRangeException(nameof(value), "A positive size is required.");
    protected static T Defined<T>(T value) where T : struct, Enum
        => Enum.IsDefined(value) ? value : throw new ArgumentOutOfRangeException(nameof(value));
}

public abstract class DiagramPositionedElement : DiagramElement
{
    private double _x, _y;
    private double _width = 180, _height = 104;
    protected DiagramPositionedElement(string id) : base(id) { }
    public double X { get => _x; set => Set(ref _x, Finite(value)); }
    public double Y { get => _y; set => Set(ref _y, Finite(value)); }
    public double Width { get => _width; set => Set(ref _width, Positive(value)); }
    public double Height { get => _height; set => Set(ref _height, Positive(value)); }
    [JsonIgnore] public Rect Bounds => new(X, Y, Width, Height);
}
