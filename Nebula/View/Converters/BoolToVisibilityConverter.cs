using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Nebula.View.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a Visibility");
            if (value is bool visibilityValue)
            {
                if (parameter is "!")
                    visibilityValue = !visibilityValue;
                return visibilityValue ? Visibility.Visible : Visibility.Collapsed;
            }

            throw new InvalidOperationException("The value must be a boolean");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
                return visibility == Visibility.Visible;
            return false;
        }
    }
}