using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Repositories.Charting;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.Framework;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.GenericCharting.Controllers
{
    public class ChartController : ControllerBase<ChartViewModel>
    {
        private IDownloadStringService downloadStringService;
        private IChartStorageReader storageReader;
        private ChartConfig chartConfig;
        public ChartSettingsViewModel SettingsViewModel { get; private set; }
        private IPersistDomainModelsAsync<Configuration> configPersister;

        public ChartController(
            ChartViewModel chartViewModel,
            ChartSettingsViewModel settingsViewModel,
            ITimer timer,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier,
            IChartStorageReader storageReader,
            Configuration configuration,
            IPersistDomainModelsAsync<Configuration> configPersister
            )
            : base(chartViewModel, timer, uiInvoker, loadingNotifier)
        {

            Guard.Requires<ArgumentNullException>(storageReader != null);
            Guard.Requires<ArgumentNullException>(configuration != null);
            Guard.Requires<ArgumentNullException>(configPersister != null);

            chartConfig = new ChartConfig(configuration);

            //Guard.Requires<ArgumentException>(chartConfig.IsValid, chartConfig.ErrorMsg);

            this.configPersister = configPersister;
            this.configPersister.SaveCompleted += OnSaveCompleted;

            this.storageReader = storageReader;

            ViewModel.Refresh.AfterExecute += OnNotifiedToRefresh;

            this.storageReader.DatasourcesRefreshed += DatasourcesRefreshed;
            this.storageReader.ChartLoaded += ChartLoaded;

            SettingsViewModel = settingsViewModel;
            SettingsViewModel.SaveSettings.ExecuteDelegate = OnSaveSettings;
            SettingsViewModel.ReloadSettings.ExecuteDelegate = OnReloadSettings;
            SettingsViewModel.AddDataSettings.ExecuteDelegate = OnAddDataSettings;

            SettingsViewModel.PropertyChanged += OnSettingsViewModelPropertyChanged;



            UpdateListOfDataSources();

            Start();
        }

        

        private void ChartLoaded(object sender, ChartLoadedEventArgs e)
        {
            uiInvoker.Invoke(() =>
            {
                var databaseAndCollection = e.Chart.Database + "/" + e.Chart.Collection;
                foreach (var dataset in e.Chart.DataSets)
                    SettingsViewModel.SeriesConfig.Add(new SeriesConfigViewModel 
                    { Database = e.Chart.Database, Collection = e.Chart.Collection, DatabaseAndCollection = databaseAndCollection, Name = dataset.Name});
            });
        }

        private void OnAddDataSettings()
        {
            
            var database = SettingsViewModel.SelectedDatabase;
            var collection = SettingsViewModel.SelectedCollection;
            if (database != null && collection != null)
                storageReader.LoadChart(database, collection);
        }

        private void OnReloadSettings()
        {
            UpdateListOfDataSources();
        }

        private void OnSaveSettings()
        {
            SetIsSavingConfig();
            CopySettingsViewModelToConfiguration();
            configPersister.Save(chartConfig.Configuration);
        }

        private void CopySettingsViewModelToConfiguration()
        {
            chartConfig.SetSeries(SettingsViewModel.SeriesConfig);
        }

        private void OnSaveCompleted(object sender, SaveCompletedEventArgs e)
        {
            // TODO: some error handling
            SetIsNotSavingConfig();
        }

        private void UpdateListOfDataSources()
        {
            SetIsLoadingConfig();

            storageReader.RefreshDatasources();
        }

        private void DatasourcesRefreshed(object sender, EventArgs args)
        {
            AddDatabasesToSettingsViewModel(storageReader.GetDatabases());
            SetIsNotLoadingConfig();
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

        private void OnSettingsViewModelPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == "SelectedDatabase")
                UpdateCollectionsInSettingsViewModel(SettingsViewModel.SelectedDatabase);
        }

        public void UpdateCollectionsInSettingsViewModel(string database)
        {
            SettingsViewModel.Collections.Clear();
            SettingsViewModel.SelectedCollection = null;
            if (database == null) return;

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

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            
        }
    }
}
