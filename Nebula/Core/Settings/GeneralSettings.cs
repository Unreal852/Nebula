using System.ComponentModel.DataAnnotations;
using Nebula.Core.Attributes;
using SharpToolbox.Safes;
using static Nebula.Core.Settings.AppSettings;

namespace Nebula.Core.Settings
{
    public class GeneralSettings
    {
        private bool                _closeToTray                       = false;
        private bool                _keyboardMediaKeysSupport          = true;
        private bool                _allowDiscordIntegration           = true;
        private int                 _keyboardMediaKeysSoundIncDecValue = 5;
        private int                 _defaultVolume                     = 50;
        private int                 _searchMaxElements                 = 50;
        private int                 _playlistPageSize                  = 20;
        private ApplicationLanguage _appLanguage                       = ApplicationLanguage.Auto;

        public GeneralSettings()
        {
        }

        [LocalizedCategory("global_application"), LocalizedDisplayName("settings_general_close_to_tray")]
        public bool CloseToTray
        {
            get => _closeToTray;
            set => Set(ref _closeToTray, value);
        }

        [LocalizedCategory("global_application"), LocalizedDisplayName("settings_general_lang")]
        public ApplicationLanguage Language
        {
            get => _appLanguage;
            set
            {
                Set(ref _appLanguage, value);
                NebulaClient.Restart();
            }
        }

        [LocalizedCategory("settings_general_keyboard"), LocalizedDisplayName("settings_general_keyboard_support")]
        public bool KeyboardMediaKeysSupport
        {
            get => _keyboardMediaKeysSupport;
            set
            {
                Set(ref _keyboardMediaKeysSupport, value);
                if (!value)
                    NebulaClient.KeyboardHook?.UnHook();
                else
                    NebulaClient.KeyboardHook?.Hook();
            }
        }

        [LocalizedCategory("global_application"), LocalizedDisplayName("settings_general_discord")]
        public bool AllowDiscordIntegration
        {
            get => _allowDiscordIntegration;
            set
            {
                Set(ref _allowDiscordIntegration, value);
                if (!value)
                    NebulaClient.Discord?.ClearActivity();
                else
                    NebulaClient.Discord?.UpdateActivity();
            }
        }

        [LocalizedCategory("settings_general_keyboard"), LocalizedDisplayName("settings_general_keyboard_volume_incdec")]
        public int KeyboardMediaKeysSoundIncDevValue
        {
            get => _keyboardMediaKeysSoundIncDecValue;
            set => Set(ref _keyboardMediaKeysSoundIncDecValue, value);
        }

        [LocalizedCategory("global_application"), LocalizedDisplayName("settings_general_default_volume"), Range(0, 100)]
        public int DefaultVolume
        {
            get => _defaultVolume;
            set => Set(ref _defaultVolume, Ensure.Between(value, 0, 100));
        }

        [LocalizedCategory("global_application"), LocalizedDisplayName("settings_general_max_search_elements")]
        public int MaxSearchElements
        {
            get => _searchMaxElements;
            set => Set(ref _searchMaxElements, Ensure.Between(value, 10, 100));
        }

        [LocalizedCategory("global_application"), LocalizedDisplayName("settings_general_playlist_page_size")]
        public int PlaylistPageSize
        {
            get => _playlistPageSize;
            set => Set(ref _playlistPageSize, Ensure.Between(value, 5, 100));
        }
    }
}