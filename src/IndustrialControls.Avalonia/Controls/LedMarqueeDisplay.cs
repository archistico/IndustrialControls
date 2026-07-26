using System;
using Avalonia;
using Avalonia.Threading;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Display LED scorrevole con finestra adattiva alla larghezza del controllo.
/// </summary>
public sealed class LedMarqueeDisplay : LedMatrixDisplay
{
    private const double HorizontalChromeWidth = 50.0;
    private const double EstimatedCharacterPitch = 15.2;

    private readonly DispatcherTimer _timer;

    private int _offset;
    private int _effectiveVisibleCharacters = 24;

    public static readonly StyledProperty<bool> IsRunningProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, bool>(
            nameof(IsRunning),
            true);

    public static readonly StyledProperty<int> VisibleCharactersProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, int>(
            nameof(VisibleCharacters),
            24,
            validate: value => value is >= 4 and <= 200);

    public static readonly StyledProperty<bool> AutoFitVisibleCharactersProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, bool>(
            nameof(AutoFitVisibleCharacters),
            true);

    public static readonly StyledProperty<int> ScrollIntervalMillisecondsProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, int>(
            nameof(ScrollIntervalMilliseconds),
            180,
            validate: value => value is >= 40 and <= 5000);

    public static readonly StyledProperty<int> EndPauseCharactersProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, int>(
            nameof(EndPauseCharacters),
            5,
            validate: value => value is >= 0 and <= 50);

    public static readonly DirectProperty<LedMarqueeDisplay, string> DisplayTextProperty =
        AvaloniaProperty.RegisterDirect<LedMarqueeDisplay, string>(
            nameof(DisplayText),
            control => control.DisplayText);

    public static readonly DirectProperty<LedMarqueeDisplay, int> EffectiveVisibleCharactersProperty =
        AvaloniaProperty.RegisterDirect<LedMarqueeDisplay, int>(
            nameof(EffectiveVisibleCharacters),
            control => control.EffectiveVisibleCharacters);

    private string _displayText = string.Empty;

    static LedMarqueeDisplay()
    {
        TextProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.Reset());

        IsRunningProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.RefreshTimer());

        ScrollIntervalMillisecondsProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.RefreshTimer());

        VisibleCharactersProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.RefreshCapacity());

        AutoFitVisibleCharactersProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.RefreshCapacity());

        EndPauseCharactersProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.Reset());
    }

    public LedMarqueeDisplay()
    {
        _effectiveVisibleCharacters = VisibleCharacters;

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

    /// <summary>
    /// Capacità manuale e fallback usata prima del primo layout.
    /// </summary>
    public int VisibleCharacters
    {
        get => GetValue(VisibleCharactersProperty);
        set => SetValue(VisibleCharactersProperty, value);
    }

    /// <summary>
    /// Adatta automaticamente la finestra di testo alla larghezza reale.
    /// </summary>
    public bool AutoFitVisibleCharacters
    {
        get => GetValue(AutoFitVisibleCharactersProperty);
        set => SetValue(AutoFitVisibleCharactersProperty, value);
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
        private set => SetAndRaise(
            DisplayTextProperty,
            ref _displayText,
            value);
    }

    public int EffectiveVisibleCharacters
    {
        get => _effectiveVisibleCharacters;
        private set => SetAndRaise(
            EffectiveVisibleCharactersProperty,
            ref _effectiveVisibleCharacters,
            value);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arrangedSize = base.ArrangeOverride(finalSize);
        UpdateEffectiveCapacity(finalSize.Width);
        return arrangedSize;
    }

    private void OnTick(
        object? sender,
        EventArgs e)
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
        DisplayText = BuildWindow(
            BuildSource(),
            0);
    }

    private void RefreshCapacity()
    {
        if (!AutoFitVisibleCharacters ||
            Bounds.Width <= HorizontalChromeWidth)
        {
            SetEffectiveCapacity(VisibleCharacters);
            return;
        }

        UpdateEffectiveCapacity(Bounds.Width);
    }

    private void UpdateEffectiveCapacity(double actualWidth)
    {
        if (!AutoFitVisibleCharacters)
        {
            SetEffectiveCapacity(VisibleCharacters);
            return;
        }

        var usableWidth = Math.Max(
            0,
            actualWidth - HorizontalChromeWidth);

        var calculated = (int)Math.Floor(
            usableWidth / EstimatedCharacterPitch);

        SetEffectiveCapacity(
            Math.Clamp(
                calculated,
                4,
                200));
    }

    private void SetEffectiveCapacity(int capacity)
    {
        if (EffectiveVisibleCharacters == capacity)
        {
            return;
        }

        EffectiveVisibleCharacters = capacity;
        Reset();
    }

    private void RefreshTimer()
    {
        _timer.Stop();
        _timer.Interval =
            TimeSpan.FromMilliseconds(
                ScrollIntervalMilliseconds);

        if (IsRunning)
        {
            _timer.Start();
        }
    }

    private string BuildSource()
    {
        var text = Text ?? string.Empty;

        if (text.Length == 0)
        {
            return string.Empty;
        }

        var leadingViewport =
            new string(
                ' ',
                EffectiveVisibleCharacters);

        var endPause =
            new string(
                ' ',
                EndPauseCharacters);

        return string.Concat(
            leadingViewport,
            text,
            endPause);
    }

    private string BuildWindow(
        string source,
        int offset)
    {
        if (source.Length == 0)
        {
            return string.Empty;
        }

        var result =
            new char[EffectiveVisibleCharacters];

        for (var index = 0;
             index < result.Length;
             index++)
        {
            result[index] =
                source[(offset + index) % source.Length];
        }

        return new string(result);
    }
}
