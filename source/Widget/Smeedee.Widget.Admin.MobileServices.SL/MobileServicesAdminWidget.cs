using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.Widget.Admin.MobileServices.SL.Views;
using Smeedee.Widget.Admin.MobileServices.ViewModels;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widget.Admin.MobileServices.SL
{
    [WidgetInfo(Name = "Remote Services Administration")]
    public class MobileServicesAdminWidget : Client.Framework.ViewModel.Widget
    {
        private MobileServicesAuthenticationViewModel viewModel;

        public MobileServicesAdminWidget()
        {
            Title = "Remote Services Administration";

            var configRepo = GetInstance<IAsyncRepository<Configuration>>();
            var configSaver = GetInstance<IPersistDomainModelsAsync<Configuration>>();
            var loadingNotifier = GetInstance<IProgressbar>();
            var uiInvoker = GetInstance<IUIInvoker>();

            viewModel =  new MobileServicesAuthenticationViewModel(configRepo, configSaver, loadingNotifier, uiInvoker);

            View = new MobileServicesAdminView() { DataContext = viewModel };
        }
    }
}
