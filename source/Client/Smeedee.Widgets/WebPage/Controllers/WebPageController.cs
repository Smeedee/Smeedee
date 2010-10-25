using System;
using Smeedee.DomainModel.Config;
using Smeedee.Framework;
using Smeedee.Widgets.WebPage.ViewModel;

namespace Smeedee.Widgets.WebPage.Controllers
{
	public class WebPageController
	{
		private WebPageViewModel webPageViewModel;
		private Configuration config;
		private const string refresh_interval = "refresh-interval";
		private const string url = "url";

		public WebPageController(WebPageViewModel webPageViewModel, Configuration configuration)
		{
			Guard.Requires<ArgumentNullException>(webPageViewModel != null);
			Guard.Requires<ArgumentNullException>(configuration != null);

			this.config = configuration;
			this.webPageViewModel = webPageViewModel;

			UpdateConfiguration(configuration);
		}

		public static Configuration GetDefaultConfiguration()
		{
			var config = new Configuration("webpage");
			config.NewSetting(url, "");
			config.NewSetting(refresh_interval, "30");
			return config;
		}

		public void UpdateConfiguration(Configuration config)
		{
			Guard.Requires<ArgumentNullException>(config != null);
			Guard.Requires<ArgumentException>(config.ContainsSetting(url));
			Guard.Requires<ArgumentException>(config.ContainsSetting(refresh_interval));
			this.config = config;

			webPageViewModel.ValidatedUrl = config.GetSetting(url).Value;
			webPageViewModel.RefreshInterval = int.Parse(config.GetSetting(refresh_interval).Value);
		}

		public Configuration SaveConfiguration()
		{
			config.ChangeSetting(url, webPageViewModel.ValidatedUrl);
			config.ChangeSetting(refresh_interval, webPageViewModel.RefreshInterval.ToString());

			return config;
		}
	}
}
