using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace Smeedee.Client.Framework.Converters
{
    public class InvertedBoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && targetType == typeof(Visibility))
            {
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility && targetType == typeof(bool))
            {
                return ((Visibility)value == Visibility.Collapsed);
            }
            throw new NotImplementedException();
        }

        #endregion
    }
}