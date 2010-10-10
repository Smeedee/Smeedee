using System;
using System.Windows;
using System.Windows.Controls;

namespace Smeedee.Widgets.SL.Corkboard.Views
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