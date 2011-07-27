using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.Widget.Admin.MobileServices.SL.Views;
using Smeedee.Widget.Admin.MobileServices.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.Admin.MobileServices.SL
{
    [WidgetInfo(Name = "Smeedee for Mobile Devices")]
    public class MobileServicesAdminWidget : Client.Framework.ViewModel.Widget
    {
        private MobileServicesAuthenticationViewModel viewModel;

        public MobileServicesAdminWidget()
        {
            Title = "View data using Smeedee Mobile";

            var configRepo = GetInstance<IAsyncRepository<Configuration>>();
            var configSaver = GetInstance<IPersistDomainModelsAsync<Configuration>>();
            var uiInvoker = GetInstance<IUIInvoker>();

            viewModel =  new MobileServicesAuthenticationViewModel(configRepo, configSaver, uiInvoker);

            View = new MobileServicesAdminView { DataContext = viewModel };
        }
    }
}
