using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Nebula.Common.Audio;

namespace Nebula.Desktop.Converters;

public sealed class AudioStateToEnabledConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AudioServiceState state) return state != AudioServiceState.Stopped;

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}