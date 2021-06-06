using System;
using System.Globalization;
using System.Windows.Data;
using Nebula.Core.Extensions;

namespace Nebula.View.Converters
{
    public class DoubleToTimeSpanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double dValue)
                return TimeSpan.FromSeconds(dValue).ToSimpleFormattedHuman();
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}