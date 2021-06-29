using Nebula.Core.Attributes;
using static Nebula.Core.Settings.AppSettings;

namespace Nebula.Core.Settings
{
    public class PrivacySettings
    {
        private bool _allowAnalytics   = true;
        private bool _allowCrashReport = true;

        public PrivacySettings()
        {
            
        }

        [LocalizedCategory("settings_privacy"), LocalizedDisplayName("settings_privacy_allow_analytics")]
        public bool AllowAnalytics
        {
            get => _allowAnalytics;
            set => Set(ref _allowAnalytics, value);
        }

        [LocalizedCategory("settings_privacy"), LocalizedDisplayName("settings_privacy_allow_crash")]
        public bool AllowCrashReports
        {
            get => _allowCrashReport;
            set => Set(ref _allowCrashReport, value);
        }
    }
}