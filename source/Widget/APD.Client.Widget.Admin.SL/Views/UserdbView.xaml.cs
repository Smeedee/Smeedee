using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using APD.Client.Widget.Admin.ViewModels;
using APD.Client.Framework.SL;
using APD.Client.Framework.ViewModels;

namespace APD.Client.Widget.Admin.SL
{
	public partial class UserdbView : UserControl
	{
		public UserdbView()
		{
			// Required to initialize variables
			InitializeComponent();

            this.Loaded += new RoutedEventHandler(UserdbView_Loaded);
        }

        void UserdbView_Loaded(object sender, RoutedEventArgs e)
        {

        }
	}
}