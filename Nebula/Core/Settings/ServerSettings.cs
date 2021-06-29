using Nebula.Core.Attributes;
using SharpToolbox.Safes;
using static Nebula.Core.Settings.AppSettings;

namespace Nebula.Core.Settings
{
    public class ServerSettings
    {
        private int    _serverPort          = 9080;
        private int    _serverSize          = 5;
        private string _serverIp            = "127.0.0.1";
        private string _serverConnectionKey = "";
        private bool   _useUpnp             = true;

        public ServerSettings()
        {
            
        }

        [LocalizedCategory("settings_server"), LocalizedDisplayName("settings_server_port")]
        public int ServerPort
        {
            get => _serverPort;
            set => Set(ref _serverPort, Ensure.GreaterThan(value, 1000));
        }
        
        [LocalizedCategory("settings_server"), LocalizedDisplayName("settings_server_size")]
        public int ServerSize
        {
            get => _serverSize;
            set => Set(ref _serverSize, Ensure.GreaterThan(value, 2));
        }

        [LocalizedCategory("settings_server"), LocalizedDisplayName("settings_server_ip")]
        public string ServerIp
        {
            get => _serverIp;
            set => Set(ref _serverIp, value);
        }

        [LocalizedCategory("settings_server"), LocalizedDisplayName("settings_server_key")]
        public string ServerConnectionKey
        {
            get => _serverConnectionKey;
            set => Set(ref _serverConnectionKey, value);
        }

        [LocalizedCategory("settings_server"), LocalizedDisplayName("settings_server_upnp")]
        public bool UseUpnp
        {
            get => _useUpnp;
            set => Set(ref _useUpnp, value);
        }
    }
}