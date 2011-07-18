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

        public ChartController(
            ChartViewModel chartViewModel,
            ChartSettingsViewModel settingsViewModel,
            ITimer timer,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier,
            IChartStorageReader storageReader,
            Configuration configuration
            )
            : base(chartViewModel, timer, uiInvoker, loadingNotifier)
        {

            Guard.Requires<ArgumentException>(storageReader != null);
            Guard.Requires<ArgumentException>(configuration != null);

            chartConfig = new ChartConfig(configuration);

            this.storageReader = storageReader;



            ViewModel.Refresh.AfterExecute += OnNotifiedToRefresh;



            storageReader.DatasourcesRefreshed += DatasourcesRefreshed;


            SettingsViewModel = settingsViewModel;
            SettingsViewModel.SaveSettings.ExecuteDelegate = SaveSettings;
            SettingsViewModel.ReloadSettings.ExecuteDelegate = ReloadSettings;
            SettingsViewModel.AddDataSettings.ExecuteDelegate = AddDataSettings;


            SettingsViewModel.PropertyChanged += SettingsViewModelPropertyChanged;

           

            UpdateListOfDataSources();

            Start();
        }

        private void AddDataSettings()
        {

        }

        private void ReloadSettings()
        {

        }

        private void SaveSettings()
        {
            SetIsSavingConfig();
            SetIsNotSavingConfig();
        }

        private void UpdateListOfDataSources()
        {
            SetIsLoadingConfig();

            //SetIsNotLoadingConfig();

            //SetIsLoadingData();

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

        private void SettingsViewModelPropertyChanged(object sender, PropertyChangedEventArgs eventArgs)
        {
            if (eventArgs.PropertyName == "SelectedDatabase")
            {
                UpdateCollectionsInSettingsViewModel(SettingsViewModel.SelectedDatabase);
            }
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
            UpdateListOfDataSources();
        }



    }
}
