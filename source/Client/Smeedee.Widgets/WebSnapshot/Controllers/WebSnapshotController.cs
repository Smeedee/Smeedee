﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private WebSnapshotSettingsViewModel SettingsViewModel;
        private readonly IPersistDomainModelsAsync<Configuration> configPersisterRepository;
        private IAsyncRepository<DomainModel.WebSnapshot.WebSnapshot> repository;
        private WebSnapshotConfig webSnapshotConfig;
        private ILog logger;
        private string previousTimestamp;

        public WebSnapshotController(
            WebSnapshotViewModel webSnapshotViewModel,
            WebSnapshotSettingsViewModel settingsViewModel,
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
            Guard.Requires<ArgumentNullException>(settingsViewModel != null);
            Guard.Requires<ArgumentNullException>(configuration != null);
            Guard.Requires<ArgumentNullException>(logger != null);

            webSnapshotConfig = new WebSnapshotConfig(configuration);
            this.configPersisterRepository = configPersister;
            configPersisterRepository.SaveCompleted += OnSaveCompleted;
            this.webSnapshotViewModel = webSnapshotViewModel;
            this.logger = logger;
            this.repository = repository;

            SettingsViewModel = settingsViewModel;
            SettingsViewModel.Save.ExecuteDelegate += OnSaveSettings;
            SettingsViewModel.ReloadSettings.ExecuteDelegate += OnReloadSettings;
            SettingsViewModel.ReloadSettings.AfterExecute += OnReloadSettingsCompleted;

            repository.GetCompleted += OnGetCompleted;

            Start();
            BeginLoadData();
        }

        private void OnReloadSettingsCompleted(object sender, EventArgs e)
        {
            //SetIsNotLoadingConfig();
            BeginLoadData();
        }

        public void OnReloadSettings()
        {
            //SetIsLoadingConfig();
            CopyConfigurationToSettingsViewModel();
        }

        private void OnSaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            SetIsNotSavingConfig();
            SettingsViewModel.IsTimeToUpdate = false;
            SettingsViewModel.IsTimeToUpdate = true;

            BeginLoadData();
        }

        private void OnSaveSettings()
        {
            SetIsSavingConfig();
            CopySettingsViewModelToConfiguration();
            configPersisterRepository.Save(webSnapshotConfig.Configuration);

        }

        private void CopySettingsViewModelToConfiguration()
        {
            webSnapshotConfig.TaskName = SettingsViewModel.SelectedImage;
            webSnapshotConfig.CoordinateX = SettingsViewModel.CropCoordinateX;
            webSnapshotConfig.CoordinateY = SettingsViewModel.CropCoordinateY;
            webSnapshotConfig.RectangleHeight = SettingsViewModel.CropRectangleHeight;
            webSnapshotConfig.RectangleWidth = SettingsViewModel.CropRectangleWidth;
            webSnapshotConfig.Timestamp = "0";
            webSnapshotConfig.IsConfigured = true;
        }

        private void CopyConfigurationToSettingsViewModel()
        {
            uiInvoker.Invoke(() =>
                                 {
                                     SettingsViewModel.SelectedImage = webSnapshotConfig.TaskName;
                                     SettingsViewModel.CropCoordinateX = webSnapshotConfig.CoordinateX;
                                     SettingsViewModel.CropCoordinateY = webSnapshotConfig.CoordinateY;
                                     SettingsViewModel.CropRectangleHeight = webSnapshotConfig.RectangleHeight;
                                     SettingsViewModel.CropRectangleWidth = webSnapshotConfig.RectangleWidth;
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
                UpdateViewModel(SnapshotDataFromDB);
                PopulateAvailableImages(SnapshotDataFromDB);
            }
            else
            {
                logger.WriteEntry(new LogEntry("WebSnapshotController", string.Format("EventAargs as null. {0}", eventArgs.Error.ToString())));
            }

            SetIsNotLoadingData();
        }

        private void UpdateViewModel(IEnumerable<DomainModel.WebSnapshot.WebSnapshot> snapshots)
        {
            if (snapshots.Count() > 0)
            {
                var snapshot = snapshots.First();
                uiInvoker.Invoke(() => SetWebSnapshot(snapshot));
            }

        }

        private string previousTimestamp;
        private void SetWebSnapshot(DomainModel.WebSnapshot.WebSnapshot snapshot)
        {
            var timestamp = snapshot.Timestamp;

            if (previousTimestamp == timestamp)
            {
                SettingsViewModel.IsTimeToUpdate = false;
            }
            else
            {
                webSnapshotConfig.Timestamp = timestamp;
                SettingsViewModel.IsTimeToUpdate = true;
            }
            previousTimestamp = timestamp;
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            BeginLoadData();
        }

        protected void LogErrorMsg(Exception exception)
        {
            logger.WriteEntry(ErrorLogEntry.Create(this, exception.ToString()));
        }


        private void PopulateAvailableImages(IEnumerable<DomainModel.WebSnapshot.WebSnapshot> snapshotDataFromDb)
        {
            uiInvoker.Invoke(() =>
                                 {
                                     var selected = SettingsViewModel.SelectedImage;
                                     SettingsViewModel.AvailableImages.Clear();
                                     SettingsViewModel.AvailableImagesUri.Clear();

                                     foreach (var snapshot in snapshotDataFromDb)
                                     {
                                         var fileName = Path.GetFileName(snapshot.PictureFilePath);
                                         SettingsViewModel.AvailableImagesUri.Add("WebSnapshots/" + fileName);
                                         SettingsViewModel.AvailableImages.Add(snapshot.Name);
                                     }
                                     SettingsViewModel.SelectedImage = selected;
                                 });
        }

        public void ShowImageInSettingsView()
        {
            uiInvoker.Invoke(() => SettingsViewModel.Image = SettingsViewModel.LoadedImage);
        }
    }
}
