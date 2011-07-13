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
        [Test]
        public void Assure_it_has_name_value()
        {

        }

        [Test]
        public void Assure_it_has_collections()
        {

        }
    }

    [TestFixture]
    public class When_CollectionViewModel_is_created : Shared
    {
        [Test]
        public void Assure_it_has_value()
        {

        }
    }

    [TestFixture]
    public class When_ChartSettingsViewModel_is_created : Shared
    {
        [Test]
        public void Assure_it_has_value()
        {

        }
    }

    public class Shared : ScenarioClass
    {
        //protected static ChartViewModel chartViewModel;
        //protected static DataPointViewModel dataPointViewModel;

        protected Mock<IRepository<ChartViewModel>> ChartRepositoryFake = new Mock<IRepository<ChartViewModel>>();
        protected Mock<IRepository<DataPointViewModel>> DataPointRepositoryFake = new Mock<IRepository<DataPointViewModel>>();
        protected Mock<IRepository<ChartSettingsViewModel>> ChartSettingsRepositoryFake = new Mock<IRepository<ChartSettingsViewModel>>();
        protected Mock<IRepository<DatabaseViewModel>> DataBaseRepositoryFake = new Mock<IRepository<DatabaseViewModel>>();
        protected Mock<IRepository<CollectionViewModel>> CollectionRepositoryFake = new Mock<IRepository<CollectionViewModel>>();

        [SetUp]
        public void Setup()
        {
            RemoveAllGlobalDependencies.ForAllViewModels();
            ConfigureGlobalDependencies.ForAllViewModels(config =>
                                                             {
                                                                 config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

                                                                 config.Bind<IRepository<ChartViewModel>>().ToInstance(
                                                                     ChartRepositoryFake.Object);
                                                                 config.Bind<IRepository<DataPointViewModel>>().
                                                                     ToInstance(DataPointRepositoryFake.Object);
                                                                 config.Bind<IRepository<ChartSettingsViewModel>>().
                                                                     ToInstance(ChartSettingsRepositoryFake.Object);
                                                                 config.Bind<IRepository<DatabaseViewModel>>().
                                                                     ToInstance(DataBaseRepositoryFake.Object);
                                                                 config.Bind<IRepository<CollectionViewModel>>().
                                                                     ToInstance(CollectionRepositoryFake.Object);
                                                             });
        }

        [TearDown]
        public void TearDown()
        {
            RemoveAllGlobalDependencies.ForAllViewModels();
            StartScenario();
        }


        
        //protected Context datapointviewmodel_has_been_created = () => dataPointViewModel = new DataPointViewModel();


        //protected Context chartViewModel_has_been_created = () => chartViewModel = new ChartViewModel();
        //protected When data_is_added = () => chartViewModel.Data.Add(new DataPointViewModel());
        //protected When chartViewModel_is_created = () => chartViewModel = new ChartViewModel();

    }
}
