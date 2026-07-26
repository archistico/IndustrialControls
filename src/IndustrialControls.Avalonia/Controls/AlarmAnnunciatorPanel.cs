using Avalonia;
using Avalonia.Controls;

namespace IndustrialControls.Avalonia.Controls;

public sealed class AlarmAnnunciatorPanel : ItemsControl
{
    public static readonly StyledProperty<int> ColumnsProperty =
        AvaloniaProperty.Register<AlarmAnnunciatorPanel, int>(
            nameof(Columns), 2, validate: value => value is >= 1 and <= 12);

    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }
}
