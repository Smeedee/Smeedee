using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.DomainModel.WebSnapshot;
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
            IPersistDomainModelsAsync<Configuration> configPersister/*,
            IAsyncRepository<DomainModel.WebSnapshot.WebSnapshot> repository*/
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
            webSnapshotSettingsViewModel.Save.ExecuteDelegate += OnSave;
            webSnapshotSettingsViewModel.Reset.ExecuteDelegate += OnReset;

           // repository.GetCompleted += OnGetCompleted;



            Start();
      //      BeginLoadData();

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


        private void OnReset()
        {
            uiInvoker.Invoke(() =>
            {
                //TODO Get(SelectedImage) and set it as Image
                //webSnapshotSettingsViewModel.Image = GenerateWriteableBitmap("http://www.dvo.com/newsletter/monthly/2007/september/images/strawberry.jpg");
            });
        }

        private WriteableBitmap GenerateWriteableBitmap(string path)
        {
            var uri = new Uri(path);
            var bmi = new BitmapImage(uri);
            var wb = new WriteableBitmap(bmi);

            uri = null;
            bmi = null;

            return wb;
        }

        protected void BeginLoadData()
        {
            if (!ViewModel.IsLoading)
            {
                SetIsLoadingData();
                repository.BeginGet(new WebSnapshotSpecification());
            }
        }


        private void OnGetCompleted(object sender, GetCompletedEventArgs<DomainModel.WebSnapshot.WebSnapshot> eventArgs)
        {
            if (eventArgs.Result != null)
            {
                var SnapshotDataFromDB = eventArgs.Result;
                //logger.WriteEntry(new LogEntry("SnapshotDataFromDB", "Did I get it? " + SnapshotDataFromDB.First().PictureFilePath));
                //UpdateViewModel(SnapshotDataFromDB);
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
                });
            }
            else
            {
                uiInvoker.Invoke(() => webSnapshotViewModel.HasStoredImage = false);
            }
        }

        private void SetWebSnapshot(string imagePath)
        {
            webSnapshotViewModel.Snapshot = GenerateWriteableBitmap(imagePath);
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
