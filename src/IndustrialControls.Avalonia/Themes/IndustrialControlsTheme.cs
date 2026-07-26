using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace IndustrialControls.Avalonia.Themes;

/// <summary>
/// Carica il tema Industrial90 della libreria.
/// </summary>
public sealed class IndustrialControlsTheme : Styles
{
    public IndustrialControlsTheme(IServiceProvider? serviceProvider = null)
    {
        AvaloniaXamlLoader.Load(serviceProvider, this);
    }
}
