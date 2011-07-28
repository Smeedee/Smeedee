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
        private WebSnapshotConfig webSnapshotConfig;
        private IRepository<DomainModel.WebSnapshot.WebSnapshot> repository;
        private IInvokeBackgroundWorker<IEnumerable<DomainModel.WebSnapshot.WebSnapshot>> asyncClient;
        private ILog logger;

        public WebSnapshotController(
            WebSnapshotViewModel webSnapshotViewModel,
            WebSnapshotSettingsViewModel webSnapshotSettingsViewModel,
            Configuration configuration,
            IInvokeBackgroundWorker<IEnumerable<DomainModel.WebSnapshot.WebSnapshot>> asyncClient,
            ITimer timer,
            ILog logger,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier,
            IPersistDomainModelsAsync<Configuration> configPersister,
            IRepository<DomainModel.WebSnapshot.WebSnapshot> repository
            )
            : base(webSnapshotViewModel, timer, uiInvoker, loadingNotifier)
        {
            Guard.Requires<ArgumentNullException>(webSnapshotViewModel != null);
            Guard.Requires<ArgumentNullException>(webSnapshotSettingsViewModel != null);
            Guard.Requires<ArgumentNullException>(configuration != null);
            //Guard.Requires<ArgumentNullException>(repository != null);


            webSnapshotConfig = new WebSnapshotConfig(configuration);
            this.configPersisterRepository = configPersister;
            configPersisterRepository.SaveCompleted += OnSaveCompleted;
            this.webSnapshotViewModel = webSnapshotViewModel;
            this.logger = logger;
            this.webSnapshotSettingsViewModel = webSnapshotSettingsViewModel;
            this.repository = repository;
            this.asyncClient = asyncClient;
            webSnapshotSettingsViewModel.Save.ExecuteDelegate += OnSave;
            webSnapshotSettingsViewModel.Reset.ExecuteDelegate += OnReset;


            Start();
            LoadData();
        }

        private void OnSaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            SetIsNotSavingConfig();
            LoadData();
        }

        private void OnSave()
        {
            SetIsSavingConfig();
            CopySettingsViewModelToConfiguration();
            configPersisterRepository.Save(webSnapshotConfig.Configuration);

        }

        private void CopySettingsViewModelToConfiguration()
        {
            webSnapshotConfig.URL = webSnapshotSettingsViewModel.SelectedImage ?? "URLZ";
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
            LoadData();
        }


        private void OnReset()
        {
            return;
            uiInvoker.Invoke(() =>
            {
                //TODO Get(SelectedImage) and set it as Image
                webSnapshotSettingsViewModel.Image = GenerateWriteableBitmap("http://www.dvo.com/newsletter/monthly/2007/september/images/strawberry.jpg");
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

        protected void LoadData()
        {
            return; //TODO pga webssørvis er ustabil
            LoadData(new WebSnapshotSpecification());
        }

        protected void LoadData(Specification<DomainModel.WebSnapshot.WebSnapshot> specification)
        {
            if (!ViewModel.IsLoading)
            {
                asyncClient.RunAsyncVoid(() => LoadDataSync(specification));
            }
            ViewModel.Snapshot = webSnapshotSettingsViewModel.Image;
        }

        private void LoadDataSync(Specification<DomainModel.WebSnapshot.WebSnapshot> specification)
        {
            SetIsLoadingData();
            var snapshot = QuerySnapshot(specification);
            logger.WriteEntry(new LogEntry("Snapshot", "I got the snapshot" + snapshot.ToString()));

            uiInvoker.Invoke(() =>
            {

                try
                {
                    //logger.WriteEntry(new LogEntry("WebSnapshotController", "Tries to get snapshot from repository"));
                    //var specification = new WebSnapshotSpecification();
                    //logger.WriteEntry(new LogEntry("Specification", "I maded the specification"+specification.ToString()));
                    //var snapshot = repository.Get(new WebSnapshotSpecification());
                    //logger.WriteEntry(new LogEntry("repository", "repository is lame "+repository.GetType() + " - " + repository.ToString()));
                    //var snapshot = repository.Get(specification);

                    UpdateViewModel(snapshot);
                }
                catch (Exception e)
                {
                    LogErrorMsg(e);
                    ViewModel.HasConnectionProblems = true;
                }

            });
            SetIsNotLoadingData();
        }

        protected IEnumerable<DomainModel.WebSnapshot.WebSnapshot> QuerySnapshot(Specification<DomainModel.WebSnapshot.WebSnapshot> specification)
        {
            IEnumerable<DomainModel.WebSnapshot.WebSnapshot> snapshot = null;
            try
            {
                logger.WriteEntry(new LogEntry("QuerySnapshot", "QuerySnapshot called, im in teh try"));
                logger.WriteEntry(new LogEntry("QuerySnapshot", "the repo is lame " + repository));
                snapshot = repository.Get(specification);
                logger.WriteEntry(new LogEntry("schnappy", "I got the snapshot"));
                ViewModel.HasConnectionProblems = false;
            }
            catch (Exception e)
            {
                LogErrorMsg(e);
                ViewModel.HasConnectionProblems = true;
            }

            return snapshot;
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

        //public void UpdateConfiguration(Configuration config)
        //{
        //    Guard.Requires<ArgumentNullException>(config != null);
        //    Guard.Requires<ArgumentException>(config.ContainsSetting(url));
        //    Guard.Requires<ArgumentException>(config.ContainsSetting(refresh_interval));

        //    this.config = config;

        //    webSnapshotSettingsViewModel.InputUrl = config.GetSetting(url).Value;
        //}

        //public Configuration SaveConfiguration()
        //{
        //    config.ChangeSetting(url, webSnapshotSettingsViewModel.InputUrl);
        //    config.ChangeSetting(refresh_interval, webSnapshotSettingsViewModel.RefreshInterval.ToString());

        //    return config;
        //}

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LogErrorMsg(Exception exception)
        {
            logger.WriteEntry(ErrorLogEntry.Create(this, exception.ToString()));
        }


    }
}
