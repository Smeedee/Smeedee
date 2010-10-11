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
    public class WebPageWidget : Client.Framework.ViewModel.Widget
    {
        public WebPageWidget()
        {
            var viewModel = new WebPageViewModel();

            View = new WebPageView {DataContext = null};
            SettingsView = new WebPageSettingsView {DataContext = viewModel};
        }
    }
}