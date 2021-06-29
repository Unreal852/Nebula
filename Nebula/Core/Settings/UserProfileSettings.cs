using System;
using LiteMVVM.Messenger;
using Nebula.Core.Attributes;
using static Nebula.Core.Settings.AppSettings;

namespace Nebula.Core.Settings
{
    public class UserProfileSettings
    {
        private string _userName  = "User" + new Random().Next(1000, 10000);
        private string _avatarUrl = "https://i.imgur.com/zasbS3Z.jpeg";

        public UserProfileSettings()
        {
        }

        [LocalizedCategory("settings_profile"), LocalizedDisplayName("settings_profile_username")]
        public string UserName
        {
            get => _userName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    Set(ref _userName, "User" + new Random().Next(1000, 10000), true);
                else
                    Set(ref _userName, value, true);
                Messenger.Default.Broadcast(this, this);
            }
        }

        [LocalizedCategory("settings_profile"), LocalizedDisplayName("settings_profile_avatar")]
        public string AvatarUrl
        {
            get => _avatarUrl;
            set
            {
                Set(ref _avatarUrl, value, true);
                Messenger.Default.Broadcast(this, this);
            }
        }
    }
}