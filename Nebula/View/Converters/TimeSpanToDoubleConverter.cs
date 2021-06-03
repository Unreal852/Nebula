using System;
using System.Globalization;
using System.Windows.Data;

namespace Nebula.View.Converters
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan && targetType == typeof(double))
                return timeSpan.TotalSeconds;
            return TimeSpan.Zero;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double timeValue && targetType == typeof(TimeSpan))
                return TimeSpan.FromSeconds(timeValue);
            return TimeSpan.Zero;
        }
    }
}