using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Smeedee.Widget.Admin.Tasks.ViewModels;

namespace Smeedee.Widget.Admin.Tasks.SL.Converters
{
    public class ConfigEntryControlConverter : IValueConverter
    {
        public const string XAML_NAMESPACE = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        public const string CUSTOM_TEXTBOX_NAMESPACE =
            "clr-namespace:Smeedee.Client.Framework.SL;assembly=Smeedee.Client.Framework.SL";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var configEntry = value as ConfigurationEntryViewModel;
            if (configEntry == null) return null;

            string xaml = "<DataTemplate xmlns='" + XAML_NAMESPACE + "'>" +
                               CreateXaml(configEntry) +
                          "</DataTemplate>";

            var dataTemplate = (DataTemplate) XamlReader.Load(xaml);
            return dataTemplate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "converted back";
        }

        private string CreateXaml(ConfigurationEntryViewModel configEntry)
        {
            switch (configEntry.Type.FullName)
            {
                case "System.String":
                    return "<SL:SelectAllOnFocusTextBox xmlns:SL='" + CUSTOM_TEXTBOX_NAMESPACE + "' Text='{Binding Value, Mode=TwoWay}' Width='150' />";
                case "System.Boolean":
                    return "<CheckBox IsChecked='{Binding Value, Mode=TwoWay}' />";
            }
            return "";
        }
    }
}