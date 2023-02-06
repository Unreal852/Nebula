using Avalonia.Markup.Xaml;

namespace Nebula.Desktop.Styles.Themes;

public sealed class NebulaDarkTheme : Avalonia.Styling.Styles
{
    public NebulaDarkTheme()
    {
        AvaloniaXamlLoader.Load(this);
    }
}