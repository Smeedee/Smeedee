using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Smeedee.Client.Framework.Converters
{
    /*Usage example:
     <UserControl.Resources>
        <[namespace_here]:BoolToBrushConverter x:Key="BoolToBrush" TrueValue="Red" FalseValue="Transparent"/>
     </UserControl.Resources>
     ...
     <TextBlock Foreground={Binding SomeBool, Converter={StaticResource BoolToBrush}} Text="This text changes color based on a bool" />
     */
    public class BoolToValueConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return FalseValue;
            else if (value is bool)
                return (bool)value ? TrueValue : FalseValue;
            else
                throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? value.Equals(TrueValue) : false;
        }
    }
    public class BoolToStringConverter : BoolToValueConverter<String> { }
    public class BoolToBrushConverter : BoolToValueConverter<Brush> { }
    public class BoolToDoubleConverter : BoolToValueConverter<Double> { }
    public class BoolToObjectConverter : BoolToValueConverter<Object> { }

    public class BoolNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = value is bool ? (bool) value : true;
            return !val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = value is bool ? (bool)value : false;
            return !val;
        }
    }
}
