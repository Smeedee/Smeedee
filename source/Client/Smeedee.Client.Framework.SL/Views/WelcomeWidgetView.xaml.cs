using System.Windows;
using System.Windows.Controls;

namespace Smeedee.Client.Framework.SL
{
	public partial class WelcomeWidgetView : UserControl
	{
		public WelcomeWidgetView()
		{
			// Required to initialize variables
			InitializeComponent();
		}

        public void ShowFirstTimeHelp()
        {
            help.Visibility = Visibility.Visible;
            help2.Visibility = Visibility.Visible;
            help3.Visibility = Visibility.Visible;
            help4.Visibility = Visibility.Visible;
        }
	}
}