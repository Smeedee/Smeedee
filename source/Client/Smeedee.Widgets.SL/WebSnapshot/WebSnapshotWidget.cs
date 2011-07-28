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
using Smeedee.Widgets.WebSnapshot.Controllers;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.SL.WebSnapshot
{
    [WidgetInfo(
        Name = "Web Snapshot",
        Description = "Takes snapshots of web pages or display a picture from an URL. Supports XPath and cropping.",
        Author = "Smeedee team",
        Version = "0.2",
        Tags = new[] { CommonTags.Fun })]
    public class WebSnapshotWidget : Client.Framework.ViewModel.Widget
    {
        private WebSnapshotViewModel viewModel;
        private WebSnapshotController controller;
        private WebSnapshotSettingsViewModel settingsViewModel;

        public WebSnapshotWidget()
        {
            Title = "Web Snapshot";
            viewModel = GetInstance<WebSnapshotViewModel>();
            settingsViewModel = GetInstance<WebSnapshotSettingsViewModel>();
            controller = NewController<WebSnapshotController>();
            viewModel.PropertyChanged += ViewModelPropertyChanged;

            View = new WebSnapshotView { DataContext = controller.ViewModel };
            SettingsView = new WebSnapshotSettingsView { DataContext = settingsViewModel };
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

        public override void Configure(DependencyConfigSemantics config)
        {
            config.Bind<WebSnapshotViewModel>().To<WebSnapshotViewModel>().InSingletonScope();
            config.Bind<WebSnapshotSettingsViewModel>().To<WebSnapshotSettingsViewModel>().InSingletonScope();
        }
    }
}
