using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Nebula.Core.Attributes;
using SharpToolbox.Safes;
using static Nebula.Core.Settings.AppSettings;

namespace Nebula.Core.Settings
{
    public class GeneralSettings
    {
        private bool _closeToTray                       = false;
        private bool _keyboardMediaKeysSupport          = true;
        private int  _keyboardMediaKeysSoundIncDecValue = 5;
        private int  _defaultVolume                     = 50;
        private int  _searchMaxElements                 = 50;
        private int  _playlistPageSize                  = 20;

        public GeneralSettings()
        {
        }

        [LocalizedCategory("global_application"), LocalizedDisplayName("settings_general_close_to_tray")]
        public bool CloseToTray
        {
            get => _closeToTray;
            set => SetAndSave(ref _closeToTray, value);
        }

        [LocalizedCategory("settings_general_keyboard"), LocalizedDisplayName("settings_general_keyboard_support")]
        public bool KeyboardMediaKeysSupport
        {
            get => _keyboardMediaKeysSupport;
            set
            {
                SetAndSave(ref _keyboardMediaKeysSupport, value);
                if (!value)
                    NebulaClient.KeyboardHook?.UnHook();
                else
                    NebulaClient.KeyboardHook?.Hook();
            }
        }

        [LocalizedCategory("settings_general_keyboard"), LocalizedDisplayName("settings_general_keyboard_volume_incdec")]
        public int KeyboardMediaKeysSoundIncDevValue
        {
            get => _keyboardMediaKeysSoundIncDecValue;
            set => SetAndSave(ref _keyboardMediaKeysSoundIncDecValue, value);
        }

        [LocalizedCategory("global_application"), LocalizedDisplayName("settings_general_default_volume"), Range(0,100)]
        public int DefaultVolume
        {
            get => _defaultVolume;
            set => SetAndSave(ref _defaultVolume, Ensure.Between(value, 0,100));
        }

        [LocalizedCategory("global_application"), LocalizedDisplayName("settings_general_max_search_elements")]
        public int MaxSearchElements
        {
            get => _searchMaxElements;
            set => SetAndSave(ref _searchMaxElements, Ensure.Between(value, 10,100));
        }

        [LocalizedCategory("global_application"), LocalizedDisplayName("settings_general_playlist_page_size")]
        public int PlaylistPageSize
        {
            get => _playlistPageSize;
            set => SetAndSave(ref _playlistPageSize, Ensure.Between(value, 5,100));
        }
    }
}