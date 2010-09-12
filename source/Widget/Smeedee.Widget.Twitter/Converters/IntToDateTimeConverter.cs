using System;
using System.Globalization;
using System.Windows.Data;

namespace Smeedee.Widget.Twitter.Converters
{
    public class IntToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var secondsAfterMidnight = ConvertToIntOrThrow(value);
            return DateTime.Today.AddSeconds(secondsAfterMidnight);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = ConvertToDateTimeOrThrow(value);
            return dateTime.Minute * 60 + dateTime.Second;
        }

        private DateTime ConvertToDateTimeOrThrow(object value)
        {
            DateTime dateTime;
            try
            {
                dateTime = (DateTime) value;
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("Must be able to cast argument to DateTime");
            }

            return dateTime;
        }

        private int ConvertToIntOrThrow(object value)
        {
            int intValue;
            try
            {
                intValue = (int) value;
            }
            catch(InvalidCastException e)
            {
                throw new ArgumentException("Must be able to cast argument to int");
            }

            if (intValue < 0)
                throw new ArgumentException("Cannot convert negative numbers");

            return intValue;
        }
    }
}
