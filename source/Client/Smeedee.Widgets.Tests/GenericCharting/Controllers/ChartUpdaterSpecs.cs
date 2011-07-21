using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Repositories.Charting;
using Smeedee.Client.Framework.Repositories.NoSql;
using Smeedee.Client.Framework.Services;
using Smeedee.Client.Framework.Services.Impl;
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.NoSql;
using Smeedee.Widgets.GenericCharting.Controllers;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;

namespace Smeedee.Widgets.Tests.GenericCharting.Controllers
{
    public class ChartUpdaterSpecs
    {

        [TestFixture]
        public class When_updating_view : Shared
        {
            [Test]
            public void Then_invalid_configuration_should_result_in_an_error_message()
            {
                Given("default configuration");
                When(calling_update);
                Then("there should be an error message in viewmodel", () =>
                                                                          {
                                                                              viewModel.ShowErrorMessageInsteadOfChart.ShouldBe(true);
                                                                              viewModel.ErrorMessage.ShouldNotBeNull();
                                                                              viewModel.ErrorMessage.Count().ShouldNotBe(0);
                                                                          });
            }

            [Test]
            public void Then_a_valid_configuration_should_hide_error_message()
            {
                Given("a valid configuration", () =>
                    {
                        chartConfig.SetSeries(new Collection<SeriesConfigViewModel> { new SeriesConfigViewModel { Action = ChartConfig.SHOW } });
                        chartConfig.IsConfigured = true;
                    });
                When(calling_update);
                Then("error message should be hidden", () =>
                    {
                        viewModel.ShowErrorMessageInsteadOfChart = false;
                        viewModel.ErrorMessage.ShouldBeNull();
                    });
            }

            private When calling_update = () => updater.Update();
        }

        public class Shared : ScenarioClass
        {
            protected static ChartUpdater updater;
            protected static ChartConfig chartConfig;
            protected static ChartViewModel viewModel;
            protected static IUIInvoker uiInvoker;
            protected static Mock<IChartStorageReader> storageReaderFake;

            [SetUp]
            public void SetUp()
            {
                Scenario("");
                chartConfig = new ChartConfig(ChartConfig.NewDefaultConfiguration());
                viewModel = new ChartViewModel();
                storageReaderFake = new Mock<IChartStorageReader>();
                uiInvoker = new NoUIInvokation();
                
                updater = new ChartUpdater(viewModel, chartConfig, uiInvoker);
            }

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }
    }
}
