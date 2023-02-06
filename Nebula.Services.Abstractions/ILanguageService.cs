using Nebula.Common.Localization;

namespace Nebula.Services.Abstractions;

public interface ILanguageService
{
    LanguageInfo CurrentLanguage { get; }

    event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

    void SetLanguage(LanguageInfo language);

    string GetString(string key, string defaultValue = "Unknown");
}
