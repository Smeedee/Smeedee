using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.Widgets.SL.WebPage.Views;
using Smeedee.Widgets.WebPage.Controllers;
using Smeedee.Widgets.WebPage.ViewModel;
using TinyMVVM.Framework;

namespace Smeedee.Widgets.SL.WebPage
{
    [WidgetInfo(Name = "Web Page",
                Description = "Lets you display a web site",
                Author = "Smeedee team",
                Version = "1.0",
                Tags = new[] { CommonTags.Fun })]
    public class WebPageWidget : Client.Framework.ViewModel.Widget
    {
    	private readonly WebPageController webPageController;
    	private readonly WebPageViewModel webPageViewModel;

    	public WebPageWidget()
        {
            webPageViewModel = GetInstance<WebPageViewModel>();
        	webPageController = GetInstance<WebPageController>();

			ConfigurationChanged += WebPageWidget_ConfigurationChanged;

            View = new WebPageView {DataContext = null};
            SettingsView = new WebPageSettingsView {DataContext = webPageViewModel};

			SaveSettings.AfterExecute += (o, e) => webPageController.SaveConfiguration();
			webPageViewModel.Save.ExecuteDelegate = () => SaveSettings.Execute();
		}

		void WebPageWidget_ConfigurationChanged(object sender, System.EventArgs e)
		{
			webPageController.UpdateConfiguration(Configuration);
		}

		protected override Configuration NewConfiguration()
		{
			return WebPageController.GetDefaultConfiguration();
		}

		public override void Configure(DependencyConfigSemantics config)
		{
			base.Configure(config);
			config.Bind<WebPageViewModel>().To<WebPageViewModel>().InSingletonScope();
		}
    }
}