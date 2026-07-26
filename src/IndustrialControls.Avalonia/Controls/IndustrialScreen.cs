using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Controls;

/// <summary>
/// Cornice per monitor e schermate operative industriali.
/// </summary>
public sealed class IndustrialScreen : ContentControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<IndustrialScreen, string>(
            nameof(Title), string.Empty);

    public static readonly StyledProperty<string> StatusTextProperty =
        AvaloniaProperty.Register<IndustrialScreen, string>(
            nameof(StatusText), "ONLINE");

    public static readonly StyledProperty<bool> IsOnlineProperty =
        AvaloniaProperty.Register<IndustrialScreen, bool>(
            nameof(IsOnline), true);

    public static readonly StyledProperty<bool> ShowScanlinesProperty =
        AvaloniaProperty.Register<IndustrialScreen, bool>(
            nameof(ShowScanlines), true);

    public static readonly DirectProperty<IndustrialScreen, IBrush> StatusBrushProperty =
        AvaloniaProperty.RegisterDirect<IndustrialScreen, IBrush>(
            nameof(StatusBrush), control => control.StatusBrush);

    private IBrush _statusBrush =
        new SolidColorBrush(Color.Parse("#58D46C"));

    static IndustrialScreen()
    {
        TitleProperty.Changed.AddClassHandler<IndustrialScreen>(
            (control, _) => control.RefreshState());
        StatusTextProperty.Changed.AddClassHandler<IndustrialScreen>(
            (control, _) => control.RefreshState());
        IsOnlineProperty.Changed.AddClassHandler<IndustrialScreen>(
            (control, _) => control.RefreshState());
    }

    public IndustrialScreen() => RefreshState();

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string StatusText
    {
        get => GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    public bool IsOnline
    {
        get => GetValue(IsOnlineProperty);
        set => SetValue(IsOnlineProperty, value);
    }

    public bool ShowScanlines
    {
        get => GetValue(ShowScanlinesProperty);
        set => SetValue(ShowScanlinesProperty, value);
    }

    public IBrush StatusBrush
    {
        get => _statusBrush;
        private set => SetAndRaise(
            StatusBrushProperty,
            ref _statusBrush,
            value);
    }

    private void RefreshState()
    {
        StatusBrush = new SolidColorBrush(
            Color.Parse(IsOnline ? "#58D46C" : "#F14C4C"));

        IndustrialAutomationMetadata.Apply(
            this,
            Title,
            string.Concat(StatusText, "; ", IsOnline ? "online" : "offline"),
            "IndustrialScreen");
    }
}
