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
    public class When_chartviewmodel_is_created : Shared 
        
    {

        private static ChartViewModel chartViewModel;

        [Test]
        public void Assure_it_has_Data()
        {
            Given("chartViewModel has been created", () => chartViewModel = new ChartViewModel());
            When("");
            Then("chartViewModel should not be null", () => chartViewModel.Data.ShouldNotBeNull());
        }

    }

    [TestFixture]
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
    public class When_DatabaseViewModel_is_created : Shared
    {
        private DatabaseViewModel dataBaseViewModel;

        [Test]
        public void Assure_it_has_collections()
        {
            Given("dataBaseViewModel has been created", () => dataBaseViewModel = new DatabaseViewModel());
            When("");
            Then("dataBaseViewModel should have a collection", () => dataBaseViewModel.Collections.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_name()
        {
            Given("dataBaseViewModel has been created", () => dataBaseViewModel = new DatabaseViewModel());
            When("");
            Then("Name should not be null", () => dataBaseViewModel.Name.ShouldNotBeNull());
        }
       
    }

    [TestFixture]
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
        public void Assure_it_has_ChartName()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("");
            Then("chartName should not be null", () => chartSettingsViewModel.ChartName.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_XAxisName()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("");
            Then("XAxisName should not be null", () => chartSettingsViewModel.XAxisName.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_YAxisName()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("");
            Then("YAxisName should not be null", () => chartSettingsViewModel.YAxisName.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_ChartType()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("");
            Then("ChartType should not be null", () => chartSettingsViewModel.ChartType.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_Databases()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("");
            Then("Databases should not be null", () => chartSettingsViewModel.Databases.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_SelectedDatabase()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("");
            Then("SelectedDataBase should not be null", () => chartSettingsViewModel.SelectedDatabase.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_SelectedCollection()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("");
            Then("SelectedCollection should not be null", () => chartSettingsViewModel.SelectedCollection.ShouldNotBeNull());
        }

        [Test]
        public void Assure_it_has_AvailableProperties()
        {
            Given(chartSettingsViewModel_has_been_created);
            When("");
            Then("AvailableProperties should not be null", () => chartSettingsViewModel.AvailableProperties.ShouldNotBeNull());
        }

        //Not implemented
        //[Test]
        //public void Assure_it_has_SelectedPropertyForXAxis()
        //{
        //    Given(chartSettingsViewModel_has_been_created);
        //    When("");
        //    Then("SelectedProperty");
        //}

        //[Test]
        //public void Assure_it_has_SelectedPropertyForYAxis()
        //{
        //    Given("");
        //    When("");
        //    Then("");
        //}

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
