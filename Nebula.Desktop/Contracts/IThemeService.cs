namespace Nebula.Desktop.Contracts;

public interface IThemeService
{
    string RequestedTheme { get; set; }
    string[] AvailableThemes { get; }
    uint? RequestedAccentColor { get; set; }

    void UpdateTheme(string themeName);
}