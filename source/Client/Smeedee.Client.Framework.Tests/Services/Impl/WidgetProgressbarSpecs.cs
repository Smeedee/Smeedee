using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.ViewModel;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;

namespace Smeedee.Client.Framework.Tests.Services.Impl
{
	public class WidgetProgressbarSpecs
	{
		[TestFixture]
		public class When_spawning : Shared
		{
			[SetUp]
			public void Setup()
			{
				Given(View_and_SettingsProgressbar_is_created);

				When("spawning");
			}

			[Test]
			public void assure_ViewProgressbar_arg_is_validated()
			{
				Then(() =>
					this.ShouldThrowException<ArgumentException>(() =>
						new WidgetProgressbar(uiInvoker, null, settingsProgressbar)));
			}

			[Test]
			public void assure_SettingsProgressbar_arg_is_validated()
			{
				Then(() =>
					this.ShouldThrowException<ArgumentException>(() =>
						new WidgetProgressbar(uiInvoker, viewProgressbar, null)));
			}
		}

		[TestFixture]
		public class When_ShowInView : Shared
		{
			[SetUp]
			public void Setup()
			{
				Given(View_and_SettingsProgressbar_is_created);
				And(WidgetProgressbar_service_is_created);

				When("ShowInView", () =>
					widgetProgressbarService.ShowInView("Loading data from server..."));
			}

			[Test]
			public void assure_ViewProgressbar_is_Visible()
			{
				Then(() =>
				     viewProgressbar.IsVisible.ShouldBeTrue());
			}

			[Test]
			public void assure_ViewProgressbar_message_is_set()
			{
				Then(() =>
				     viewProgressbar.Message.ShouldBe("Loading data from server..."));
			}
		}

		[TestFixture]
		public class When_HideInView : Shared
		{
			[SetUp]
			public void Setup()
			{
				Given(View_and_SettingsProgressbar_is_created);
				And(WidgetProgressbar_service_is_created);
				And("ViewProgressbar is displayed", () =>
				{
					viewProgressbar.IsVisible = true;
					viewProgressbar.Message = "Loading data...";
				});

				When("HideInView", () =>
					widgetProgressbarService.HideInView());
			}

			[Test]
			public void assure_ViewProgressbar_is_not_visible()
			{
				Then(() => viewProgressbar.IsVisible.ShouldBeFalse());
			}

			[Test]
			public void assure_ViewProgressbar_Message_is_reset()
			{
				Then(() => viewProgressbar.Message.ShouldBe(string.Empty));
			}
		}

		[TestFixture]
		public class When_ShowInSettingsView : Shared
		{
			[SetUp]
			public void Setup()
			{
				Given(View_and_SettingsProgressbar_is_created);
				And(WidgetProgressbar_service_is_created);

				When("ShowInSettingsView", () =>
					widgetProgressbarService.ShowInSettingsView("Loading settings..."));
			}

			[Test]
			public void assure_SettingsProgreessbar_is_visible()
			{
				Then(() => settingsProgressbar.IsVisible.ShouldBeTrue());
			}

			[Test]
			public void assure_SettingsProgressbar_Message_is_set()
			{
				Then(() => settingsProgressbar.Message.ShouldBe("Loading settings..."));
			}
		}

		public class Shared : ScenarioClass
		{
			protected static IUIInvoker uiInvoker = new NoUIInvokation();
			protected static Progressbar viewProgressbar;
			protected static Progressbar settingsProgressbar;
			protected static WidgetProgressbar widgetProgressbarService;

			protected Context View_and_SettingsProgressbar_is_created = () =>
			{
				viewProgressbar = new Progressbar();
				settingsProgressbar = new Progressbar();
			};

			protected Context WidgetProgressbar_service_is_created = () =>
			{
				widgetProgressbarService = new WidgetProgressbar(uiInvoker, viewProgressbar, settingsProgressbar);
			};



			[TearDown]
			public void TearDown()
			{
				StartScenario();
			}
		}
	}
}
