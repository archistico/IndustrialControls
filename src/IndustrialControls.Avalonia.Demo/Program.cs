using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using IndustrialControls.Avalonia.Controls;

namespace IndustrialControls.Avalonia.Demo;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            if (args.Length == 2 && args[0] is "--render-diagram" or "--render-catalog" or "--render-toggle")
            {
                BuildAvaloniaApp().SetupWithoutStarting();
                MainWindow? catalog = null;
                if (args[0] == "--render-catalog") catalog = new MainWindow();
                var diagram = new IndustrialDiagram { Document = PlantDiagramSample.Create(), IsAnimationEnabled = false, AnimationTime = .4 };
                var size = new Size(1640, 820);
                Control visual = args[0] == "--render-toggle" ? new ToggleSwitchPreview() : catalog?.Content as Control ?? diagram;
                if (args[0] == "--render-toggle") size = new Size(950, 370);
                if (catalog is not null) size = new Size(1520, 980);
                visual.Measure(size);
                visual.Arrange(new Rect(size));
                var activeDiagram = catalog?.FindControl<IndustrialDiagram>("PlantDiagram") ?? diagram;
                activeDiagram.IsAnimationEnabled = false;
                activeDiagram.FitToContent(35);
                using var bitmap = new RenderTargetBitmap(new PixelSize((int)size.Width, (int)size.Height), new Vector(96, 96));
                bitmap.Render(visual);
                bitmap.Save(args[1]);
                catalog?.Close();
                return;
            }
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception exception)
        {
            DemoStartupDiagnostics.WriteException(
                "Application startup",
                exception);

            throw;
        }
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
