using System.Text;

namespace IndustrialControls.Avalonia.Demo;

internal static class DemoStartupDiagnostics
{
    private const string DirectoryName =
        "IndustrialControls.Avalonia.Demo";

    public static string LogPath
    {
        get
        {
            var baseDirectory = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);

            return Path.Combine(
                baseDirectory,
                DirectoryName,
                "startup-error.log");
        }
    }

    public static void WriteException(
        string phase,
        Exception exception)
    {
        try
        {
            var path = LogPath;
            var directory = Path.GetDirectoryName(path);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var report = new StringBuilder();
            report.AppendLine("IndustrialControls.Avalonia Demo");
            report.AppendLine($"UTC: {DateTimeOffset.UtcNow:O}");
            report.AppendLine($"Phase: {phase}");
            report.AppendLine();
            report.AppendLine(exception.ToString());

            File.WriteAllText(
                path,
                report.ToString(),
                Encoding.UTF8);
        }
        catch
        {
            // Startup diagnostics must never hide the original exception.
        }
    }
}
