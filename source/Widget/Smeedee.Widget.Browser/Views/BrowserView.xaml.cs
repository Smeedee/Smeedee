using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Smeedee.Widget.Browser.Views
{
    public partial class BrowserView : UserControl
    {
        public BrowserView()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(BrowserView_Loaded);
        }

        void BrowserView_Loaded(object sender, RoutedEventArgs e)
        {
            var browserViewModel = DataContext as ViewModel.Browser;

            if (browserViewModel != null)
            {
                browserViewModel.PropertyChanged += (o, ee) =>
                {
                    if (ee.PropertyName == "Url")
                        webBrowser.Navigate(new Uri(browserViewModel.Url));
                };
            }
        }
    }
}
