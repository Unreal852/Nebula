using LiteMVVM.Navigation;
using Nebula.Core.Settings;

namespace Nebula.ViewModel
{
    public class SettingsViewModel : INavigable
    {
        public SettingsViewModel()
        {
        }

        public GeneralSettings         General         => NebulaClient.Settings.General;
        public PrivacySettings         Privacy         => NebulaClient.Settings.Privacy;
        public ServerSettings          Server          => NebulaClient.Settings.Server;
        public PersonalizationSettings Personalization => NebulaClient.Settings.Personalization;
        public UserProfileSettings     UserProfile     => NebulaClient.Settings.UserProfile;

        public void OnNavigated(object param)
        {
        }

        public void OnLeft()
        {
            AppSettings.SaveSettings();
            AppSettings.SaveProfile();
        }
    }
}