using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Demo;

public sealed partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private readonly double[] _scopeSamples = new double[192];

    private readonly SignalTraceSeries _powerSeries;
    private readonly SignalTraceSeries _pressureSeries;
    private readonly SignalTraceSeries _frequencySeries;
    private readonly SignalTraceSeries _levelSeries;
    private readonly SignalTraceSeries _valveSeries;

    private double _simulationTime;
    private int _qualityIndex;
    private int _timerTicks;

    public MainWindow()
    {
        InitializeComponent();

        _powerSeries = ProcessTrend.AddSeries(
            "POWER",
            "MWe",
            Color.Parse("#58D46C"));

        _pressureSeries = ProcessTrend.AddSeries(
            "PRESSURE",
            "MPa",
            Color.Parse("#57A8E8"));

        _frequencySeries = ProcessTrend.AddSeries(
            "FREQUENCY Δ",
            "Hz",
            Color.Parse("#E3C83B"));

        _levelSeries = Recorder.AddSeries(
            "DRUM LEVEL",
            "%",
            Color.Parse("#2D6F3F"));

        _valveSeries = Recorder.AddSeries(
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

    private SignalQuality CurrentQuality =>
        _qualityIndex switch
        {
            1 => SignalQuality.Uncertain,
            2 => SignalQuality.Bad,
            3 => SignalQuality.Unavailable,
            _ => SignalQuality.Good
        };

    private void OnSimulationTick(
        object? sender,
        EventArgs e)
    {
        _simulationTime += 0.1;
        _timerTicks++;
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
            (0.25 * Math.Sin(
                (_simulationTime * 0.21) + 0.9));

        var frequencyDeviation =
            5.0 +
            (0.35 * Math.Sin(
                (_simulationTime * 0.75) + 1.5));

        ProcessTrend.AddSample(
            _powerSeries,
            _simulationTime,
            power,
            quality);

        ProcessTrend.AddSample(
            _pressureSeries,
            _simulationTime,
            pressure,
            quality == SignalQuality.Bad
                ? SignalQuality.Uncertain
                : quality);

        ProcessTrend.AddSample(
            _frequencySeries,
            _simulationTime,
            frequencyDeviation,
            SignalQuality.Good);

        Recorder.AddSample(
            _levelSeries,
            _simulationTime,
            52.0 +
            (8.0 * Math.Sin(_simulationTime * 0.18)),
            quality);

        Recorder.AddSample(
            _valveSeries,
            _simulationTime,
            48.0 +
            (12.0 * Math.Sin(
                (_simulationTime * 0.24) + 1.1)),
            SignalQuality.Good);

        for (var index = 0;
             index < _scopeSamples.Length;
             index++)
        {
            var phase =
                index /
                (double)(_scopeSamples.Length - 1);

            _scopeSamples[index] =
                (0.72 * Math.Sin(
                    (phase * Math.PI * 6.0) +
                    _simulationTime)) +
                (0.12 * Math.Sin(
                    (phase * Math.PI * 18.0) -
                    _simulationTime));
        }

        Scope.SetSamples(_scopeSamples);
        Scope.Quality = quality;

        if (_timerTicks % 5 == 0)
        {
            CursorReadoutDisplay.Text =
                ProcessTrend.CursorReadout;
        }
    }

    private void OnApplyInterlock(
        object? sender,
        RoutedEventArgs e)
    {
        SetInterlockState(true);

        ControlInterlock.IsInterlocked = true;
        ControlInterlock.SatisfiedPermissiveCount = 2;
        ControlInterlock.RequiredPermissiveCount = 4;
        ControlInterlock.Reason =
            "SYNCHRONISM AND PROTECTION PERMISSIVES NOT SATISFIED";
    }

    private void OnClearInterlock(
        object? sender,
        RoutedEventArgs e)
    {
        SetInterlockState(false);

        ControlInterlock.IsInterlocked = false;
        ControlInterlock.SatisfiedPermissiveCount = 4;
        ControlInterlock.RequiredPermissiveCount = 4;
        ControlInterlock.Reason =
            "ALL REQUIRED PERMISSIVES SATISFIED";
    }

    private void SetInterlockState(bool isInterlocked)
    {
        FeedwaterSlider.IsInterlocked = isInterlocked;
        ValveSlider.IsInterlocked = isInterlocked;
        LoadKnob.IsInterlocked = isInterlocked;
        ModeSelector.IsInterlocked = isInterlocked;
        BreakerSwitch.IsInterlocked = isInterlocked;
        PumpSwitch.IsInterlocked = isInterlocked;
        SpeedTrimSwitch.IsInterlocked = isInterlocked;
    }

    private void OnToggleRun(
        object? sender,
        RoutedEventArgs e)
    {
        Recorder.IsRunning = !Recorder.IsRunning;

        TrendStatusDisplay.Text = Recorder.IsRunning
            ? "STRIP-CHART RECORDER RUNNING"
            : "STRIP-CHART RECORDER PAUSED";

        TrendStatusDisplay.LedColor = Recorder.IsRunning
            ? LedDisplayColor.Green
            : LedDisplayColor.Amber;
    }

    private void OnCycleQuality(
        object? sender,
        RoutedEventArgs e)
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

        TrendStatusDisplay.Text =
            string.Concat(
                "POWER SIGNAL QUALITY: ",
                quality.ToString().ToUpperInvariant());

        TrendStatusDisplay.LedColor = quality switch
        {
            SignalQuality.Good => LedDisplayColor.Green,
            SignalQuality.Uncertain => LedDisplayColor.Amber,
            SignalQuality.Bad => LedDisplayColor.Red,
            _ => LedDisplayColor.Blue
        };
    }

    private void OnCursorLeft(
        object? sender,
        RoutedEventArgs e)
    {
        ProcessTrend.CursorFraction =
            Math.Max(
                0,
                ProcessTrend.CursorFraction - 0.05);

        CursorReadoutDisplay.Text =
            ProcessTrend.CursorReadout;
    }

    private void OnCursorRight(
        object? sender,
        RoutedEventArgs e)
    {
        ProcessTrend.CursorFraction =
            Math.Min(
                1,
                ProcessTrend.CursorFraction + 0.05);

        CursorReadoutDisplay.Text =
            ProcessTrend.CursorReadout;
    }

    private void OnClearHistory(
        object? sender,
        RoutedEventArgs e)
    {
        ProcessTrend.ClearSamples();
        Recorder.ClearSamples();
        Scope.ClearSamples();

        CursorReadoutDisplay.Text =
            ProcessTrend.CursorReadout;

        TrendStatusDisplay.Text =
            "ALL SIGNAL HISTORY CLEARED";

        TrendStatusDisplay.LedColor =
            LedDisplayColor.Red;
    }

    private void OnRaiseAlarms(
        object? sender,
        RoutedEventArgs e)
    {
        AlarmPanel.Activate("STEAM_LOW");
        AlarmPanel.Activate("VACUUM_LOW");
        AlarmPanel.Activate("PUMP_TRIP");
        AlarmPanel.Activate("FEED_LOW");
        AlarmPanel.Activate("GRID_DEVIATION");
        AlarmPanel.Activate("OIL_PRESSURE");

        UpdateAlarmStatus(
            "NEW ALARMS — ACK REQUIRED",
            LedDisplayColor.Red);
    }

    private void OnAcknowledgeAll(
        object? sender,
        RoutedEventArgs e)
    {
        var count = AlarmPanel.AcknowledgeAll();

        UpdateAlarmStatus(
            count > 0
                ? $"ACKNOWLEDGED {count} ALARM INDICATORS"
                : "NO ALARMS TO ACKNOWLEDGE",
            count > 0
                ? LedDisplayColor.Amber
                : LedDisplayColor.Green);
    }

    private void OnClearConditions(
        object? sender,
        RoutedEventArgs e)
    {
        var count = AlarmPanel.ClearAllConditions();

        UpdateAlarmStatus(
            count > 0
                ? $"CONDITIONS RETURNED — {AlarmPanel.LatchedAlarmCount} LATCHED"
                : "NO ACTIVE CONDITIONS",
            AlarmPanel.LatchedAlarmCount > 0
                ? LedDisplayColor.Amber
                : LedDisplayColor.Green);
    }

    private void OnResetAll(
        object? sender,
        RoutedEventArgs e)
    {
        var count = AlarmPanel.ResetAll();

        UpdateAlarmStatus(
            count > 0
                ? $"RESET {count} LATCHED ALARMS"
                : "RESET NOT PERMITTED OR NOTHING LATCHED",
            AlarmPanel.LatchedAlarmCount == 0
                ? LedDisplayColor.Green
                : LedDisplayColor.Red);
    }

    private void UpdateAlarmStatus(
        string text,
        LedDisplayColor color)
    {
        AlarmStatusDisplay.Text = text;
        AlarmStatusDisplay.LedColor = color;
    }
}
