using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;
using TinyMVVM.Repositories;

namespace Smeedee.Widgets.Tests.GenericCharting.ViewModels
{


    [TestFixture]
    [Ignore]
    public class When_chartviewmodel_is_created : Shared
    {
        private static ChartViewModel viewModel;
    }

    [TestFixture]
    [Ignore]
    public class When_datapointviewmodel_is_created : Shared
    {
        private static DataPointViewModel dataPointViewModel;

        [Test]
        public void Assure_it_has_x_value()
        {
            Given("datapointviewmodel has been created", () => dataPointViewModel = new DataPointViewModel());
            When("");
            Then("datapointvievmodel should have a x-value", () => dataPointViewModel.X.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_y_value()
        {
            Given("datapointviewmodel has been created", () => dataPointViewModel = new DataPointViewModel());
            When("");
            Then("datapointvievmodel should have a y-value", () => dataPointViewModel.Y.ShouldNotBeNull());
        }
    }

    [TestFixture]
    [Ignore]
    public class When_charttypeviewmodel_is_created : Shared
    {
        private ChartTypeViewModel chartTypeViewModel;

        [Test]
        public void Assure_it_has_charttype_value()
        {
            Given("chartTypeViewModel has been created", () => chartTypeViewModel = new ChartTypeViewModel());
            When("");
            Then("chartTypeViewModel should have a name value", () => chartTypeViewModel.ChartType.ShouldNotBeNull());
        }
    }

    [TestFixture]
    [Ignore]
    public class When_CollectionViewModel_is_created : Shared
    {
        private CollectionViewModel collectionViewModel;
        [Test]
        public void Assure_it_has_Name()
        {
            Given("collectionViewModel has been created", () => collectionViewModel = new CollectionViewModel());
            When("");
            Then("it should have a name", () => collectionViewModel.Name.ShouldNotBeNull());
        }
    }

    [TestFixture]
    public class When_ChartSettingsViewModel_is_created : Shared
    {
        [Test]
        public void Assure_it_has_Databases()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("it is created");
            Then("assure it contains databases", () => chartSettingsViewModel.Databases.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_Collections()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("it is created");
            Then("assure it contains collections", () => chartSettingsViewModel.Collections.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_SeriesConfig()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("it is created");
            Then("assure it contains seriesconfig", () => chartSettingsViewModel.SeriesConfig.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_correct_XAxisTypes()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("it is created");
            Then("assure it contains correct XAxisTypes", () =>
                                                              {
                                                                  chartSettingsViewModel.XAxisTypes.ShouldNotBeNull();
                                                              });
        }


    }

    public class Shared : ScenarioClass
    {

        protected static ChartSettingsViewModel chartSettingsViewModel;

        protected Context chartSettingsViewModel_has_been_created =
            () => chartSettingsViewModel = new ChartSettingsViewModel();

        [SetUp]
        public void Setup()
        {
            Scenario("");
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }
}
