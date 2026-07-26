using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Spia industriale con stato acceso, colore funzionale e lampeggio opzionale.
/// </summary>
public sealed class IndustrialLamp : TemplatedControl
{
    private readonly DispatcherTimer _blinkTimer;
    private bool _blinkPhase = true;

    public static readonly StyledProperty<bool> IsOnProperty =
        AvaloniaProperty.Register<IndustrialLamp, bool>(nameof(IsOn));

    public static readonly StyledProperty<IndustrialLampColor> LampColorProperty =
        AvaloniaProperty.Register<IndustrialLamp, IndustrialLampColor>(
            nameof(LampColor),
            IndustrialLampColor.Red);

    public static readonly StyledProperty<IndustrialLampShape> ShapeProperty =
        AvaloniaProperty.Register<IndustrialLamp, IndustrialLampShape>(
            nameof(Shape),
            IndustrialLampShape.Round);

    public static readonly StyledProperty<IndustrialLampState> StateProperty =
        AvaloniaProperty.Register<IndustrialLamp, IndustrialLampState>(
            nameof(State),
            IndustrialLampState.Normal);

    public static readonly StyledProperty<double> GlowIntensityProperty =
        AvaloniaProperty.Register<IndustrialLamp, double>(
            nameof(GlowIntensity),
            0.85,
            validate: value => value is >= 0 and <= 1);

    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<IndustrialLamp, string?>(nameof(Label));

    public static readonly StyledProperty<bool> ShowLabelProperty =
        AvaloniaProperty.Register<IndustrialLamp, bool>(nameof(ShowLabel), true);

    public static readonly DirectProperty<IndustrialLamp, IBrush> ActiveBrushProperty =
        AvaloniaProperty.RegisterDirect<IndustrialLamp, IBrush>(
            nameof(ActiveBrush),
            control => control.ActiveBrush);

    public static readonly DirectProperty<IndustrialLamp, IBrush> InactiveBrushProperty =
        AvaloniaProperty.RegisterDirect<IndustrialLamp, IBrush>(
            nameof(InactiveBrush),
            control => control.InactiveBrush);

    public static readonly DirectProperty<IndustrialLamp, double> EffectiveOpacityProperty =
        AvaloniaProperty.RegisterDirect<IndustrialLamp, double>(
            nameof(EffectiveOpacity),
            control => control.EffectiveOpacity);

    public static readonly DirectProperty<IndustrialLamp, CornerRadius> LampCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<IndustrialLamp, CornerRadius>(
            nameof(LampCornerRadius),
            control => control.LampCornerRadius);

    private IBrush _activeBrush = Brushes.Red;
    private IBrush _inactiveBrush = new SolidColorBrush(Color.Parse("#351717"));
    private double _effectiveOpacity = 0.28;
    private CornerRadius _lampCornerRadius = new(999);

    static IndustrialLamp()
    {
        IsOnProperty.Changed.AddClassHandler<IndustrialLamp>((control, _) => control.RefreshVisualState());
        LampColorProperty.Changed.AddClassHandler<IndustrialLamp>((control, _) => control.RefreshVisualState());
        ShapeProperty.Changed.AddClassHandler<IndustrialLamp>((control, _) => control.RefreshVisualState());
        StateProperty.Changed.AddClassHandler<IndustrialLamp>((control, _) => control.RefreshBlinking());
        GlowIntensityProperty.Changed.AddClassHandler<IndustrialLamp>((control, _) => control.RefreshVisualState());
    }

    public IndustrialLamp()
    {
        _blinkTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(650)
        };
        _blinkTimer.Tick += OnBlinkTimerTick;
        RefreshVisualState();
    }

    public bool IsOn
    {
        get => GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }

    public IndustrialLampColor LampColor
    {
        get => GetValue(LampColorProperty);
        set => SetValue(LampColorProperty, value);
    }

    public IndustrialLampShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public IndustrialLampState State
    {
        get => GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public double GlowIntensity
    {
        get => GetValue(GlowIntensityProperty);
        set => SetValue(GlowIntensityProperty, value);
    }

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public bool ShowLabel
    {
        get => GetValue(ShowLabelProperty);
        set => SetValue(ShowLabelProperty, value);
    }

    public IBrush ActiveBrush
    {
        get => _activeBrush;
        private set => SetAndRaise(ActiveBrushProperty, ref _activeBrush, value);
    }

    public IBrush InactiveBrush
    {
        get => _inactiveBrush;
        private set => SetAndRaise(InactiveBrushProperty, ref _inactiveBrush, value);
    }

    public double EffectiveOpacity
    {
        get => _effectiveOpacity;
        private set => SetAndRaise(EffectiveOpacityProperty, ref _effectiveOpacity, value);
    }

    public CornerRadius LampCornerRadius
    {
        get => _lampCornerRadius;
        private set => SetAndRaise(LampCornerRadiusProperty, ref _lampCornerRadius, value);
    }

    private void RefreshBlinking()
    {
        _blinkTimer.Stop();
        _blinkPhase = true;

        if (State is IndustrialLampState.BlinkingSlow or IndustrialLampState.BlinkingFast)
        {
            _blinkTimer.Interval = State == IndustrialLampState.BlinkingFast
                ? TimeSpan.FromMilliseconds(220)
                : TimeSpan.FromMilliseconds(650);
            _blinkTimer.Start();
        }

        RefreshVisualState();
    }

    private void OnBlinkTimerTick(object? sender, EventArgs e)
    {
        _blinkPhase = !_blinkPhase;
        RefreshVisualState();
    }

    private void RefreshVisualState()
    {
        var palette = GetPalette(LampColor);
        ActiveBrush = new SolidColorBrush(palette.Active);
        InactiveBrush = new SolidColorBrush(palette.Inactive);
        LampCornerRadius = Shape switch
        {
            IndustrialLampShape.Round => new CornerRadius(999),
            IndustrialLampShape.Capsule => new CornerRadius(999),
            IndustrialLampShape.Square => new CornerRadius(2),
            _ => new CornerRadius(3)
        };

        var visibleOn = IsOn;
        if (State is IndustrialLampState.BlinkingSlow or IndustrialLampState.BlinkingFast)
        {
            visibleOn &= _blinkPhase;
        }

        EffectiveOpacity = State switch
        {
            IndustrialLampState.Unavailable => 0.12,
            IndustrialLampState.Fault => _blinkPhase ? 1.0 : 0.35,
            _ when visibleOn => Math.Clamp(0.45 + (GlowIntensity * 0.55), 0.45, 1.0),
            _ => 0.28
        };
    }

    private static (Color Active, Color Inactive) GetPalette(IndustrialLampColor color)
    {
        return color switch
        {
            IndustrialLampColor.Amber => (Color.Parse("#FFB238"), Color.Parse("#3A2914")),
            IndustrialLampColor.Yellow => (Color.Parse("#F2DD4B"), Color.Parse("#373313")),
            IndustrialLampColor.Green => (Color.Parse("#58D46C"), Color.Parse("#16351C")),
            IndustrialLampColor.Blue => (Color.Parse("#57A8E8"), Color.Parse("#172D3D")),
            IndustrialLampColor.White => (Color.Parse("#F1F1DF"), Color.Parse("#34342F")),
            _ => (Color.Parse("#F14C4C"), Color.Parse("#3A1717"))
        };
    }
}
