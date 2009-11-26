using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace APD.Client.Framework.SL.Controls
{
	public partial class LoadingAnimation : UserControl
	{
		public LoadingAnimation()
		{
			// Required to initialize variables
			InitializeComponent();

            VisualStateManager.GoToState(this, "Animating", true);
		}
	}
}