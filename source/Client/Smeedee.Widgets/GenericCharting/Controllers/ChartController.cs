using System;
using System.Collections.Generic;
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

            this.storageReader = storageReader;
            
            
            chartViewModel.Refresh.AfterExecute += new EventHandler(OnNotifiedToRefresh);

            Start();

            //DownloadAndAddDataToViewModel();
        }

        private void DownloadAndAddDataToViewModel()
        {

        }


        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            DownloadAndAddDataToViewModel();
        }

        private Uri Url;

    }
}
