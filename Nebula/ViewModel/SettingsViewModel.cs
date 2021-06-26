using Nebula.Core.Settings;

namespace Nebula.ViewModel
{
    public class SettingsViewModel
    {
        public SettingsViewModel()
        {
        }

        public GeneralSettings General => NebulaClient.Settings.General;
        public PrivacySettings Privacy => NebulaClient.Settings.Privacy;
        public ServerSettings  Server  => NebulaClient.Settings.Server;
    }
}