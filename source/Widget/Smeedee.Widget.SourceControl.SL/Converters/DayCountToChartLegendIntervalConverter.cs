using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Smeedee.Widget.SourceControl.SL.Converters
{
    public class DayCountToChartLegendIntervalConverter : IValueConverter
    {
        private const double MAX_LABELS_PER_SLIDE = 21.0;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) 
                throw new NotImplementedException();

            int dayCount = (int)value;

            if (dayCount < 1)
                return 0;

            return Math.Ceiling(dayCount / MAX_LABELS_PER_SLIDE);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
