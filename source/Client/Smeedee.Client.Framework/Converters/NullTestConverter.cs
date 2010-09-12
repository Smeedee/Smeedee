using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Smeedee.Client.Framework.Converters
{
    /*Usage example:
     <UserControl.Resources>
        <[namespace_here]:NullTestToBrushConverter x:Key="NullTestToBrush" NullValue="Red" NotNullValue="Transparent"/>
     </UserControl.Resources>
     ...
     <TextBlock Foreground={Binding SomeBool, Converter={StaticResource NullTestToBrush}} Text="This text changes color based on a NullTest" />
     */
    public class NullTestToValueConverter<T> : IValueConverter
    {
        public T NullValue { get; set; }
        public T NotNullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null) ? NullValue : NotNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NullTestToStringConverter : NullTestToValueConverter<String> { }
    public class NullTestToBrushConverter : NullTestToValueConverter<Brush> { }
    public class NullTestToDoubleConverter : NullTestToValueConverter<Double> { }
    public class NullTestToObjectConverter : NullTestToValueConverter<Object> { }
}
