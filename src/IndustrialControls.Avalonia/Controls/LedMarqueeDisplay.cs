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
    private const int MinimumVisibleCharacters = 4;
    private const int MaximumVisibleCharacters = 200;

    private readonly DispatcherTimer _timer;

    private int _offset;
    private int _effectiveVisibleCharacters = 24;
    private int _scrollSourceBuildCount;
    private string _scrollSource = string.Empty;
    private char[] _windowBuffer = Array.Empty<char>();

    public static readonly StyledProperty<bool> IsRunningProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, bool>(
            nameof(IsRunning),
            true);

    public static readonly StyledProperty<int> VisibleCharactersProperty =
        AvaloniaProperty.Register<LedMarqueeDisplay, int>(
            nameof(VisibleCharacters),
            24,
            validate: value => value is
                >= MinimumVisibleCharacters and
                <= MaximumVisibleCharacters);

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
            (control, _) => control.ResetScrollState());

        IsRunningProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.RefreshTimer());

        ScrollIntervalMillisecondsProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.RefreshTimer());

        VisibleCharactersProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.RefreshCapacity());

        AutoFitVisibleCharactersProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.RefreshCapacity());

        EndPauseCharactersProperty.Changed.AddClassHandler<LedMarqueeDisplay>(
            (control, _) => control.ResetScrollState());
    }

    public LedMarqueeDisplay()
    {
        _effectiveVisibleCharacters = VisibleCharacters;

        _timer = new DispatcherTimer();
        _timer.Tick += OnTick;

        ResetScrollState();
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

    internal int ScrollSourceBuildCount =>
        _scrollSourceBuildCount;

    internal int CachedScrollSourceLength =>
        _scrollSource.Length;

    protected override Size ArrangeOverride(Size finalSize)
    {
        var arrangedSize =
            base.ArrangeOverride(finalSize);

        ApplyEffectiveCapacity(
            CalculateEffectiveVisibleCharacters(
                finalSize.Width,
                AutoFitVisibleCharacters,
                VisibleCharacters));

        return arrangedSize;
    }

    internal static int CalculateEffectiveVisibleCharacters(
        double actualWidth,
        bool autoFit,
        int manualVisibleCharacters)
    {
        var normalizedManual = Math.Clamp(
            manualVisibleCharacters,
            MinimumVisibleCharacters,
            MaximumVisibleCharacters);

        if (!autoFit ||
            !double.IsFinite(actualWidth) ||
            actualWidth <= 0)
        {
            return normalizedManual;
        }

        var usableWidth = Math.Max(
            0,
            actualWidth - HorizontalChromeWidth);

        var calculated = (int)Math.Floor(
            usableWidth / EstimatedCharacterPitch);

        return Math.Clamp(
            calculated,
            MinimumVisibleCharacters,
            MaximumVisibleCharacters);
    }

    internal void AdvanceForDiagnostics() =>
        AdvanceWindow();

    private void OnTick(
        object? sender,
        EventArgs e) =>
        AdvanceWindow();

    private void AdvanceWindow()
    {
        if (_scrollSource.Length == 0)
        {
            DisplayText = string.Empty;
            return;
        }

        _offset =
            (_offset + 1) %
            _scrollSource.Length;

        UpdateDisplayWindow();
    }

    private void ResetScrollState()
    {
        _offset = 0;
        RebuildScrollSource();
        UpdateDisplayWindow();
    }

    private void RefreshCapacity()
    {
        ApplyEffectiveCapacity(
            CalculateEffectiveVisibleCharacters(
                Bounds.Width,
                AutoFitVisibleCharacters,
                VisibleCharacters));
    }

    private void ApplyEffectiveCapacity(int capacity)
    {
        if (EffectiveVisibleCharacters == capacity)
        {
            return;
        }

        EffectiveVisibleCharacters = capacity;
        ResetScrollState();
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

    private void RebuildScrollSource()
    {
        _scrollSourceBuildCount++;

        var text =
            Text ??
            string.Empty;

        if (text.Length == 0)
        {
            _scrollSource = string.Empty;
            EnsureWindowBuffer();
            return;
        }

        _scrollSource = string.Concat(
            new string(
                ' ',
                EffectiveVisibleCharacters),
            text,
            new string(
                ' ',
                EndPauseCharacters));

        EnsureWindowBuffer();
    }

    private void EnsureWindowBuffer()
    {
        if (_windowBuffer.Length ==
            EffectiveVisibleCharacters)
        {
            return;
        }

        _windowBuffer =
            new char[EffectiveVisibleCharacters];
    }

    private void UpdateDisplayWindow()
    {
        if (_scrollSource.Length == 0)
        {
            DisplayText = string.Empty;
            return;
        }

        for (var index = 0;
             index < _windowBuffer.Length;
             index++)
        {
            _windowBuffer[index] =
                _scrollSource[
                    (_offset + index) %
                    _scrollSource.Length];
        }

        DisplayText =
            new string(_windowBuffer);
    }
}
