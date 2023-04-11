using Nebula.Common.Settings;

namespace Nebula.Services.Contracts;

public interface ISettingsService
{
    AppSettings Settings { get; }

    event EventHandler<EventArgs>? SettingsChanged;

    void SaveSettings();

    void ResetSettings();
}
