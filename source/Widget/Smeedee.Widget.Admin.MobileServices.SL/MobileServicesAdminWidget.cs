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
using Smeedee.Client.Framework.ViewModel;
using Smeedee.Widget.Admin.MobileServices.SL.Views;
using Smeedee.Widget.Admin.MobileServices.ViewModels;

namespace Smeedee.Widget.Admin.MobileServices.SL
{
    [WidgetInfo(Name = "Remote Services Administration")]
    public class MobileServicesAdminWidget : Client.Framework.ViewModel.Widget
    {
        public MobileServicesAdminWidget()
        {
            Title = "Remote Services Administration";

            var viewModel =  new MobileServicesAuthenticationViewModel();

            View = new MobileServicesAdminView() { DataContext = viewModel };
        }
    }
}
