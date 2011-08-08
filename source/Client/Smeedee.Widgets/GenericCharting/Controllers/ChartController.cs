using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Repositories.Charting;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.Framework.Logging;
using Smeedee.Framework;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.GenericCharting.Controllers
{
    public class ChartController : ControllerBase<ChartViewModel>
    {

        public ChartSettingsViewModel SettingsViewModel { get; private set; }
        public ChartConfig ChartConfig { get; private set; }

        private readonly IPersistDomainModelsAsync<Configuration> configPersister;

        private IChartStorageReader storageReader;
        private ILog logger;        
        private ChartUpdater chartUpdater;

        public ChartController(
            ChartViewModel chartViewModel,
            ChartSettingsViewModel settingsViewModel,
            ITimer timer,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier,
            IChartStorageReader storageReader,
            Configuration configuration,
            IPersistDomainModelsAsync<Configuration> configPersister,
            ILog logger,
            IWidget widget
            )
            : base(chartViewModel, timer, uiInvoker, loadingNotifier, widget, configPersister)
        {
            Guard.Requires<ArgumentNullException>(storageReader != null);
            Guard.Requires<ArgumentNullException>(configPersister != null);
            Guard.Requires<ArgumentNullException>(configuration != null);
            Guard.Requires<ArgumentNullException>(logger != null);

            this.logger = logger;

            ChartConfig = new ChartConfig(configuration);

            chartUpdater = new ChartUpdater(ViewModel, storageReader, uiInvoker) {ChartConfig = ChartConfig};
            chartUpdater.UpdateFinished += OnUpdateFinished;
            
            this.configPersister = configPersister;
            //this.configPersister.SaveCompleted += OnSaveCompleted;
                        
            ViewModel.Refresh.AfterExecute += OnNotifiedToRefresh;

            this.storageReader = storageReader;
            this.storageReader.DatasourcesRefreshed += DatasourcesRefreshed;
            this.storageReader.Error += OnError;

            SettingsViewModel = settingsViewModel;
            SettingsViewModel.SaveSettings.ExecuteDelegate = OnSaveSettings;
            SettingsViewModel.ReloadSettings.ExecuteDelegate = OnReloadSettings;
            SettingsViewModel.AddDataSettings.ExecuteDelegate = OnAddDataSettings;

            SettingsViewModel.PropertyChanged += OnSettingsViewModelPropertyChanged;

            UpdateListOfDataSources();
            Start();
            LoadData();
        }
        
        public void LoadData()
        {
            SetIsLoadingData();
            chartUpdater.Update();
        }
        
        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            LoadData();
        }

        private void OnUpdateFinished(object sender, EventArgs e)
        {
            SetIsNotLoadingData();
        }

        private void OnSaveSettings()
        {
            CopySettingsViewModelToConfiguration();
            SaveConfiguration();
        }

        private void CopySettingsViewModelToConfiguration()
        {
            ChartConfig.ChartName = SettingsViewModel.ChartName;
            ChartConfig.XAxisName = SettingsViewModel.XAxisName;
            ChartConfig.YAxisName = SettingsViewModel.YAxisName;
            ChartConfig.XAxisType = SettingsViewModel.XAxisType;
            ChartConfig.SetSeries(SettingsViewModel.SeriesConfig);
            ChartConfig.IsConfigured = true;
        }

        public void OnReloadSettings()
        {
            UpdateListOfDataSources();
            CopyConfigurationToSettingsViewModel();
        }

        private void UpdateListOfDataSources()
        {
            storageReader.RefreshDatasources();
        }

        private void DatasourcesRefreshed(object sender, EventArgs args)
        {
            AddDatabasesToSettingsViewModel(storageReader.GetDatabases());
        }

        private void AddDatabasesToSettingsViewModel(IEnumerable<string> db)
        {
            uiInvoker.Invoke(() =>
            {
                SettingsViewModel.Databases.Clear();
                SettingsViewModel.SelectedDatabase = null;
                SettingsViewModel.Collections.Clear();
                foreach (var databaseName in db)
                {
                    SettingsViewModel.Databases.Add(databaseName);
                }
            });
        }

        private void CopyConfigurationToSettingsViewModel()
        {
            uiInvoker.Invoke(() =>
            {
                SettingsViewModel.ChartName = ChartConfig.ChartName;
                SettingsViewModel.XAxisName = ChartConfig.XAxisName;
                SettingsViewModel.YAxisName = ChartConfig.YAxisName;
                SettingsViewModel.XAxisType = ChartConfig.XAxisType;
                SettingsViewModel.SeriesConfig.Clear();
                ChartConfig.GetSeries().ForEach(AddSeriesToSettingsView);
            });
        }

        public void AddSeriesToSettingsView(SeriesConfigViewModel series)
        {
            AddSeriesPropertyChangedEventListener(series);
            SettingsViewModel.SeriesConfig.Add(series);
        }

        private void AddSeriesPropertyChangedEventListener(SeriesConfigViewModel series)
        {
            series.PropertyChanged += OnSeriesPropertyChanged;
        }

        private void OnSeriesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var series = sender as SeriesConfigViewModel;
            if (series.Action == ChartConfig.REMOVE)
            {
                uiInvoker.Invoke(() => SettingsViewModel.SeriesConfig.Remove(series));
            }
        }

        private void OnAddDataSettings()
        {        
            var database = SettingsViewModel.SelectedDatabase;
            var collection = SettingsViewModel.SelectedCollection;
            if (database != null && collection != null)
                storageReader.LoadChart(database, collection, AddDataChartLoaded);
        }

        private void AddDataChartLoaded(Chart chart)
        {
            uiInvoker.Invoke(() =>
            {
                foreach (var dataset in chart.DataSets)
                {
                    var series = new SeriesConfigViewModel { Database = chart.Database, Collection = chart.Collection, Name = dataset.Name };
                    AddSeriesToSettingsView(series);
                }
            });
        }

        public void OnSettingsViewModelPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == "SelectedDatabase")
                UpdateCollectionsInSettingsViewModel(SettingsViewModel.SelectedDatabase);
        }

        public void UpdateCollectionsInSettingsViewModel(string database)
        {
            SettingsViewModel.Collections.Clear();
            SettingsViewModel.SelectedCollection = null;

            var collections = storageReader.GetCollectionsInDatabase(database);
            if (collections != null && collections.Count() > 0)
            {
                uiInvoker.Invoke(() =>
                {
                    foreach (var collectionName in collections)
                    {
                        SettingsViewModel.Collections.Add(collectionName);
                    }
                });
            }
        }

        protected override void OnConfigurationChanged(Configuration configuration)
        {
            UpdateConfiguration(configuration);
        }

        public void UpdateConfiguration(Configuration configuration)
        {
            ChartConfig = new ChartConfig(configuration);
            chartUpdater.ChartConfig = ChartConfig;
            CopyConfigurationToSettingsViewModel();
            LoadData();
        }

        private void OnError(object sender, ChartErrorEventArgs e)
        {
            ViewModel.ErrorMessage = e.Message;
            ViewModel.ShowErrorMessageInsteadOfChart = true;
            SetIsNotLoadingConfig();
            SetIsNotLoadingData();

            var entry = ErrorLogEntry.Create(this, e.Exception.Message);
            logger.WriteEntry(entry);
        }
    }
}
