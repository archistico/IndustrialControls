using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Demo;

/// <summary>Offscreen visual comparison of the lever's two positions and interlocked finish.</summary>
internal sealed class ToggleSwitchPreview : Control
{
    private readonly ToggleSwitchDial[] _dials =
    [
        new() { IsOn = false }, new() { IsOn = true },
        new() { IsOn = false, IsInterlocked = true }, new() { IsOn = true, IsInterlocked = true },
    ];
    public override void Render(DrawingContext context)
    {
        context.DrawRectangle(new SolidColorBrush(Color.Parse("#24292D")), null, new Rect(Bounds.Size));
        Write(context, "INDUSTRIAL TOGGLE SWITCH", new Point(28, 22), 20, Brushes.WhiteSmoke);
        for (var i = 0; i < _dials.Length; i++)
        {
            var x = 28 + i * 225;
            context.DrawRectangle(new SolidColorBrush(Color.Parse("#41484D")),
                new Pen(new SolidColorBrush(Color.Parse("#687279")), 1), new Rect(x, 70, 207, 270), 4, 4);
            var dial = _dials[i];
            Write(context, dial.IsOn ? "ON" : "OFF", new Point(x + 86, 88), 15, Brushes.WhiteSmoke);
            dial.Measure(new Size(180, 180));
            dial.Arrange(new Rect(0, 0, 180, 180));
            using (context.PushTransform(Matrix.CreateTranslation(x + 13, 112))) dial.Render(context);
            Write(context, dial.IsInterlocked ? "INTERLOCKED" : "SWITCHING AVAILABLE",
                new Point(x + (dial.IsInterlocked ? 54 : 30), 310), 10, dial.IsInterlocked ? Brushes.Khaki : Brushes.LightGray);
        }
    }
    private static void Write(DrawingContext context, string text, Point point, double size, IBrush brush)
        => context.DrawText(new FormattedText(text, CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight, Typeface.Default, size, brush), point);
}
