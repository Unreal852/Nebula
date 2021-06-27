using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Media;
using HandyControl.Themes;
using Nebula.Core.Attributes;
using Nebula.View.Controls;
using SharpToolbox.Safes;
using static Nebula.Core.Settings.AppSettings;

namespace Nebula.Core.Settings
{
    public class PersonalizationSettings
    {
        private ApplicationTheme _theme       = ApplicationTheme.Light;
        private SolidColorBrush  _themeAccent = new((Color) ColorConverter.ConvertFromString("#673AB7"));

        public PersonalizationSettings()
        {
        }

        [LocalizedCategory("settings_personalization_appearance"), LocalizedDisplayName("settings_personalization_appearance_theme")]
        public ApplicationTheme Theme
        {
            get => _theme;
            set
            {
                SetAndSave(ref _theme, value);
                ThemeManager.Current.ApplicationTheme = value;
            }
        }

        [Editor(typeof(ColorPickerPropertyEditor), typeof(UITypeEditor)),
         LocalizedCategory("settings_personalization_appearance"),
         LocalizedDisplayName("settings_personalization_appearance_theme_accent")]
        public SolidColorBrush ThemeAccent
        {
            get => _themeAccent;
            set
            {
                SetAndSave(ref _themeAccent, value);
                ThemeManager.Current.AccentColor = value;
            }
        }
    }
}