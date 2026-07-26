using System;
using Avalonia;
using Avalonia.Threading;

namespace IndustrialControls.Avalonia.Controls;

public sealed class LedMarqueeDisplay : LedMatrixDisplay
{
    private readonly DispatcherTimer _timer;
    private int _offset;

    public static readonly StyledProperty<bool> IsRunningProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, bool>(nameof(IsRunning), true);

    public static readonly StyledProperty<int> VisibleCharactersProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, int>(
            nameof(VisibleCharacters), 24, validate: value => value is >= 4 and <= 200);

    public static readonly StyledProperty<int> ScrollIntervalMillisecondsProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, int>(
            nameof(ScrollIntervalMilliseconds), 180, validate: value => value is >= 40 and <= 5000);

    public static readonly StyledProperty<int> EndPauseCharactersProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, int>(
            nameof(EndPauseCharacters), 5, validate: value => value is >= 0 and <= 50);

    public static readonly DirectProperty<LedMarqueeDisplay, string> DisplayTextProperty =
        AvaloniaProperty.RegisterDirect<LedMarqueeDisplay, string>(
            nameof(DisplayText), control => control.DisplayText);

    private string _displayText = string.Empty;

    static LedMarqueeDisplay()
    {
        TextProperty.Changed.AddClassHandler<LedMarqueeDisplay>((control, _) => control.Reset());
        IsRunningProperty.Changed.AddClassHandler<LedMarqueeDisplay>((control, _) => control.RefreshTimer());
        ScrollIntervalMillisecondsProperty.Changed.AddClassHandler<LedMarqueeDisplay>((control, _) => control.RefreshTimer());
        VisibleCharactersProperty.Changed.AddClassHandler<LedMarqueeDisplay>((control, _) => control.Reset());
    }

    public LedMarqueeDisplay()
    {
        _timer = new DispatcherTimer();
        _timer.Tick += OnTick;
        Reset();
        RefreshTimer();
    }

    public bool IsRunning
    {
        get => GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }

    public int VisibleCharacters
    {
        get => GetValue(VisibleCharactersProperty);
        set => SetValue(VisibleCharactersProperty, value);
    }

    public int ScrollIntervalMilliseconds
    {
        get => GetValue(ScrollIntervalMillisecondsProperty);
        set => SetValue(ScrollIntervalMillisecondsProperty, value);
    }

    public int EndPauseCharacters
    {
        get => GetValue(EndPauseCharactersProperty);
        set => SetValue(EndPauseCharactersProperty, value);
    }

    public string DisplayText
    {
        get => _displayText;
        private set => SetAndRaise(DisplayTextProperty, ref _displayText, value);
    }

    private void OnTick(object? sender, EventArgs e)
    {
        var source = BuildSource();
        if (source.Length == 0)
        {
            DisplayText = string.Empty;
            return;
        }

        _offset = (_offset + 1) % source.Length;
        DisplayText = BuildWindow(source, _offset);
    }

    private void Reset()
    {
        _offset = 0;
        DisplayText = BuildWindow(BuildSource(), 0);
    }

    private void RefreshTimer()
    {
        _timer.Stop();
        _timer.Interval = TimeSpan.FromMilliseconds(ScrollIntervalMilliseconds);
        if (IsRunning)
        {
            _timer.Start();
        }
    }

    private string BuildSource()
    {
        var padding = new string(' ', Math.Max(1, EndPauseCharacters));
        return string.Concat(Text ?? string.Empty, padding);
    }

    private string BuildWindow(string source, int offset)
    {
        if (source.Length == 0)
        {
            return string.Empty;
        }

        var result = new char[VisibleCharacters];
        for (var index = 0; index < result.Length; index++)
        {
            result[index] = source[(offset + index) % source.Length];
        }

        return new string(result);
    }
}
