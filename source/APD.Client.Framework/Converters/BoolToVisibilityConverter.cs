﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace APD.Client.Framework.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && targetType == typeof(Visibility))
            {
                return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            }
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility && targetType == typeof(bool))
            {
                return ((Visibility)value == Visibility.Visible);
            }
            throw new NotImplementedException();
        }

        #endregion
    }
}
