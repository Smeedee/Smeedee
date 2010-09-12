using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Smeedee.Widget.Corkboard.SL
{
	public partial class RetrospectiveNoteView : UserControl
	{
		public RetrospectiveNoteView()
		{
			// Required to initialize variables
			InitializeComponent();
            Loaded += RetrospectiveNoteView_Loaded;
		}

	    private void RetrospectiveNoteView_Loaded(object sender, RoutedEventArgs e)
	    {
	       Description.Focus();
	    }
	}
}