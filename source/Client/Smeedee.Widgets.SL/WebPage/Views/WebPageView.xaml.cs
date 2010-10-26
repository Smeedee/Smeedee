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
			webBrowser.Visibility = Visibility.Collapsed;
		}

		public void ShowWebBrowser()
		{
			webBrowser.Visibility = Visibility.Visible;
		}
    }
}
