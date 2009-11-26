using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using APD.Client.Widget.SourceControl.ViewModels;

namespace APD.Client.Widget.SourceControl.SL.Views
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

            CheckIfCommentIsBad();
        }

        private void CheckIfCommentIsBad()
        {
            var viewModel = DataContext as ChangesetViewModel;
            if (viewModel != null)
            {
                if (viewModel.CommentIsBad)
                    GoToCommentIsBadState();
            }
        }

        private void GoToCommentIsBadState()
        {
            VisualStateManager.GoToState(this, "CommentIsBad", true);
            
        }
    }
}
