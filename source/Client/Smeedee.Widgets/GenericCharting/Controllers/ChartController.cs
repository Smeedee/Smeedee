using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Framework;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.GenericCharting.Controllers
{
    public class ChartController : ControllerBase<ChartViewModel>
    {
        private IDownloadStringService downloadStringService;

        public ChartController(
            ChartViewModel viewModel,
            ITimer timer,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier,
            IDownloadStringService downloadStringService
            ) : base(viewModel, timer, uiInvoker, loadingNotifier)
        {

            Guard.Requires<ArgumentException>(downloadStringService != null);

            this.downloadStringService = downloadStringService;
            
            Start();

            DownloadAndAddDataToViewModel();
        }

        private void DownloadAndAddDataToViewModel()
        {
            throw new NotImplementedException();
        }


        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            DownloadAndAddDataToViewModel();
        }

        private Uri Url;

    }
}
