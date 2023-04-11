using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using Nebula.Common;
using Nebula.Common.Localization;
using Nebula.Desktop.Contracts;
using Nebula.Desktop.Properties;
using Serilog;

namespace Nebula.Desktop.ViewModel;

public sealed partial class SettingsPageViewModel : ViewModelPageBase
{
    private readonly IAppService _appService;
    private readonly IThemeService _themeService;
    private readonly ISettingsService _settingsService;
    private readonly ILanguageService _languageService;
    private readonly IUpdateService _updateService;

    [ObservableProperty]
    private IReadOnlyList<string> _applicationThemes = default!;

    [ObservableProperty]
    private IReadOnlyList<LanguageInfo> _applicationLanguages = default!;

    [ObservableProperty]
    private LanguageInfo _currentApplicationLanguage = default!;

    [ObservableProperty]
    private Color _currentAccentColor;

    [ObservableProperty]
    private string _currentApplicationTheme = string.Empty;

    [ObservableProperty]
    private string _localLibraryPath = string.Empty;

    [ObservableProperty]
    private string _partyServerIp = string.Empty;

    [ObservableProperty]
    private string _partyUsername = string.Empty;

    [ObservableProperty]
    private int _partyServerPort = 0;

    [ObservableProperty]
    private UpdateInfo? _updateVersion;

    public SettingsPageViewModel(IAppService appService, IThemeService themeService, ILanguageService languageService,
                                 IUpdateService updateService,
                                 ISettingsService settingsService)
    {
        _appService = appService;
        _themeService = themeService;
        _settingsService = settingsService;
        _languageService = languageService;
        _updateService = updateService;

        PageName = Resources.PageSettings;
        PageIcon = Symbol.SettingsFilled;
        PageIsFooter = true;

        Initialize();
    }

    public string AppVersion { get; private set; } = default!;

    private void Initialize()
    {
        AppVersion = _appService.GetAppVersion();

        ApplicationThemes = _themeService.AvailableThemes;
        CurrentApplicationTheme = _themeService.RequestedTheme;
        CurrentAccentColor = _themeService.RequestedAccentColor.HasValue
                ? Color.FromUInt32(_themeService.RequestedAccentColor.Value)
                : Colors.Transparent;

        ApplicationLanguages = LanguageInfo.Languages.ToArray();
        CurrentApplicationLanguage = _languageService.CurrentLanguage;

        LocalLibraryPath = _settingsService.Settings.LocalLibraryPath;

        PartyServerIp = _settingsService.Settings.PartyServerIp;
        PartyServerPort = _settingsService.Settings.PartyServerPort;
        PartyUsername = _settingsService.Settings.PartyUsername;
    }

    [RelayCommand]
    private async Task Update()
    {
        if (UpdateVersion == null || UpdateVersion == UpdateInfo.UpToDate)
        {
            UpdateVersion = await _updateService.CheckForUpdates(AppVersion);
            OnPropertyChanged(nameof(UpdateVersion));
            return;
        }

        await _updateService.DownloadUpdate(UpdateVersion);
    }

    [RelayCommand]
    private async Task SelectLocalLibrary()
    {
        var result = await MainWindow.Instance.StorageProvider.OpenFolderPickerAsync(
                new Avalonia.Platform.Storage.FolderPickerOpenOptions()
                {
                    Title = "Select local library",
                    AllowMultiple = false
                });

        if (result.Count == 1)
        {
            LocalLibraryPath = result[0].Path.ToString();
        }
    }

    partial void OnCurrentApplicationThemeChanged(string value)
    {
        var thm = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
        if (thm == null)
        {
            Log.Error("Failed to retrieve {Service} service", typeof(FluentAvaloniaTheme));
            return;
        }

        thm.RequestedTheme = value;
        _settingsService.Settings.Theme = value;
        _settingsService.SaveSettings();
    }

    partial void OnCurrentApplicationLanguageChanged(LanguageInfo value)
    {
        _languageService.SetLanguage(value);
        _settingsService.Settings.Language = value;
        _settingsService.SaveSettings();
    }

    partial void OnCurrentAccentColorChanged(Color value)
    {
        var thm = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
        if (thm == null)
        {
            Log.Error("Failed to retrieve {Service} service", typeof(FluentAvaloniaTheme));
            return;
        }

        thm.CustomAccentColor = value;
        _settingsService.Settings.AccentColor = value.ToUint32();
        _settingsService.SaveSettings();
    }

    partial void OnLocalLibraryPathChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Directory.Exists(value))
            return;
        _settingsService.Settings.LocalLibraryPath = value;
        _settingsService.SaveSettings();
    }

    partial void OnPartyServerIpChanged(string value)
    {
        if (!IPAddress.TryParse(value, out _))
            return;
        _settingsService.Settings.PartyServerIp = value;
        _settingsService.SaveSettings();
    }

    partial void OnPartyServerPortChanged(int value)
    {
        _settingsService.Settings.PartyServerPort = value;
        _settingsService.SaveSettings();
    }

    partial void OnPartyUsernameChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 3)
            return;
        _settingsService.Settings.PartyUsername = value;
        _settingsService.SaveSettings();
    }
}