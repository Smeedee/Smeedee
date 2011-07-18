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
        public ChartSettingsViewModel settingsViewModel;

        public ChartController(
            ChartViewModel chartViewModel,
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

            

            chartViewModel.Refresh.AfterExecute += new EventHandler(OnNotifiedToRefresh);

            

            

            settingsViewModel = new ChartSettingsViewModel();
            settingsViewModel.SaveSettings.ExecuteDelegate = SaveSettings;
            settingsViewModel.ReloadSettings.ExecuteDelegate = ReloadSettings;
            settingsViewModel.AddDataSettings.ExecuteDelegate = AddDataSettings;

            Start();

            DownloadAndAddDataToViewModel();
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

        private void DownloadAndAddDataToViewModel()
        {
            SetIsLoadingConfig();
            SetIsLoadingData();

            var db = storageReader.GetDatabases();
            AddDatabasesToSettingsViewModel(db);
            
            SetIsNotLoadingConfig();
            SetIsNotLoadingData();

        }

         public void AddDatabasesToSettingsViewModel(IList<string> db)
         {
             var databases = new ObservableCollection<DatabaseViewModel>();
             var databaseNames = db;

             foreach (var databaseName in databaseNames)
             {
                 databases.Add(new DatabaseViewModel{Name = databaseName});
             }

             settingsViewModel.Databases = databases;

         }

        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            DownloadAndAddDataToViewModel();
        }



    }
}
