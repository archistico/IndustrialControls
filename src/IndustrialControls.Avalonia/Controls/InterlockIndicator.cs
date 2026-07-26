using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Indicatore compatto di permissivi e interlock operativi.
/// </summary>
public sealed class InterlockIndicator : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<InterlockIndicator, string>(
            nameof(Title), "CONTROL PERMISSIVES");

    public static readonly StyledProperty<bool> IsInterlockedProperty =
        AvaloniaProperty.Register<InterlockIndicator, bool>(nameof(IsInterlocked));

    public static readonly StyledProperty<string> ReasonProperty =
        AvaloniaProperty.Register<InterlockIndicator, string>(
            nameof(Reason), "ALL PERMISSIVES SATISFIED");

    public static readonly StyledProperty<int> SatisfiedPermissiveCountProperty =
        AvaloniaProperty.Register<InterlockIndicator, int>(
            nameof(SatisfiedPermissiveCount), 3, validate: value => value >= 0);

    public static readonly StyledProperty<int> RequiredPermissiveCountProperty =
        AvaloniaProperty.Register<InterlockIndicator, int>(
            nameof(RequiredPermissiveCount), 3, validate: value => value >= 0);

    public static readonly DirectProperty<InterlockIndicator, bool> IsPermittedProperty =
        AvaloniaProperty.RegisterDirect<InterlockIndicator, bool>(
            nameof(IsPermitted), control => control.IsPermitted);

    public static readonly DirectProperty<InterlockIndicator, string> StatusTextProperty =
        AvaloniaProperty.RegisterDirect<InterlockIndicator, string>(
            nameof(StatusText), control => control.StatusText);

    public static readonly DirectProperty<InterlockIndicator, string> CountTextProperty =
        AvaloniaProperty.RegisterDirect<InterlockIndicator, string>(
            nameof(CountText), control => control.CountText);

    public static readonly DirectProperty<InterlockIndicator, IBrush> StatusBrushProperty =
        AvaloniaProperty.RegisterDirect<InterlockIndicator, IBrush>(
            nameof(StatusBrush), control => control.StatusBrush);

    public static readonly DirectProperty<InterlockIndicator, IndustrialLampColor> LampColorProperty =
        AvaloniaProperty.RegisterDirect<InterlockIndicator, IndustrialLampColor>(
            nameof(LampColor), control => control.LampColor);

    private bool _isPermitted = true;
    private string _statusText = "PERMITTED";
    private string _countText = "3 / 3";
    private IBrush _statusBrush = new SolidColorBrush(Color.Parse("#58D46C"));
    private IndustrialLampColor _lampColor = IndustrialLampColor.Green;

    static InterlockIndicator()
    {
        IsInterlockedProperty.Changed.AddClassHandler<InterlockIndicator>(
            (control, _) => control.RefreshState());
        ReasonProperty.Changed.AddClassHandler<InterlockIndicator>(
            (control, _) => control.RefreshState());
        SatisfiedPermissiveCountProperty.Changed.AddClassHandler<InterlockIndicator>(
            (control, _) => control.RefreshState());
        RequiredPermissiveCountProperty.Changed.AddClassHandler<InterlockIndicator>(
            (control, _) => control.RefreshState());
    }

    public InterlockIndicator() => RefreshState();

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool IsInterlocked
    {
        get => GetValue(IsInterlockedProperty);
        set => SetValue(IsInterlockedProperty, value);
    }

    public string Reason
    {
        get => GetValue(ReasonProperty);
        set => SetValue(ReasonProperty, value);
    }

    public int SatisfiedPermissiveCount
    {
        get => GetValue(SatisfiedPermissiveCountProperty);
        set => SetValue(SatisfiedPermissiveCountProperty, value);
    }

    public int RequiredPermissiveCount
    {
        get => GetValue(RequiredPermissiveCountProperty);
        set => SetValue(RequiredPermissiveCountProperty, value);
    }

    public bool IsPermitted
    {
        get => _isPermitted;
        private set => SetAndRaise(IsPermittedProperty, ref _isPermitted, value);
    }

    public string StatusText
    {
        get => _statusText;
        private set => SetAndRaise(StatusTextProperty, ref _statusText, value);
    }

    public string CountText
    {
        get => _countText;
        private set => SetAndRaise(CountTextProperty, ref _countText, value);
    }

    public IBrush StatusBrush
    {
        get => _statusBrush;
        private set => SetAndRaise(StatusBrushProperty, ref _statusBrush, value);
    }

    public IndustrialLampColor LampColor
    {
        get => _lampColor;
        private set => SetAndRaise(LampColorProperty, ref _lampColor, value);
    }

    private void RefreshState()
    {
        IsPermitted =
            !IsInterlocked &&
            SatisfiedPermissiveCount >= RequiredPermissiveCount;

        StatusText = IsPermitted ? "PERMITTED" : "INTERLOCKED";
        CountText = string.Concat(
            SatisfiedPermissiveCount,
            " / ",
            RequiredPermissiveCount);

        StatusBrush = new SolidColorBrush(
            Color.Parse(IsPermitted ? "#58D46C" : "#F14C4C"));

        LampColor = IsPermitted
            ? IndustrialLampColor.Green
            : IndustrialLampColor.Red;
    }
}
