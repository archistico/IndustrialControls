using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace IndustrialControls.Avalonia.Demo;

public sealed partial class App : Application
{
    public override void Initialize()
    {
        try
        {
            AvaloniaXamlLoader.Load(this);
        }
        catch (Exception exception)
        {
            DemoStartupDiagnostics.WriteException(
                "Application XAML loading",
                exception);

            throw;
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is
            IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                desktop.MainWindow = new MainWindow();
            }
            catch (Exception exception)
            {
                DemoStartupDiagnostics.WriteException(
                    "Main window construction",
                    exception);

                desktop.MainWindow =
                    CreateStartupFailureWindow(exception);
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static Window CreateStartupFailureWindow(
        Exception exception) =>
        new()
        {
            Title = "IndustrialControls.Avalonia Demo — startup error",
            Width = 920,
            Height = 620,
            Background = new SolidColorBrush(
                Color.Parse("#1C1F21")),
            Content = new ScrollViewer
            {
                Content = new StackPanel
                {
                    Margin = new Thickness(24),
                    Spacing = 14,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "THE DEMO COULD NOT CREATE ITS MAIN WINDOW",
                            FontSize = 22,
                            FontWeight = FontWeight.Bold,
                            Foreground = new SolidColorBrush(
                                Color.Parse("#F14C4C"))
                        },
                        new TextBlock
                        {
                            Text =
                                "A diagnostic log was written to:",
                            Foreground = new SolidColorBrush(
                                Color.Parse("#E5E7DE"))
                        },
                        new TextBlock
                        {
                            Text = DemoStartupDiagnostics.LogPath,
                            TextWrapping = TextWrapping.Wrap,
                            FontFamily = new FontFamily(
                                "Cascadia Mono,Consolas,monospace"),
                            Foreground = new SolidColorBrush(
                                Color.Parse("#58D46C"))
                        },
                        new TextBlock
                        {
                            Text = exception.ToString(),
                            TextWrapping = TextWrapping.Wrap,
                            FontFamily = new FontFamily(
                                "Cascadia Mono,Consolas,monospace"),
                            Foreground = new SolidColorBrush(
                                Color.Parse("#D8DBD4"))
                        }
                    }
                }
            }
        };
}
