using Nebula.Common.Settings;
using System;

namespace Nebula.Desktop.Contracts;

public interface ISettingsService
{
    AppSettings Settings { get; }

    event EventHandler<EventArgs>? SettingsChanged;

    void SaveSettings();

    void ResetSettings();
}
