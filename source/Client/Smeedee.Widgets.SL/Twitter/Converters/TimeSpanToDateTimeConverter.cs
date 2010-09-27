using System;
using System.Globalization;
using System.Windows.Data;

namespace Smeedee.Widgets.SL.Twitter.Converters
{
    public class TimeSpanToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = (TimeSpan) value;
            return DateTime.Today.AddSeconds(timeSpan.TotalSeconds);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime) value;
            return new TimeSpan(0, dateTime.Minute, dateTime.Second);
        }
    }
}
