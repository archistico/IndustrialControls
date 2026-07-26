using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Demo;

public sealed partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private double _simulationTime;
    private int _qualityIndex;

    public MainWindow()
    {
        InitializeComponent();

        ProcessTrend.AddSeries(
            "POWER",
            "MWe",
            Color.Parse("#58D46C"));
        ProcessTrend.AddSeries(
            "PRESSURE",
            "MPa",
            Color.Parse("#57A8E8"));
        ProcessTrend.AddSeries(
            "FREQUENCY Δ",
            "Hz",
            Color.Parse("#E3C83B"));

        Recorder.AddSeries(
            "DRUM LEVEL",
            "%",
            Color.Parse("#2D6F3F"));
        Recorder.AddSeries(
            "VALVE POSITION",
            "%",
            Color.Parse("#8C4D2C"));

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };

        _timer.Tick += OnSimulationTick;
        _timer.Start();

        AddSimulationSamples();
    }

    protected override void OnClosed(EventArgs e)
    {
        _timer.Stop();
        _timer.Tick -= OnSimulationTick;
        base.OnClosed(e);
    }

    private void OnSimulationTick(object? sender, EventArgs e)
    {
        _simulationTime += 0.1;
        AddSimulationSamples();
    }

    private void AddSimulationSamples()
    {
        var quality = CurrentQuality;

        var power =
            5.0 +
            (0.55 * Math.Sin(_simulationTime * 0.32)) +
            (0.08 * Math.Sin(_simulationTime * 1.7));

        var pressure =
            6.8 +
            (0.25 * Math.Sin((_simulationTime * 0.21) + 0.9));

        var frequencyDeviation =
            5.0 +
            (0.35 * Math.Sin((_simulationTime * 0.75) + 1.5));

        ProcessTrend.AddSample(
            "POWER",
            _simulationTime,
            power,
            quality);
        ProcessTrend.AddSample(
            "PRESSURE",
            _simulationTime,
            pressure,
            quality == SignalQuality.Bad
                ? SignalQuality.Uncertain
                : quality);
        ProcessTrend.AddSample(
            "FREQUENCY Δ",
            _simulationTime,
            frequencyDeviation,
            SignalQuality.Good);

        Recorder.AddSample(
            "DRUM LEVEL",
            _simulationTime,
            52.0 + (8.0 * Math.Sin(_simulationTime * 0.18)),
            quality);
        Recorder.AddSample(
            "VALVE POSITION",
            _simulationTime,
            48.0 + (12.0 * Math.Sin((_simulationTime * 0.24) + 1.1)),
            SignalQuality.Good);

        Scope.SetSamples(
            Enumerable.Range(0, 192)
                .Select(index =>
                {
                    var phase = index / 191.0;
                    return
                        (0.72 * Math.Sin((phase * Math.PI * 6.0) + _simulationTime)) +
                        (0.12 * Math.Sin((phase * Math.PI * 18.0) - _simulationTime));
                }));

        Scope.Quality = quality;
        CursorReadoutDisplay.Text = ProcessTrend.CursorReadout;
    }

    private SignalQuality CurrentQuality =>
        _qualityIndex switch
        {
            1 => SignalQuality.Uncertain,
            2 => SignalQuality.Bad,
            3 => SignalQuality.Unavailable,
            _ => SignalQuality.Good
        };

    private void OnToggleRun(object? sender, RoutedEventArgs e)
    {
        Recorder.IsRunning = !Recorder.IsRunning;

        StatusDisplay.Text = Recorder.IsRunning
            ? "STRIP-CHART RECORDER RUNNING"
            : "STRIP-CHART RECORDER PAUSED";

        StatusDisplay.LedColor = Recorder.IsRunning
            ? LedDisplayColor.Green
            : LedDisplayColor.Amber;
    }

    private void OnCycleQuality(object? sender, RoutedEventArgs e)
    {
        _qualityIndex = (_qualityIndex + 1) % 4;
        var quality = CurrentQuality;

        PowerQuality.Quality = quality;
        PressureQuality.Quality =
            quality == SignalQuality.Bad
                ? SignalQuality.Uncertain
                : quality;
        FrequencyQuality.Quality = SignalQuality.Good;
        Scope.Quality = quality;

        StatusDisplay.Text =
            string.Concat("POWER SIGNAL QUALITY: ", quality.ToString().ToUpperInvariant());

        StatusDisplay.LedColor = quality switch
        {
            SignalQuality.Good => LedDisplayColor.Green,
            SignalQuality.Uncertain => LedDisplayColor.Amber,
            SignalQuality.Bad => LedDisplayColor.Red,
            _ => LedDisplayColor.Blue
        };
    }

    private void OnCursorLeft(object? sender, RoutedEventArgs e)
    {
        ProcessTrend.CursorFraction =
            Math.Max(0, ProcessTrend.CursorFraction - 0.05);
        CursorReadoutDisplay.Text = ProcessTrend.CursorReadout;
    }

    private void OnCursorRight(object? sender, RoutedEventArgs e)
    {
        ProcessTrend.CursorFraction =
            Math.Min(1, ProcessTrend.CursorFraction + 0.05);
        CursorReadoutDisplay.Text = ProcessTrend.CursorReadout;
    }

    private void OnClearHistory(object? sender, RoutedEventArgs e)
    {
        ProcessTrend.ClearSamples();
        Recorder.ClearSamples();
        Scope.ClearSamples();
        CursorReadoutDisplay.Text = ProcessTrend.CursorReadout;

        StatusDisplay.Text = "ALL SIGNAL HISTORY CLEARED";
        StatusDisplay.LedColor = LedDisplayColor.Red;
    }
}
