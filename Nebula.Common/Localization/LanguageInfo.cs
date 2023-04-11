using System.Globalization;
using Ardalis.SmartEnum;

namespace Nebula.Common.Localization;

public sealed class LanguageInfo : SmartEnum<LanguageInfo, string>
{
    public static readonly LanguageInfo French = new("Français", "fr");
    public static readonly LanguageInfo English = new("English", "en");

    private CultureInfo? _culture;

    private LanguageInfo(string name, string code) : base(name, code)
    {
    }

    public CultureInfo Culture => _culture ??= new CultureInfo(Value);
}
