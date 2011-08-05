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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.SL;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Widgets.SL.WebSnapshot.Views;
using Smeedee.Widgets.WebSnapshot.Controllers;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.SL.WebSnapshot
{
    [WidgetInfo(
        Name = "Web Snapshot",
        Description = "Display snapshots taken by the \"Web Snapshot\" task",
        Author = "Smeedee team",
        Version = "0.2",
        Tags = new[] { CommonTags.Fun })]
    public class WebSnapshotWidget : Client.Framework.ViewModel.Widget
    {
        private WebSnapshotViewModel viewModel;
        private WebSnapshotController controller;
        private WebSnapshotSettingsViewModel settingsViewModel;
        private WebSnapshotView snapshotView;
        private WebSnapshotSettingsView settingsView;

        public WebSnapshotWidget()
        {
            Title = "Web Snapshot";
            viewModel = GetInstance<WebSnapshotViewModel>();
            settingsViewModel = GetInstance<WebSnapshotSettingsViewModel>();
            controller = NewController<WebSnapshotController>();

            viewModel.PropertyChanged += ViewModelPropertyChanged;
            
            settingsViewModel.PropertyChanged += WebSnapshot_Propertychanged;

            snapshotView = new WebSnapshotView { DataContext = settingsViewModel };
            View = snapshotView;

            settingsView= new WebSnapshotSettingsView { DataContext = settingsViewModel };
            SettingsView = settingsView;

            ConfigurationChanged += (o, e) =>
                                        {
                                            if (!IsInSettingsMode)
                                                controller.UpdateConfiguration(Configuration);
                                        };
        }

        private void WebSnapshot_Propertychanged(object sender, PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "IsTimeToUpdate")
            {
                snapshotView.UpdateImage();
            }
            else if (e.PropertyName == "SelectedImage")
            {
                LoadImageFunction();
            }
            else if (e.PropertyName == "LoadedImage")
            {
                controller.ShowImageInSettingsView();
                settingsView.LoadedImageCB = settingsViewModel.LoadedImage as WriteableBitmap;
            }
        }

        private void ViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var tempViewModel = sender as WebSnapshotViewModel;
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

        protected override Configuration NewConfiguration()
        {
            return WebSnapshotConfig.NewDefaultConfiguration();
        }


        BitmapImage _imageFromServer;
        void LoadImageFunction()
        {
            _imageFromServer = new BitmapImage();
            _imageFromServer.ImageOpened += bi_ImageOpened;
            _imageFromServer.ImageFailed += bi_ImageFailed;
            _imageFromServer.CreateOptions = BitmapCreateOptions.None;
            _imageFromServer.UriSource = new Uri(App.Current.Host.Source, "../" + settingsViewModel.UriOfSelectedImage);
        }

        void bi_ImageFailed(object sender, ExceptionRoutedEventArgs e) { }

        void bi_ImageOpened(object sender, RoutedEventArgs e)
        {
            var wb = new WriteableBitmap(_imageFromServer);
            settingsViewModel.LoadedImage = wb;
        }
    }
}
