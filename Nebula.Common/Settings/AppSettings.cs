using Ardalis.SmartEnum.SystemTextJson;
using Nebula.Common.Localization;
using System.Text.Json.Serialization;

namespace Nebula.Common.Settings;

public sealed class AppSettings
{
    public string Theme { get; set; } = "Dark";
    public string LocalLibraryPath { get; set; } = string.Empty;
    public string PartyUsername { get; set; } = $"UnknownUser{Random.Shared.Next(1000, 10000)}";
    public string PartyServerIp { get; set; } = "127.0.0.1";
    public int PartyServerPort { get; set; } = 9080;
    public uint? AccentColor { get; set; }

    [JsonConverter(typeof(SmartEnumValueConverter<LanguageInfo, string>))]
    public LanguageInfo Language { get; set; } = LanguageInfo.English;
}