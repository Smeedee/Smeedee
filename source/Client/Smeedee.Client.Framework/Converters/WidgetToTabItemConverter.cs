using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Client.Framework.Converters
{
    class WidgetToTabItemConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var source = value as ObservableCollection<Widget>;
            if (source != null)
            {
                var result = new List<TabItem>();

                foreach (Widget widget in source)
                {
                    result.Add(new TabItem()
                    {
                        Header = widget.Title,
                        Content = widget.View
                    });
                }
                return result;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
