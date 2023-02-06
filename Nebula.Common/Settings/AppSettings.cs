using Nebula.Common.Localization;

namespace Nebula.Common.Settings;

public sealed class AppSettings
{
    public string       Theme            { get; set; } = "Dark";
    public string       LocalLibraryPath { get; set; } = string.Empty;
    public uint?        AccentColor      { get; set; }
    public LanguageInfo Language         { get; set; } = LanguageInfo.English;
}