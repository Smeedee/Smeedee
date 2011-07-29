using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Framework;
using Smeedee.Widgets.WebSnapshot.ViewModel;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.WebSnapshot.Controllers
{
    public class WebSnapshotController : ControllerBase<WebSnapshotViewModel>
    {
        private WebSnapshotViewModel webSnapshotViewModel;
        private WebSnapshotSettingsViewModel webSnapshotSettingsViewModel;
        private readonly IPersistDomainModelsAsync<Configuration> configPersisterRepository;
        private IAsyncRepository<DomainModel.WebSnapshot.WebSnapshot> repository;
        private WebSnapshotConfig webSnapshotConfig;
        private ILog logger;

        public WebSnapshotController(
            WebSnapshotViewModel webSnapshotViewModel,
            WebSnapshotSettingsViewModel webSnapshotSettingsViewModel,
            Configuration configuration,
            ITimer timer,
            ILog logger,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier,
            IPersistDomainModelsAsync<Configuration> configPersister,
            IAsyncRepository<DomainModel.WebSnapshot.WebSnapshot> repository
            )
            : base(webSnapshotViewModel, timer, uiInvoker, loadingNotifier)
        {
            Guard.Requires<ArgumentNullException>(webSnapshotViewModel != null);
            Guard.Requires<ArgumentNullException>(webSnapshotSettingsViewModel != null);
            Guard.Requires<ArgumentNullException>(configuration != null);
            Guard.Requires<ArgumentNullException>(logger != null);

            webSnapshotConfig = new WebSnapshotConfig(configuration);
            this.configPersisterRepository = configPersister;
            configPersisterRepository.SaveCompleted += OnSaveCompleted;
            this.webSnapshotViewModel = webSnapshotViewModel;
            this.logger = logger;
            this.webSnapshotSettingsViewModel = webSnapshotSettingsViewModel;
            this.repository = repository;
            webSnapshotSettingsViewModel.PropertyChanged += webSnapshotSettingsViewModel_PropertyChanged;
            webSnapshotSettingsViewModel.Save.ExecuteDelegate += OnSave;
            webSnapshotSettingsViewModel.ReloadSettings.ExecuteDelegate += OnReLoadSettings;
            webSnapshotSettingsViewModel.ReloadSettings.AfterExecute += OnReloadSettingsCompleted;

            repository.GetCompleted += OnGetCompleted;

            Start();
            BeginLoadData();
        }

        private void webSnapshotSettingsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedImage")
            {
                uiInvoker.Invoke(() => webSnapshotSettingsViewModel.Image = new BitmapImage(new Uri(webSnapshotSettingsViewModel.SelectedImage)));
            }
        }

        private void OnReloadSettingsCompleted(object sender, EventArgs e)
        {
            SetIsNotLoadingConfig();
            BeginLoadData();
        }

        private void OnReLoadSettings()
        {
            SetIsLoadingConfig();
            CopyConfigurationToSettingsViewModel();
        }

        private void OnSaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            SetIsNotSavingConfig();
            BeginLoadData();
        }

        private void OnSave()
        {
            SetIsSavingConfig();
            CopySettingsViewModelToConfiguration();

            uiInvoker.Invoke(() => webSnapshotViewModel.Snapshot = webSnapshotSettingsViewModel.Image);

            configPersisterRepository.Save(webSnapshotConfig.Configuration);

        }

        private void CopySettingsViewModelToConfiguration()
        {
            webSnapshotConfig.URL = webSnapshotSettingsViewModel.SelectedImage;
            webSnapshotConfig.CoordinateX = webSnapshotSettingsViewModel.CropCoordinateX;
            webSnapshotConfig.CoordinateY = webSnapshotSettingsViewModel.CropCoordinateY;
            webSnapshotConfig.RectangleHeight = webSnapshotSettingsViewModel.CropRectangleHeight;
            webSnapshotConfig.RectangleWidth = webSnapshotSettingsViewModel.CropRectangleWidth;
            webSnapshotConfig.IsConfigured = true;
        }

        private void CopyConfigurationToSettingsViewModel()
        {
            uiInvoker.Invoke(() =>
            {
                webSnapshotSettingsViewModel.SelectedImage = webSnapshotConfig.URL;
                webSnapshotSettingsViewModel.CropCoordinateX = webSnapshotConfig.CoordinateX;
                webSnapshotSettingsViewModel.CropCoordinateY = webSnapshotConfig.CoordinateY;
                webSnapshotSettingsViewModel.CropRectangleHeight = webSnapshotConfig.RectangleHeight;
                webSnapshotSettingsViewModel.CropRectangleWidth = webSnapshotConfig.RectangleWidth;
            });
        }

        public void UpdateConfiguration(Configuration configuration)
        {
            webSnapshotConfig = new WebSnapshotConfig(configuration);
            CopyConfigurationToSettingsViewModel();
            BeginLoadData();
        }

        protected void BeginLoadData()
        {
            //if (!ViewModel.IsLoading)
            //{
            //    SetIsLoadingData();
            //    repository.BeginGet(new WebSnapshotSpecification());
            //}
        }


        private void OnGetCompleted(object sender, GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot> eventArgs)
        {
            if (eventArgs.Result != null)
            {
                var SnapshotDataFromDB = eventArgs.Result;
                logger.WriteEntry(new LogEntry("SnapshotDataFromDB", "Did I get it? " + SnapshotDataFromDB.First().PictureFilePath));
                UpdateViewModel(SnapshotDataFromDB);
            }
            else
            {
                logger.WriteEntry(new LogEntry("OnGetCompleted,eventArgs was null", eventArgs.Error.ToString()));
            }

            SetIsNotLoadingData();
        }

        private void UpdateViewModel(IEnumerable<DomainModel.WebSnapshot.WebSnapshot> snapshots)
        {
            logger.WriteEntry(new LogEntry("UpdateViewModel", "UpdateViewModel called"));
            if (snapshots.Count() > 0)
            {
                var snapshot = snapshots.First();
                uiInvoker.Invoke(() =>
                {
                    var snapshotPath = snapshot.PictureFilePath;
                    SetWebSnapshot(snapshotPath);
                    //SetWebSnapshot(snapshot);
                });
            }
            else
            {
                uiInvoker.Invoke(() => webSnapshotViewModel.HasStoredImage = false);
            }
        }

        private void SetWebSnapshot(DomainModel.WebSnapshot.WebSnapshot snapshot)
        {
            //var timestamp = snapshot.Timestamp;
            
            //if (previousTimestamp == timestamp) return;

            //webSnapshotViewModel.Snapshot = CropImage(snapshot);
            //previousTimestamp = timestamp;
        }

        private BitmapImage CropImage(DomainModel.WebSnapshot.WebSnapshot snapshot)
        {
             //= webSnapshotConfig.URL;
             //= webSnapshotConfig.CoordinateX;
             //= webSnapshotConfig.CoordinateY;
             //= webSnapshotConfig.RectangleHeight;
             //= webSnapshotConfig.RectangleWidth;


        }

        private void SetWebSnapshot(string imagePath)
        {
            webSnapshotViewModel.Snapshot = new BitmapImage(new Uri(imagePath));
            webSnapshotViewModel.HasStoredImage = imagePath != null;
        }



        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            BeginLoadData();
        }

        protected void LogErrorMsg(Exception exception)
        {
            logger.WriteEntry(ErrorLogEntry.Create(this, exception.ToString()));
        }


    }
}
