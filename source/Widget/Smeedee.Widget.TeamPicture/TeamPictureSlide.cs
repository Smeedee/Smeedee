using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Widget.TeamPicture.ViewModel;
using Smeedee.Widget.TeamPicture.Views;

namespace Smeedee.Widget.TeamPicture
{
    [Export(typeof(Slide))]
    public class TeamPictureSlide : Slide
    {
        public TeamPictureSlide()
        {
            Title = "Team Picture";
            var viewModel = new TeamPictureViewModel();
            SettingsView = new TeamPictureSettingsView()
            {
                DataContext = viewModel
            };
            View = new TeamPictureView()
            {
                DataContext = viewModel
            };
        }
    }
}
