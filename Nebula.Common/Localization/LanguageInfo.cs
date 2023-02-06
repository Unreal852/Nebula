using System.Globalization;
using System.Text.Json.Serialization;

namespace Nebula.Common.Localization;

public sealed class LanguageInfo
{
    public static readonly LanguageInfo French = new() { Name = "Français", Code = "fr" };
    public static readonly LanguageInfo English = new() { Name = "English", Code = "en" };

    public static IEnumerable<LanguageInfo> Languages
    {
        get
        {
            yield return French;
            yield return English;
        }
    }

    private CultureInfo? _culture;

    public required string Name { get; init; }
    public required string Code { get; init; }

    [JsonIgnore]
    public CultureInfo Culture => _culture ??= new CultureInfo(Code);

    public static bool operator==(LanguageInfo left, LanguageInfo right)
    {
        return left.Code == right.Code;
    }

    public static bool operator !=(LanguageInfo left, LanguageInfo right)
    {
        return left.Code != right.Code;
    }
}
