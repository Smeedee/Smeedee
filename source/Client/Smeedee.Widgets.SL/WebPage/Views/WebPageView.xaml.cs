using System.Windows;
using System.Windows.Controls;

namespace Smeedee.Widgets.SL.WebPage.Views
{
    public partial class WebPageView : UserControl
    {
        public WebPageView()
        {
            InitializeComponent();
        }

		public void HideWebBrowser()
		{
			Dispatcher.BeginInvoke(() =>
			{
				webBrowser.Visibility = Visibility.Collapsed;
			});
		}

		public void ShowWebBrowser()
		{
			Dispatcher.BeginInvoke(() =>
			{
				webBrowser.Visibility = Visibility.Visible;
			});
		}
    }
}
