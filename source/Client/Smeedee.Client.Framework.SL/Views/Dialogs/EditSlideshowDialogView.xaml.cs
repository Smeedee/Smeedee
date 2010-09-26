using System.Windows;
using System.Windows.Controls;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.Framework.ViewModel.Dialogs;

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

                //Todo: This was done by experimentation and is by all accounts an ugly hack! Research into a better solution should be undertaken!
                var slideshowDialog = this.DataContext as EditSlideshowDialog;
                
                if(slideshowDialog != null && slideshowDialog.Slideshow != null)
                {
                    var slideShow = slideshowDialog.Slideshow;
                    var currentSlide = slideShow.CurrentSlide;
                    var index = slideShow.Slides.IndexOf(currentSlide);

                    if (index <= 1)
                    {
                        scrollviewer.ScrollToHorizontalOffset(1);
                    }
                    else
                    {
                        scrollviewer.ScrollToHorizontalOffset( 138 * (index-1));
                    }
                }
            }
        }

        private void Slides_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           Slides.ScrollIntoView(Slides.SelectedItem);
        }
	}
}
