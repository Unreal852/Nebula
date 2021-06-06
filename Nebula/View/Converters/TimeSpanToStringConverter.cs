using System;
using System.Globalization;
using System.Windows.Data;
using Nebula.Core.Extensions;

namespace Nebula.View.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan && targetType == typeof(string))
                return parameter is "txt" ? timeSpan.ToFormattedHuman() : timeSpan.ToSimpleFormattedHuman();
            return TimeSpan.Zero;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}