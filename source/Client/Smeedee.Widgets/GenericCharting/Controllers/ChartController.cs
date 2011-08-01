using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Repositories.Charting;
using Smeedee.Client.Framework.Services;
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
        private const string SettingsEntryName = "GenericCharting";

        private IChartStorageReader storageReader;
        private ChartConfig chartConfig;
        private ILog logger;
        public ChartSettingsViewModel SettingsViewModel { get; private set; }
        private readonly IPersistDomainModelsAsync<Configuration> configPersister;

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
            Client.Framework.ViewModel.Widget widget
            )
            : base(chartViewModel, timer, uiInvoker, loadingNotifier, widget)
        {

            Guard.Requires<ArgumentNullException>(storageReader != null);
            Guard.Requires<ArgumentNullException>(configPersister != null);
            Guard.Requires<ArgumentNullException>(configuration != null);
            Guard.Requires<ArgumentNullException>(logger != null);

            this.logger = logger;

            chartConfig = new ChartConfig(configuration);

            chartUpdater = new ChartUpdater(ViewModel, storageReader, uiInvoker) {ChartConfig = chartConfig};
            chartUpdater.UpdateFinished += OnUpdateFinished;
            this.configPersister = configPersister;
            this.configPersister.SaveCompleted += OnSaveCompleted;
            
            this.storageReader = storageReader;

            ViewModel.Refresh.AfterExecute += OnNotifiedToRefresh;

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

        public ChartConfig ChartConfig { get { return chartConfig; } }

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
       
        public void AddDatabasesToSettingsViewModel(IList<string> db)
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

        private void OnSaveSettings()
        {
            SetIsSavingConfig();
            CopySettingsViewModelToConfiguration();
            configPersister.Save(chartConfig.Configuration);
        }

        private void CopySettingsViewModelToConfiguration()
        {
            chartConfig.ChartName = SettingsViewModel.ChartName;
            chartConfig.XAxisName = SettingsViewModel.XAxisName;
            chartConfig.YAxisName = SettingsViewModel.YAxisName;
            chartConfig.XAxisType = SettingsViewModel.XAxisType;
            chartConfig.SetSeries(SettingsViewModel.SeriesConfig);
            chartConfig.IsConfigured = true;
        }

        private void OnSaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            // TODO: some error handling
            SetIsNotSavingConfig();
            LoadData();
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

        private void CopyConfigurationToSettingsViewModel()
        {
            uiInvoker.Invoke(() =>
            {
                SettingsViewModel.ChartName = chartConfig.ChartName;
                SettingsViewModel.XAxisName = chartConfig.XAxisName;
                SettingsViewModel.YAxisName = chartConfig.YAxisName;
                SettingsViewModel.XAxisType = chartConfig.XAxisType;
                SettingsViewModel.SeriesConfig.Clear();
                chartConfig.GetSeries().ForEach(AddSeriesToSettingsView);
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
            chartConfig = new ChartConfig(configuration);
            chartUpdater.ChartConfig = chartConfig;
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
