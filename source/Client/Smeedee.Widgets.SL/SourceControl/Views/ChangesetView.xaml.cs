using System.Windows;
using System.Windows.Controls;
using Smeedee.Widget.SourceControl.ViewModels;

namespace Smeedee.Widget.SourceControl.SL.Views
{
    public partial class ChangesetView : UserControl
    {
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
