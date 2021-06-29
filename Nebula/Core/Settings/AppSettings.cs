using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Nebula.Core.Json;
using SharpToolbox.Safes;

namespace Nebula.Core.Settings
{
    public class AppSettings
    {
        public const           string        SettingsFileName = "nebula_settings.json";
        public const           string        UserProfileFileName = "user_profile.json";
        public const           string        PlaylistDatabaseFileName = "user_playlists.db";
        public static readonly DirectoryInfo SettingsDirectory = new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Nebula"));

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            IgnoreNullValues = true,
            IgnoreReadOnlyProperties = true,
            WriteIndented = true,
            Converters = {new JsonStringEnumConverter(JsonNamingPolicy.CamelCase), new SolidColorBrushJsonConverter()}
        };

        static AppSettings()
        {
        }

        private static T SafeDeserialize<T>(string filePath) where T : new()
        {
            SafeResult<T> result = Deserialize<T>(filePath);
            if (result.IsSuccess)
                return result.Result;
            Serialize(filePath, new T());
            result = Deserialize<T>(filePath);
            if (!result.IsSuccess)
                throw result.Exception;
            return result.Result;
        }

        private static SafeResult<T> Deserialize<T>(string filePath) where T : new()
        {
            if (!File.Exists(filePath))
                return SafeResult.Failed<T>(new FileNotFoundException("File not found", filePath));
            return Safe.Try(() =>
            {
                string fileContent = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(fileContent, JsonOptions);
            });
        }

        private static void Serialize<T>(string filePath, T value) where T : new()
        {
            FileInfo fileInfo = new FileInfo(filePath);
            fileInfo.Directory?.Create();
            if (fileInfo.Exists)
                fileInfo.Delete();
            string json = JsonSerializer.Serialize(value, JsonOptions);
            File.WriteAllText(filePath, json);
        }

        public static AppSettings LoadSettings()
        {
            var settings = SafeDeserialize<AppSettings>(Path.Combine(SettingsDirectory.FullName, SettingsFileName));
            settings.UserProfile = SafeDeserialize<UserProfileSettings>(Path.Combine(SettingsDirectory.FullName, UserProfileFileName));
            return settings;
        }

        public static void SaveSettings(AppSettings settings = null)
        {
            Serialize(Path.Combine(SettingsDirectory.FullName, SettingsFileName), settings ?? NebulaClient.Settings);
        }

        public static void SaveProfile(UserProfileSettings userprofile = null)
        {
            Serialize(Path.Combine(SettingsDirectory.FullName, UserProfileFileName), userprofile ?? NebulaClient.Settings?.UserProfile ?? new UserProfileSettings());
        }

        public static void Set<T>(ref T storage, T value, bool isProfile = false)
        {
            if (EqualityComparer<object>.Default.Equals(storage, value))
                return;
            storage = value;
            if (NebulaClient.Settings is not {SaveOnChange: true})
                return;
            if (isProfile)
                SaveProfile(NebulaClient.Settings.UserProfile);
            else
                SaveSettings();
        }

        public AppSettings()
        {
        }

        public              GeneralSettings         General         { get; set; } = new();
        public              PrivacySettings         Privacy         { get; set; } = new();
        public              ServerSettings          Server          { get; set; } = new();
        public              PersonalizationSettings Personalization { get; set; } = new();
        [JsonIgnore] public UserProfileSettings     UserProfile     { get; set; } = new();
        [JsonIgnore] public bool                    SaveOnChange    { get; set; } = false;
    }
}