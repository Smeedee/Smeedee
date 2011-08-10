using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Client.SL;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
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
        Version = "1.0",
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

            PropertyChanged += (o,e) =>
                                   {
                                       if (e.PropertyName == "IsInSettingsMode")
                                           controller.OnReloadSettings();
                                   } ;

            PropertyChanged += controller.ToggleRefreshInSettingsMode;

            settingsViewModel.PropertyChanged += WebSnapshot_Propertychanged;

            snapshotView = new WebSnapshotView { DataContext = settingsViewModel };
            View = snapshotView;

            settingsView = new WebSnapshotSettingsView { DataContext = settingsViewModel };
            SettingsView = settingsView;

            ConfigurationChanged += (o, e) =>
                                        {
                                            if (!IsInSettingsMode)
                                                controller.UpdateConfiguration(Configuration);
                                        };
        }

        
        private void WebSnapshot_Propertychanged(object sender, PropertyChangedEventArgs e)
        {

            switch (e.PropertyName)
            {
                case "SelectedImage":
                case "IsTimeToUpdate":
                    LoadImageFunction();
                    break;
                case "LoadedImage":
                    controller.ShowImageInSettingsView();
                    settingsView.LoadedImageCB = settingsViewModel.LoadedImage as WriteableBitmap;
                    snapshotView.UpdateImage();
                    break;
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
            _imageFromServer.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            _imageFromServer.UriSource = new Uri(App.Current.Host.Source, "../" + settingsViewModel.UriOfSelectedImage);
        }

        void bi_ImageFailed(object sender, ExceptionRoutedEventArgs e) { }

        void bi_ImageOpened(object sender, RoutedEventArgs e)
        {
            var bm = (BitmapImage)sender;
            var wb = new WriteableBitmap(bm);
            settingsViewModel.LoadedImage = wb;
        }
    }
}
