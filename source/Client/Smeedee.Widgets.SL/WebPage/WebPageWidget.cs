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
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widgets.SL.WebPage.Views;
using Smeedee.Widgets.WebPage.ViewModel;

namespace Smeedee.Widgets.SL.WebPage
{
    [WidgetInfo(Name = "Web Page",
                Description = "Lets you display a web site",
                Author = "Smeedee team",
                Version = "1.0",
                Tags = new[] { CommonTags.Fun })]
    public class WebPageWidget : Widget
    {
        public WebPageWidget()
        {
            var viewModel = new WebPageViewModel();

            View = new WebPageView() {DataContext = null};
            SettingsView = new WebPageSettingsView() {DataContext = viewModel};
        }
    }
}
