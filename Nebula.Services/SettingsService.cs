using System.Text.Json;
using Nebula.Common.Settings;
using Nebula.Services.Abstractions;

namespace Nebula.Services;
public sealed class SettingsService : ISettingsService
{
    // TODO: Add an event when the settings are changed

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
    };

    private readonly string _settingsPath = default!;

    public SettingsService()
    {
        _settingsPath = Path.Combine(AppContext.BaseDirectory, "settings", "appsettings.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
        LoadOrCreateSettings();
    }

    public AppSettings Settings { get; private set; } = default!;

    public event EventHandler<EventArgs>? SettingsChanged;

    private void LoadOrCreateSettings()
    {
        if(File.Exists(_settingsPath))
        {
            var fileContent = File.ReadAllText(_settingsPath);
            Settings = JsonSerializer.Deserialize<AppSettings>(fileContent, _serializerOptions) ?? throw new Exception("Failed to read app settings");
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
        if(File.Exists(_settingsPath))
            File.Delete(_settingsPath);
        File.WriteAllText(_settingsPath, serialized);
        SettingsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ResetSettings()
    {
        if(File.Exists(_settingsPath))
            File.Delete(_settingsPath);
        LoadOrCreateSettings();
    }
}
