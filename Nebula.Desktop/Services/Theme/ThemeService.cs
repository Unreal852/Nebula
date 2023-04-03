using System;
using Avalonia;
using FluentAvalonia.Styling;
using Nebula.Desktop.Styles.Themes;
using Nebula.Common.Settings;
using Avalonia.Media;
using Nebula.Services.Contracts;

namespace Nebula.Desktop.Services.Theme;

public sealed class ThemeService : IThemeService
{
    private readonly ISettingsService    _settingsService;
    private readonly FluentAvaloniaTheme _avaloniathemeService;

    public ThemeService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _avaloniathemeService = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>() ??
                                throw new Exception($"Couldn't find {nameof(FluentAvaloniaTheme)} service");
        _avaloniathemeService.RequestedThemeChanged += OnAppThemeChanged;
        UpdateTheme(_settingsService.Settings);
    }

    private Avalonia.Styling.Styles ThemeStyle { get; set; } = default!;

    public string RequestedTheme
    {
        get => _avaloniathemeService.RequestedTheme;
        set => _avaloniathemeService.RequestedTheme = value;
    }

    public string[] AvailableThemes { get; } =
    {
            FluentAvaloniaTheme.DarkModeString, FluentAvaloniaTheme.LightModeString,
            FluentAvaloniaTheme.HighContrastModeString
    };

    public uint? RequestedAccentColor
    {
        get => _avaloniathemeService.CustomAccentColor?.ToUint32();
        set => _avaloniathemeService.CustomAccentColor = value.HasValue ? Color.FromUInt32(value.Value) : null;
    }

    public void UpdateTheme(string themeName)
    {
        RequestedTheme = themeName;
    }

    private void UpdateTheme(AppSettings settings)
    {
        if (settings.Theme != "auto")
            RequestedTheme = settings.Theme;

        if (settings.AccentColor != null)
            RequestedAccentColor = settings.AccentColor;
    }

    private void UpdateTheme()
    {
        Application.Current!.Styles.Remove(ThemeStyle);

        if (_avaloniathemeService.RequestedTheme == FluentAvaloniaTheme.DarkModeString)
            ThemeStyle = new NebulaDarkTheme();
        else if (_avaloniathemeService.RequestedTheme == FluentAvaloniaTheme.LightModeString)
            ThemeStyle = new NebulaLightTheme();

        if (ThemeStyle != null)
            Application.Current!.Styles.Add(ThemeStyle);
    }

    private void OnAppThemeChanged(FluentAvaloniaTheme sender, RequestedThemeChangedEventArgs args)
    {
        UpdateTheme();
    }
}