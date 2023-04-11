using System;
using Avalonia;
using FluentAvalonia.Styling;
using Avalonia.Media;
using Nebula.Desktop.Contracts;
using Avalonia.Styling;

namespace Nebula.Desktop.Services.Theme;

public sealed class ThemeService : IThemeService
{
    private readonly ISettingsService _settingsService;
    private readonly FluentAvaloniaTheme _avaloniathemeService;

    public ThemeService(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _avaloniathemeService = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>() ?? throw new Exception($"Couldn't find {nameof(FluentAvaloniaTheme)} service");

        ActualTheme = _settingsService.Settings.Theme switch
        {
            "Light" => ThemeVariant.Light,
            "Dark" => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }

    private Avalonia.Styling.Styles ThemeStyle { get; set; } = default!;

    public ThemeVariant[] AvailableThemes { get; } =
{
            ThemeVariant.Default, ThemeVariant.Light, ThemeVariant.Dark
    };

    public ThemeVariant ActualTheme
    {
        get => Application.Current!.ActualThemeVariant;
        set
        {
            Application.Current!.RequestedThemeVariant = value;
            ActualAccentColor = _settingsService.Settings.AccentColor;
        }
    }

    public uint? ActualAccentColor
    {
        get => _avaloniathemeService.CustomAccentColor?.ToUint32();
        set => _avaloniathemeService.CustomAccentColor = value.HasValue ? Color.FromUInt32(value.Value) : null;
    }
}