using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


namespace Smeedee.Client.Framework.SL.Converters
{
    public class StringToResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Application.Current.Resources[value.ToString()] : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
