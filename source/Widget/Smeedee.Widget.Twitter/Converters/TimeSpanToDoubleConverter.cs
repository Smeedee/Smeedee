using System;
using System.Globalization;
using System.Windows.Data;

namespace Smeedee.Widget.Twitter.Converters
{
    public class TimeSpanToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = (TimeSpan) value;
            return timeSpan.TotalSeconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var doubleValue = (double) value;
            return new TimeSpan(0, 0, (int) doubleValue);
        }
    }
}
