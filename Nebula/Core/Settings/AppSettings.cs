using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using SharpToolbox.Safes;

namespace Nebula.Core.Settings
{
    public class AppSettings
    {
        private const          string        SettingsFileName  = "nebula_settings.json";
        private const          string        UserProfile       = "user_profile.json";
        public static readonly DirectoryInfo SettingsDirectory = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Nebula"));

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            IgnoreNullValues = true,
            WriteIndented = true,
            IgnoreReadOnlyProperties = true,
            Converters = {new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)}
        };

        static AppSettings()
        {
            if (!SettingsDirectory.Exists)
                SettingsDirectory.Create();
        }

        public static AppSettings LoadSettings()
        {
            string filePath = Path.Combine(SettingsDirectory.FullName, SettingsFileName);
            if (!File.Exists(filePath))
                SaveSettings(new AppSettings());
            SafeResult<AppSettings> result = Safe.Try(() =>
            {
                string fileContent = File.ReadAllText(filePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(fileContent, JsonOptions);
                return settings;
            });
            if (result.Success)
                return result.Result;
            File.Delete(filePath);
            return LoadSettings();
        }

        public static void SaveSettings(AppSettings settings = null)
        {
            string filePath = Path.Combine(SettingsDirectory.FullName, SettingsFileName);
            if (File.Exists(filePath))
                File.Delete(filePath);
            string json = JsonSerializer.Serialize(settings ?? NebulaClient.Settings, JsonOptions);
            File.WriteAllText(filePath, json);
        }

        public static void SetAndSave<T>(ref T storage, T value)
        {
            if (NebulaClient.Settings == null || EqualityComparer<object>.Default.Equals(storage, value))
                return;
            storage = value;
            SaveSettings();
        }

        public AppSettings()
        {
        }

        public GeneralSettings General { get; set; } = new();
        public PrivacySettings Privacy { get; set; } = new();
        public ServerSettings  Server  { get; set; } = new();
    }
}