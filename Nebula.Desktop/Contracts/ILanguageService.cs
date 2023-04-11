using Nebula.Common.Localization;
using System;

namespace Nebula.Desktop.Contracts;

public interface ILanguageService
{
    LanguageInfo CurrentLanguage { get; }

    event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

    void SetLanguage(LanguageInfo language);

    string GetString(string key, string defaultValue = "Unknown");
}
