using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Smeedee.Client.Framework.Resources;

namespace Smeedee.Widgets.SL.SourceControl.Converters
{
    public class BackgroundToFontBrushConverter : IValueConverter
    {
        private object brightColor;
        private object darkColor;

        public BackgroundToFontBrushConverter()
        {
            brightColor = Application.Current.Resources["FontBrushBright"];
            darkColor = Application.Current.Resources["FontBrushBlack"];
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BrushProvider.IsBrightBrush(value.ToString()) ? darkColor : brightColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
