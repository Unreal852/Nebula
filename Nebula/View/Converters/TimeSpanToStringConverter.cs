using System;
using System.Globalization;
using System.Windows.Data;
using Nebula.Utils.Extensions;

namespace Nebula.View.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan && targetType == typeof(string))
                return timeSpan.ToFormattedHuman();
            return TimeSpan.Zero;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}