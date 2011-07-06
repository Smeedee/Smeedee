using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Smeedee.Client.Framework.Controller;
using Smeedee.Client.Framework.Services;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.GenericCharting.Controllers
{
    public class ChartController : ControllerBase<ChartViewModel>
    {
        public ChartController(
            ChartViewModel viewModel,
            ITimer timer,
            IUIInvoker uiInvoker,
            IProgressbar loadingNotifier
            ) : base(viewModel, timer, uiInvoker, loadingNotifier)
        {
            
        }

        
        
        protected override void OnNotifiedToRefresh(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
