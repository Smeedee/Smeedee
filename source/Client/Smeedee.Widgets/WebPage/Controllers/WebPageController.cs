using System;
using System.ComponentModel;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.Framework;
using Smeedee.Widgets.WebPage.ViewModel;

namespace Smeedee.Widgets.WebPage.Controllers
{
	public class WebPageController
	{
		private WebPageViewModel webPageViewModel;
		private Configuration config;
		private ITimer timer;
		
        private const string refresh_interval = "refresh-interval";
		private const string url = "url";

		public WebPageController(WebPageViewModel webPageViewModel, Configuration configuration, ITimer timer)
		{
			Guard.Requires<ArgumentNullException>(webPageViewModel != null);
			Guard.Requires<ArgumentNullException>(configuration != null);
			Guard.Requires<ArgumentNullException>(timer != null);

			this.config = configuration;

			this.webPageViewModel = webPageViewModel;
			webPageViewModel.PropertyChanged += webPageViewModel_PropertyChanged;
			webPageViewModel.ReloadSettings.ExecuteDelegate = () => UpdateConfiguration(config);

			this.timer = timer;
			timer.Elapsed += new EventHandler(timer_Elapsed);
			ConfigureAndStartTimer();

			UpdateConfiguration(configuration);
		}

		private void ConfigureAndStartTimer()
		{
			timer.Start(webPageViewModel.RefreshIntervalInSeconds);
		}

		void webPageViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "RefreshInterval")
			{
				timer.Stop();
				ConfigureAndStartTimer();
			}
		}

		void timer_Elapsed(object sender, EventArgs e)
		{
			webPageViewModel.OnRefresh();
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

			webPageViewModel.InputUrl = config.GetSetting(url).Value;
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
