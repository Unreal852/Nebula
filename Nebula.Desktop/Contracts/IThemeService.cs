using Avalonia.Styling;

namespace Nebula.Desktop.Contracts;

public interface IThemeService
{
    ThemeVariant ActualTheme { get; set; }
    ThemeVariant[] AvailableThemes { get; }
    uint? ActualAccentColor { get; set; }
}