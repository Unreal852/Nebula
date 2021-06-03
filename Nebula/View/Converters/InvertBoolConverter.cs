using System;
using System.Globalization;
using System.Windows.Data;

namespace Nebula.View.Converters
{
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");
            if (value == null || value.GetType() != typeof(bool))
                throw new InvalidOperationException("The value must be a boolean");
            return !(bool) value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}