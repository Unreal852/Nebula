using System.Globalization;
using Nebula.Core.Settings;

namespace Nebula.Core.Extensions
{
    public static class LanguageExtensions
    {
        public static CultureInfo GetCulture(this ApplicationLanguage language)
        {
            return language switch
            {
                ApplicationLanguage.English  => new CultureInfo("en-EN"),
                ApplicationLanguage.Français => new CultureInfo("fr-FR"),
                _                            => CultureInfo.CurrentUICulture
            };
        }
    }
}