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
        private ChartSettingsViewModel settingsViewModel;

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

            Start();

            DownloadAndAddDataToViewModel();

            settingsViewModel = new ChartSettingsViewModel();
            settingsViewModel.SaveSettings.ExecuteDelegate = SaveSettings;
            settingsViewModel.ReloadSettings.ExecuteDelegate = ReloadSettings;
            settingsViewModel.AddDataSettings.ExecuteDelegate = AddDataSettings;
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

            var databases = new ObservableCollection<DatabaseViewModel>();
            databases.Add(new DatabaseViewModel{});
            
            
            SetIsNotLoadingConfig();
            SetIsNotLoadingData();

        }


        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            DownloadAndAddDataToViewModel();
        }

        private Uri Url;

    }
}
