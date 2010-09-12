using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.Client.Framework.Tests;
using Smeedee.Client.Framework.ViewModel;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Config.SlideConfig;
using Smeedee.DomainModel.Framework;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Services.Impl;
using TinyMVVM.IoC;

namespace Smeedee.Client.Tests.ViewModel
{
    class WidgetTests
    {
    	[TestFixture]
    	public class When_OnInitialize
    	{
    		private CustomWidget widget;

    		[SetUp]
    		public void Setup()
    		{
    		    ViewModelBootstrapperForTests.Initialize();
    			widget = new CustomWidget();
    		}

    		[Test]
    		public void Then_assure_Configure_is_called()
    		{
    			widget.ConfigureCallCount.ShouldBe(1);
    		}

    	    [Test]
    	    public void Assure_Configuration_is_set_from_NewConfiguration()
    	    {
                widget.Configuration.Name.ShouldBe("hello world!");
            }
    	}

        [TestFixture]
        public class When_spawned : WidgetTestContext
        {
            private Mock<IManageConfigurations> configManagerMock = new Mock<IManageConfigurations>();

            public override void Context()
            {
                RemoveAllGlobalDependencies.ForAllViewModels();
                ConfigureGlobalDependencies.ForAllViewModels(config =>
                {
                    config.Bind<IBackgroundWorker>().To<BackgroundWorkerForTesting>();
                    config.Bind<IPersistDomainModelsAsync<Configuration>>().ToInstance(new Mock<IPersistDomainModelsAsync<Configuration>>().Object);
                    config.Bind<IManageConfigurations>().ToInstance(configManagerMock.Object);
                });
                Given_Widget_is_created();
            }

            [Test]
            public void Assure_widget_is_registered_with_config_manager_when_id_is_set()
            {
                viewModel.SetConfigurationId(new Guid());
                configManagerMock.Verify(c => c.RegisterForConfigurationUpdates(viewModel), Times.Once());
            }

            [Test]
            public void assure_it_has_a_ErrorInfo()
            {
                viewModel.ErrorInfo.ShouldNotBeNull();
            }

            [Test]
            public void assure_it_has_a_IsInSettingsMode_flag()
            {
                viewModel.IsInSettingsMode.ShouldBe(false);
            }

            [Test]
            public void assure_Settings_command_can_be_executed()
            {
                viewModel.Settings.CanExecute(null).ShouldBeTrue();
            }

        	[Test]
        	public void assure_it_has_a_ViewProgressbar()
        	{
        		viewModel.ViewProgressbar.ShouldNotBeNull();
        	}

        	[Test]
        	public void assure_it_has_a_SettingsProgressbar()
        	{
        		viewModel.SettingsProgressbar.ShouldNotBeNull();
        	}

        	[Test]
        	public void assure_it_has_a_ProgressbarService()
        	{
        		viewModel.GetDependency<IProgressbar>().ShouldBeInstanceOfType<WidgetProgressbar>();
        	}

        	[Test]
        	public void assure_Global_Dependencies_are_added()
            {
        		viewModel.GetDependency<IBackgroundWorker>().ShouldBeInstanceOfType<BackgroundWorkerForTesting>();
        	}
        }

		[TestFixture]
		public class When_Settings_Command_is_executed : WidgetTestContext
		{
			public override void Context()
			{
				Given_Widget_is_created();

				When_execute_Settings_Command();
			}

			[Test]
			public void assure_is_in_settings_mode()
			{
				viewModel.IsInSettingsMode.ShouldBeTrue();
			}


			[Test]
			public void assure_it_goes_back_to_view_mode_when_executed_againt()
			{
				When_execute_Settings_Command();

				viewModel.IsInSettingsMode.ShouldBeFalse();
			}

		}

        [TestFixture]
        public class When_reporting_a_failure : WidgetTestContext
        {
            private FailingTraybarWidget failingTraybarWidget;


            public override void Context()
            {
                Given_failingTraybarWidget_is_created();

                report_failure("something went wrong");
            }

            private void Given_failingTraybarWidget_is_created()
            {
                failingTraybarWidget = new FailingTraybarWidget();
            }

            private void report_failure(string msg)
            {
                failingTraybarWidget.Report(msg);
            }

            [Test]
            public void assure_failure_is_reported()
            {
                failingTraybarWidget.ErrorInfo.HasError.ShouldBeTrue();
            }

            [Test]
            public void assure_error_msg_is_provided_in_error_report()
            {
                failingTraybarWidget.ErrorInfo.ErrorMessage.ShouldBe("something went wrong");
            }

            [Test]
            public void assure_failure_report_can_be_removed()
            {
                failingTraybarWidget.RemoveReport();

                failingTraybarWidget.ErrorInfo.HasError.ShouldBeFalse();
                failingTraybarWidget.ErrorInfo.ErrorMessage.ShouldBe(string.Empty);
            }

        }

        class FailingTraybarWidget : Widget
        {
            public FailingTraybarWidget()
            {
            }

            public void Report(string msg)
            {
                ReportFailure(msg);
            }

            public void RemoveReport()
            {
                NoFailure();
            }
        }

		class CustomWidget : Widget
		{
			public int ConfigureCallCount { get; set; }

			public override void Configure(DependencyConfigSemantics config)
			{
				ConfigureCallCount++;
			}

            protected override Configuration NewConfiguration()
            {
                return new Configuration("hello world!");
            }
		}
    }
}
