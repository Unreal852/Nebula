using Avalonia.Markup.Xaml;

namespace Nebula.Desktop.Styles.Themes;

public sealed class NebulaLightTheme : Avalonia.Styling.Styles
{
    public NebulaLightTheme()
    {
        AvaloniaXamlLoader.Load(this);
    }
}