using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.Widgets.WebPage.Controllers;
using Smeedee.Widgets.WebPage.ViewModel;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Widgets.Tests.WebPage.Controllers
{
	public class WebPageControllerSpec
	{
		[TestFixture]
		public class When_spawning
		{
			[Test]
			public void Then_assure_WebPage_arg_is_validated()
			{
				this.ShouldThrowException<ArgumentNullException>(() =>
					new WebPageController(null, WebPageController.GetDefaultConfiguration()));
			}
		}

		[TestFixture]
		public class When_get_default_configuration
		{
			private WebPageController controller;
			private Configuration config;

			[SetUp]
			public void Setup()
			{
				controller = new WebPageController(new WebPageViewModel(), WebPageController.GetDefaultConfiguration());
				config = WebPageController.GetDefaultConfiguration();
			}

			[Test]
			public void Then_assure_config_has_a_name()
			{
				config.Name.ShouldBe("webpage");
			}

			[Test]
			public void Then_assure_it_can_store_a_url()
			{
				config.ContainsSetting("url").ShouldBeTrue();
			}

			[Test]
			public void Then_assure_it_has_a_default_refresh_interval()
			{
				config.ContainsSetting("refresh-interval").ShouldBeTrue();
				config.GetSetting("refresh-interval").Value.ShouldBe("30");
			}
		}

		[TestFixture]
		public class When_update_configuration
		{
			private WebPageViewModel webPageViewModel;
			private WebPageController controller;
			private Configuration updatedConfig;

			[SetUp]
			public void Setup()
			{
				webPageViewModel = new WebPageViewModel();
				controller = new WebPageController(webPageViewModel, WebPageController.GetDefaultConfiguration());
				updatedConfig = WebPageController.GetDefaultConfiguration();
			}

			[Test]
			public void Then_assure_ViewModels_URL_is_changed()
			{
				updatedConfig.ChangeSetting("url", "http://smeedee.org");

				controller.UpdateConfiguration(updatedConfig);

				webPageViewModel.ValidatedUrl.ShouldBe("http://smeedee.org");
			}

			[Test]
			public void Then_assure_ViewModels_InputUrl_is_changed()
			{
				updatedConfig.ChangeSetting("url", "http://www.smeedee.org");

				controller.UpdateConfiguration(updatedConfig);

				webPageViewModel.InputUrl.ShouldBe("http://www.smeedee.org");
			}

			[Test]
			public void Then_assure_ViewModels_RefreshInterval_is_changed()
			{
				updatedConfig.ChangeSetting("refresh-interval", "10");

				controller.UpdateConfiguration(updatedConfig);

				webPageViewModel.RefreshInterval.ShouldBe(10);
			}

			[Test]
			public void Then_assure_configuration_is_validated()
			{
				var invalidConfig = new Configuration("webpage");

				this.ShouldThrowException<ArgumentNullException>(() =>
					controller.UpdateConfiguration(null));

				this.ShouldThrowException<ArgumentException>(() =>
					controller.UpdateConfiguration(invalidConfig));

				invalidConfig.NewSetting("url", "");

				this.ShouldThrowException<ArgumentException>(() =>
					controller.UpdateConfiguration(invalidConfig));

				invalidConfig.ChangeSetting("refresh-interval", "a string");

				this.ShouldThrowException<ArgumentException>(() =>
					controller.UpdateConfiguration(invalidConfig));
			}
		}

		[TestFixture]
		public class When_save_configuration
		{
			private WebPageViewModel webPageViewModel;
			private WebPageController controller;

			[SetUp]
			public void Setup()
			{
				webPageViewModel = new WebPageViewModel();
				controller = new WebPageController(webPageViewModel, WebPageController.GetDefaultConfiguration());
			}

			[Test]
			public void Then_assure_URL_is_extracted_from_ViewModel()
			{
				webPageViewModel.ValidatedUrl = "http://www.smeedee.org";

				var config = controller.SaveConfiguration();

				config.GetSetting("url").Value.ShouldBe("http://www.smeedee.org");
			}

			[Test]
			public void Then_assure_refresh_interval_is_extracted_from_ViewModel()
			{
				webPageViewModel.RefreshInterval = 10;

				var config = controller.SaveConfiguration();

				config.GetSetting("refresh-interval").Value.ShouldBe("10");
			}
		}
		
	}
}
