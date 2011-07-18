using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private int refresh_event_raised;

        public ChartController(
            ChartViewModel chartViewModel,
            ChartSettingsViewModel settingsViewModel,
            ITimer timer,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier,
            IChartStorageReader storageReader,
            Configuration configuration
            ) : base(chartViewModel, timer, uiInvoker, loadingNotifier)
        {

            Guard.Requires<ArgumentException>(storageReader != null);
            Guard.Requires<ArgumentException>(configuration != null);

            chartConfig = new ChartConfig(configuration);

            this.storageReader = storageReader;

            

            ViewModel.Refresh.AfterExecute += OnNotifiedToRefresh;

            refresh_event_raised = 0;
            storageReader.DatasourcesRefreshed += DatasourcesRefreshed;


            SettingsViewModel = settingsViewModel;
            SettingsViewModel.SaveSettings.ExecuteDelegate = SaveSettings;
            SettingsViewModel.ReloadSettings.ExecuteDelegate = ReloadSettings;
            SettingsViewModel.AddDataSettings.ExecuteDelegate = AddDataSettings;

            Start();

            UpdateListOfDataSources();
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
            //SetIsLoadingConfig();
         
            //SetIsNotLoadingData();
            
            SetIsLoadingData();

            storageReader.RefreshDatasources();
            
            SetIsNotLoadingData();
        }

        private void DatasourcesRefreshed(object sender, EventArgs args)
        {
            var db = storageReader.GetDatabases();

            if (DatabasesExists())
            {
                AddDatabasesToSettingsViewModel(db);
                AddCollectionsToSettingsViewModel(db[0]);
            }

            SetIsNotLoadingConfig();            
        }

        private bool DatabasesExists()
        {
            return storageReader.GetDatabases().Count > 0;
        }

        public void AddDatabasesToSettingsViewModel(IList<string> db)
         {
             foreach (var databaseName in db)
             {
                 SettingsViewModel.Databases.Add(databaseName);
             }
         }

        public void AddCollectionsToSettingsViewModel(string database)
        {
/*            databaseViewModel = new DatabaseViewModel();
            var collectionViewModels = new ObservableCollection<CollectionViewModel>();

            var collectionsInDatabase = storageReader.GetCollectionsInDatabase(database);
            int i = collectionsInDatabase.Count();

            foreach (var collection in collectionsInDatabase)
            {
                collectionViewModels.Add(new CollectionViewModel { Name = collection });
            }
            databaseViewModel.Collections = collectionViewModels;
            int j = databaseViewModel.Collections.Count();*/
        }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            UpdateListOfDataSources();
        }



    }
}
