using System;
using System.ComponentModel;
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
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widgets.SL.WebSnapshot.Views;
using Smeedee.Widgets.WebSnapshot.ViewModel;

namespace Smeedee.Widgets.SL.WebSnapshot
{
    [WidgetInfo(
        Name = "Web Snapshot",
        Description = "Takes snapshots of web pages or display a picture from an URL, cropable.",
        Author = "SoC 2011",
        Version = "0.1",
        Tags = new[] { CommonTags.Fun})]
    public class WebSnapshotWidget : Client.Framework.ViewModel.Widget
    {
        private WebSnapshotViewModel viewModel;

        public WebSnapshotWidget()
        {
            Title = "Web Snapshot";
            viewModel = GetInstance<WebSnapshotViewModel>();
            viewModel.PropertyChanged += ViewModelPropertyChanged;

            SettingsView = new WebSnapshotSettingsView {DataContext = viewModel};
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tempViewModel = sender as WebSnapshotSettingsViewModel;
            var isDoneSaving = (tempViewModel != null && e.PropertyName.Equals("IsSaving") && !tempViewModel.IsSaving);

            if (isDoneSaving && IsInSettingsMode)
            {
                OnSettings();
            }
        }
    }
}
