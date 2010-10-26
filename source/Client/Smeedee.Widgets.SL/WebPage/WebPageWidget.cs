using Smeedee.Client.Framework.Messages;
using Smeedee.Client.Framework.Services;
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
    	private readonly IEventAggregator eventAggregator;

    	public WebPageWidget()
        {
            webPageViewModel = GetInstance<WebPageViewModel>();
        	webPageController = GetInstance<WebPageController>();
			eventAggregator = GetInstance<IEventAggregator>();

			ConfigurationChanged += WebPageWidget_ConfigurationChanged;

    		var webPageView = new WebPageView {DataContext = webPageViewModel};
            SettingsView = new WebPageSettingsView {DataContext = webPageViewModel};

			eventAggregator.Subscribe<OpenModalDialogMessage>(this, msg =>
			{
				webPageView.HideWebBrowser();				
			});
			eventAggregator.Subscribe<CloseModalDialogMessage>(this, msg =>
			{
				webPageView.ShowWebBrowser();
			});

			SaveSettings.BeforeExecute += (o, e) => webPageController.SaveConfiguration();
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