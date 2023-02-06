using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Nebula.Common;
using Nebula.Desktop.Properties;

namespace Nebula.Desktop.Converters;

public class UpdateInfoToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is UpdateInfo updateInfo)
        {
            return updateInfo.UpdateAvailable ? Resources.SettingsUpdateNow : Resources.SettingsCheckForUpdates;
        }

        return Resources.SettingsCheckForUpdates;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}