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
using Smeedee.Client.Web.Services;
using Smeedee.Client.Widgets.SL.SourceControl.Views;
using Smeedee.Widget.Standard.SourceControl.ViewModel;
using Smeedee.Widget.Standard.SourceControl.Views;

namespace Smeedee.Client.Widgets.SL.SourceControl
{
    //[Export(typeof(Slide))]
    public class LatestCommitSlide : Slide
    {
        public LatestCommitSlide()
        {
            Title = "Latest commits";
            var configViewModel = new ConfigurationViewModel();
            SettingsView = new ConfigureVCS()
            {
                DataContext = configViewModel
            };

            var viewModel = new SourceControlViewModel();
            View = new LatestChangesView()
            {
                DataContext = viewModel
            };
        }
    }
}
