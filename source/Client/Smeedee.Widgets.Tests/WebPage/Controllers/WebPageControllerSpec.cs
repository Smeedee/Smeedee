using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
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
					new WebPageController(null, WebPageController.GetDefaultConfiguration(), new StandardTimer()));
			}

			[Test]
			public void Then_assure_Timer_arg_is_validated()
			{
				this.ShouldThrowException<ArgumentNullException>(() =>
					new WebPageController(new WebPageViewModel(), WebPageController.GetDefaultConfiguration(), null));
			}
		}

		[TestFixture]
		public class When_get_default_configuration : Shared
		{
			private Configuration config;

			public override void Before()
			{
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
		public class When_update_configuration : Shared
		{
			private Configuration updatedConfig;

			public override void Before()
			{
				updatedConfig = WebPageController.GetDefaultConfiguration();
			}

			[Test]
			public void Then_assure_ViewModels_URL_is_changed()
			{
				updatedConfig.ChangeSetting("url", "http://smeedee.org");

				webPageController.UpdateConfiguration(updatedConfig);

				webPageViewModel.ValidatedUrl.ShouldBe("http://smeedee.org");
			}

			[Test]
			public void Then_assure_ViewModels_InputUrl_is_changed()
			{
				updatedConfig.ChangeSetting("url", "http://www.smeedee.org");

				webPageController.UpdateConfiguration(updatedConfig);

				webPageViewModel.InputUrl.ShouldBe("http://www.smeedee.org");
			}

			[Test]
			public void Then_assure_ViewModels_RefreshInterval_is_changed()
			{
				updatedConfig.ChangeSetting("refresh-interval", "10");

				webPageController.UpdateConfiguration(updatedConfig);

				webPageViewModel.RefreshInterval.ShouldBe(10);
			}

			[Test]
			public void Then_assure_configuration_is_validated()
			{
				var invalidConfig = new Configuration("webpage");

				this.ShouldThrowException<ArgumentNullException>(() =>
					webPageController.UpdateConfiguration(null));

				this.ShouldThrowException<ArgumentException>(() =>
					webPageController.UpdateConfiguration(invalidConfig));

				invalidConfig.NewSetting("url", "");

				this.ShouldThrowException<ArgumentException>(() =>
					webPageController.UpdateConfiguration(invalidConfig));

				invalidConfig.ChangeSetting("refresh-interval", "a string");

				this.ShouldThrowException<ArgumentException>(() =>
					webPageController.UpdateConfiguration(invalidConfig));
			}
		}

		[TestFixture]
		public class When_save_configuration : Shared
		{
			[SetUp]
			public void Setup()
			{
				webPageViewModel = new WebPageViewModel();
				webPageController = new WebPageController(webPageViewModel, WebPageController.GetDefaultConfiguration(), new StandardTimer());
			}

			[Test]
			public void Then_assure_URL_is_extracted_from_ViewModel()
			{
				webPageViewModel.ValidatedUrl = "http://www.smeedee.org";

				var config = webPageController.SaveConfiguration();

				config.GetSetting("url").Value.ShouldBe("http://www.smeedee.org");
			}

			[Test]
			public void Then_assure_refresh_interval_is_extracted_from_ViewModel()
			{
				webPageViewModel.RefreshInterval = 10;

				var config = webPageController.SaveConfiguration();

				config.GetSetting("refresh-interval").Value.ShouldBe("10");
			}
		}


		[TestFixture]
		public class When_Refresh : Shared 
		{
			private int ten = 10;

			[Test]
			public void Then_assure_ValidatedUrl_is_updated()
			{
				webPageViewModel.InputUrl = "http://smeedee.org:8111";
				webPageViewModel.PropertyChangeRecorder.Start();

				ten.Times(() => timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty));

				webPageViewModel.PropertyChangeRecorder.Data.Count(r => r.PropertyName == "ValidatedUrl").ShouldBe(ten);
			}
		}

		[TestFixture]
		public class When_ViewModels_RefreshInterval_is_changed : Shared
		{
			public override void Before()
			{
				webPageViewModel.RefreshInterval = 10;
			}

			[Test]
			public void Then_assure_Timer_is_restarted()
			{
				timerFake.Verify(t => t.Stop(), Times.Once());
				timerFake.Verify(t => t.Start(It.Is<int>((i) => i == webPageViewModel.RefreshIntervalInSeconds)), Times.Once());
			}
		}

		public class Shared
		{
			protected Mock<ITimer> timerFake;
			protected WebPageViewModel webPageViewModel;
			protected WebPageController webPageController;

			[SetUp]
			public void Setup()
			{
				timerFake = new Mock<ITimer>();
				webPageViewModel = new WebPageViewModel();
				webPageController = new WebPageController(webPageViewModel, WebPageController.GetDefaultConfiguration(), timerFake.Object);
				Before();
			}

			public virtual void Before()
			{
				
			}
		}
	}

	public static class IntExtensions
	{
		public static void Times(this int n, Action codeBlock)
		{
			for (int i = 0; i < n; i++)
			{
				codeBlock.Invoke();	
			}
		}
	}
}
