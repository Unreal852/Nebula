using System;
using System.Collections;
using System.Globalization;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Nebula.Common.Localization;
using Nebula.Desktop.Contracts;
using Nebula.Desktop.Properties;

namespace Nebula.Services.Localization;

public sealed class LanguageService : ILanguageService
{
    private readonly ISettingsService _settingsService;

    public LanguageService(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        if (LanguageInfo.TryFromValue(_settingsService.Settings.Language, out var language))
            SetLanguage(language);
        else
            SetLanguage(LanguageInfo.English);
    }

    private IResourceDictionary? LanguageDictionary { get; set; }
    public LanguageInfo CurrentLanguage { get; private set; } = default!;

    public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

    public void SetLanguage(LanguageInfo language)
    {
        Thread.CurrentThread.CurrentUICulture = language.Culture;

        if (LanguageDictionary != null)
        {
            Application.Current!.Resources.MergedDictionaries.Remove(LanguageDictionary);
            LanguageDictionary.Clear();
        }
        else
            LanguageDictionary = new ResourceDictionary();

        var defaultResources = Resources.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);

        foreach (DictionaryEntry entry in defaultResources!)
        {
            if (entry.Key == null)
                continue;
            LanguageDictionary.Add($"Loc{entry.Key}", Resources.ResourceManager.GetString((string)entry.Key, language.Culture));
        }

        Application.Current!.Resources.MergedDictionaries.Add(LanguageDictionary);

        var old = CurrentLanguage;
        CurrentLanguage = language;
        LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(old, CurrentLanguage));
        // TODO: Maybe we can update it without removing and readding it everytimes ? 
    }

    public string GetString(string key, string defaultValue = "Unknown")
    {
        return Resources.ResourceManager.GetString(key, CurrentLanguage.Culture) ?? defaultValue;
    }
}
