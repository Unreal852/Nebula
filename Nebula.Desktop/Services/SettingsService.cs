using System;
using System.IO;
using System.Text.Json;
using Nebula.Common.Settings;
using Nebula.Desktop.Contracts;
using Serilog;

namespace Nebula.Desktop.Services;

public sealed class SettingsService : ISettingsService
{
    // TODO: Add an event when the settings are changed

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly ILogger _logger;
    private readonly string _settingsPath = default!;

    public SettingsService(ILogger logger)
    {
        _logger = logger.WithContext(nameof(SettingsService));
        _settingsPath = Path.Combine(AppContext.BaseDirectory, "settings", "appsettings.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
        LoadOrCreateSettings();
    }

    public AppSettings Settings { get; private set; } = default!;

    public event EventHandler<EventArgs>? SettingsChanged;

    private void LoadOrCreateSettings()
    {
        if (File.Exists(_settingsPath))
        {
            var fileContent = File.ReadAllText(_settingsPath);
            try
            {
                Settings = JsonSerializer.Deserialize<AppSettings>(fileContent, _serializerOptions) ?? throw new Exception("Failed to read app settings");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to read settings. Resetting to default");
                ResetSettings();
            }
        }
        else
        {
            Settings = new AppSettings();
            SaveSettings();
        }
    }

    public void SaveSettings()
    {
        var serialized = JsonSerializer.Serialize(Settings, _serializerOptions);
        if (File.Exists(_settingsPath))
            File.Delete(_settingsPath);
        File.WriteAllText(_settingsPath, serialized);
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ResetSettings()
    {
        if (File.Exists(_settingsPath))
            File.Delete(_settingsPath);
        LoadOrCreateSettings();
    }
}
