using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ResolutionVigenere.View.Converters
{
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverseParam = parameter.ToString().ToLower() == "inverse";

            if (value is bool)
            {
                bool boolValue = (bool) value;

                if (inverseParam)
                    return boolValue ? Visibility.Collapsed : Visibility.Visible;

                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
