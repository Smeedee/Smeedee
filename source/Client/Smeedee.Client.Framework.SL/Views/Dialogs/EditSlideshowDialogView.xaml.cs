using System.Windows;
using System.Windows.Controls;

namespace Smeedee.Client.Framework.SL.Views.Dialogs
{
	public partial class EditSlideshowDialogView : UserControl
	{
        public EditSlideshowDialogView()
		{
			InitializeComponent();
            ScrollViewer.GotFocus += ScrollViewerParts_GotFocus;
           
		}

        void ScrollViewerParts_GotFocus(object sender, RoutedEventArgs e)
        {
            var element = e.OriginalSource as FrameworkElement;

            if (element != null)
            {
                var scrollviewer = sender as ScrollViewer;
                scrollviewer.ScrollIntoView(element);
            }
        }

        private void Slides_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           Slides.ScrollIntoView(Slides.SelectedItem);
        }
	}
}
