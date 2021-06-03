using System;
using System.Globalization;
using System.Windows.Data;
using Nebula.Utils.Extensions;

namespace Nebula.View.Converters
{
    public class DoubleToTimeSpanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dValue)
                return TimeSpan.FromSeconds(dValue).ToFormattedHuman();
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}