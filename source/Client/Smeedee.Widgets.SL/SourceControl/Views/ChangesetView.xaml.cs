using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Smeedee.Widget.DeveloperInfo.SL.Views;
using Smeedee.Widgets.SL.SourceControl.Converters;
using Smeedee.Widgets.SourceControl.ViewModels;

namespace Smeedee.Widgets.SL.SourceControl.Views
{
    public partial class ChangesetView : UserControl
    {
        private static BackgroundToFontBrushConverter bgToBrushConverter = new BackgroundToFontBrushConverter();

        public ChangesetView()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(ChangesetView_Loaded);
        }

        void ChangesetView_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "IdleState", true);
            
            if (ChangeSetShouldBlink())
            {
                GoToCommentIsBadState();
            }

            SetDarkFontColorIfBrightBackground();
        }

        private void SetDarkFontColorIfBrightBackground()
        {
            var viewModel = DataContext as ChangesetViewModel;
            if (viewModel != null)
            {
                var brush = bgToBrushConverter.Convert(viewModel.BackgroundColor, typeof(Brush), null, CultureInfo.CurrentUICulture);


                var border = (Border) VisualTreeHelper.GetChild(this, 0);
                if (border != null)
                {
                    var grid = (Grid) border.FindName("LayoutRoot");
                    if (grid != null)
                    {
                        var developer = (UserInfo) grid.FindName("DeveloperInfo");
                        if (developer != null)
                            developer.TextBrush = (Brush) brush;
                    }
                }
            }
        }

        public bool ChangeSetShouldBlink()
        {
            var viewModel = DataContext as ChangesetViewModel;
            if (viewModel != null)
            {
                return viewModel.ShouldBlink && viewModel.CommentIsBad;
            }
            return false;
        }

        private void GoToCommentIsBadState()
        {
            VisualStateManager.GoToState(this, "CommentIsBad", true);
        }
    }
}
