using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.ViewModel;

namespace Smeedee.Client.Framework.SL.Views.Dialogs
{
    public partial class SelectWidgetsDialogView : UserControl
    {
        private bool isChanging;

        public SelectWidgetsDialogView()
        {
            InitializeComponent();
            txtSearch.Focus();
        }

        private void SetIsDescriptionCappedForItems(IList items, bool setting)
        {
            if (items == null)
                return;

            foreach (var item in items)
            {
                var listBoxItem = GetChildObject<ListBoxItem>(list, item);

                if (listBoxItem != null)
                {
                    var widgetMetadataItem = item as WidgetMetadata;
                    if (widgetMetadataItem != null && widgetMetadataItem.IsSelected == false)
                        widgetMetadataItem.IsDescriptionCapped = setting;
                }
            }
        }

        private T GetChildObject<T>(FrameworkElement obj, object context) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                FrameworkElement c = VisualTreeHelper.GetChild(obj, i) as FrameworkElement;
                if (c.GetType().Equals(typeof(T)) && (c.DataContext != null && ((FrameworkElement)c).DataContext.Equals(context)))
                {
                    return (T)c;
                }
                FrameworkElement gc = GetChildObject<T>(c, context);
                if (gc != null)
                    return (T)gc;
            }
            return null;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var link = sender as HyperlinkButton;
            if (link != null && link.Content != null)
            {
                txtSearch.Text = link.Content.ToString();
            }
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetIsDescriptionCappedForItems(e.AddedItems, false);
            SetIsDescriptionCappedForItems(e.RemovedItems, true);
        }
    }
}
