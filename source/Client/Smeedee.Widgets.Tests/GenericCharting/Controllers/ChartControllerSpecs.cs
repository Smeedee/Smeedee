using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Repositories.Charting;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.Widgets.GenericCharting.Controllers;
using Smeedee.Widgets.GenericCharting.ViewModels;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using TinyMVVM.Framework.Services;
using TinyMVVM.Framework.Testing.Services;
using TinyMVVM.IoC;

namespace Smeedee.Widgets.Tests.GenericCharting.Controllers
{
    class ChartControllerSpecs
    {

        [TestFixture]
        public class When_controller_is_spawning : Shared
        {

            [Test]
            public void Then_null_viewModelArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as viewModel");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(null, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object, storageReader.Object, configuration)));
            }

            [Test]
            public void Then_null_timerArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as timer");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, null, uIInvokerFake.Object, loadingNotifierFake.Object, storageReader.Object, configuration)));
            }

            [Test]
            public void Then_null_uIInvokerArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as uIInvoker");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, timerFake.Object, null, loadingNotifierFake.Object, storageReader.Object, configuration)));
            }

            [Test]
            public void Then_null_loadingNotifierArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as loadingNotifier");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, timerFake.Object, uIInvokerFake.Object, null, storageReader.Object, configuration)));
            }

            [Test]
            public void Then_null_storageReaderArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as storageReader");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object ,null, configuration)));
            }

            [Test]
            public void Then_null_configurationArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as configuration");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                         () =>
                         new ChartController(chartViewModel, timerFake.Object, uIInvokerFake.Object,
                                             loadingNotifierFake.Object, storageReader.Object, null)));
            }

            [Test]
            public void Then_configuration_should_contain_Database_settings()
            {
                Given(the_controller_has_been_created);
                When("it is created");
                Then("the configuration should contain database settings");
            }
            //Tests for:    configuration contains RefreshInterval
            //              configuration contains Database settings
            //              configuration contains Collection Settings
            //              configuration contains XAxisPropertyName
            //              configuration contains YAxisPropertyName

        }

        [TestFixture]
        public class When_controller_is_spawned : Shared
        {
            [Test]
            public void Then_assure_viewModel_is_not_null()
            {
                Given(the_controller_has_been_created);
                When("it is created");
                Then("the viewModel should not be null", () => chartController.ViewModel.ShouldNotBeNull());
            }

            

            [Test]
            public void Then_assure_timer_is_started()
            {
                Given(the_controller_has_been_created);
                When("it has been created");
                Then("the timer should have started",
                     () => timerFake.Verify(t => t.Start(It.IsAny<int>()), Times.Once()));
            }
        }

        //[TestFixture]
        //public class When_OnNotifiedToRefresh_is_called :Shared
        //{
            //[Test]
            //[Ignore]
            //public void Then_assure_data_is_loaded_to_viewModel()
            //{

            //    Given(the_controller_has_been_created);
            //    When("OnNotifiedToRefresh is called", () => timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty));
            //    Then("data should be loaded to chartViewModel", () => "");

            //}

            //[Test]
            //public void assure_data_is_loaded_from_storage()
            //{
            //    Given(the_controller_has_been_created).And(some_data_exists_in_storage);
            //    When("");
            //    Then("data is loaded from storage", () =>
            //                                            {
                                                            
            //                                            });
            //}

            //private static Chart chart;
            //private static void SetChart(Chart c)
            //{
            //    chart = c;
            //} 
            //protected Context some_data_exists_in_storage = () =>
            //                                                    {
            //                                                        var chart = new Chart("mockDB", "mockCollection");
            //                                                        var dataset = new DataSet("mockSet");
            //                                                        dataset.DataPoints.Add(1);
            //                                                        chart.DataSets.Add(dataset);

            //                                                        storageReader.Setup(
            //                                                            s =>
            //                                                            s.LoadChart(It.IsAny<string>(), It.IsAny<string>()))
            //                                                            .Callback((string s,string callback) => SetChart(chart));
            //                                                    };
        //}
        

        public class Shared : ScenarioClass
        {
            protected static ChartController chartController;
            protected static ChartViewModel chartViewModel;

            protected static Configuration configuration;
            
            protected static Mock<ITimer> timerFake;
            protected static Mock<IUIInvoker> uIInvokerFake;
            protected static Mock<IProgressbar> loadingNotifierFake;
            protected static Mock<IChartStorageReader> storageReader;

            protected Context the_controller_has_been_created = 
                () => chartController = new ChartController(chartViewModel, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object, storageReader.Object, configuration);
            


            //protected Mock<IRepository<ChartViewModel>> ChartRepositoryFake = new Mock<IRepository<ChartViewModel>>();
            //protected Mock<IRepository<DataPointViewModel>> DataPointRepositoryFake = new Mock<IRepository<DataPointViewModel>>();
            //protected Mock<IRepository<ChartSettingsViewModel>> ChartSettingsRepositoryFake = new Mock<IRepository<ChartSettingsViewModel>>();
            //protected Mock<IRepository<DatabaseViewModel>> DataBaseRepositoryFake = new Mock<IRepository<DatabaseViewModel>>();
            //protected Mock<IRepository<CollectionViewModel>> CollectionRepositoryFake = new Mock<IRepository<CollectionViewModel>>();

            [SetUp]
            public void Setup()
            {
                Scenario("");
                chartViewModel = new ChartViewModel();

                timerFake = new Mock<ITimer>();
                uIInvokerFake = new Mock<IUIInvoker>();
                loadingNotifierFake = new Mock<IProgressbar>();
                storageReader = new Mock<IChartStorageReader>();

                configuration = new Configuration();

           


                //RemoveAllGlobalDependencies.ForAllViewModels();
                //ConfigureGlobalDependencies.ForAllViewModels(config =>
                //{
                //    config.Bind<IUIInvoker>().To<UIInvokerForTesting>();

                //    config.Bind<IRepository<ChartViewModel>>().ToInstance(
                //        ChartRepositoryFake.Object);
                //    config.Bind<IRepository<DataPointViewModel>>().
                //        ToInstance(DataPointRepositoryFake.Object);
                //    config.Bind<IRepository<ChartSettingsViewModel>>().
                //        ToInstance(ChartSettingsRepositoryFake.Object);
                //    config.Bind<IRepository<DatabaseViewModel>>().
                //        ToInstance(DataBaseRepositoryFake.Object);
                //    config.Bind<IRepository<CollectionViewModel>>().
                //        ToInstance(CollectionRepositoryFake.Object);
                //});
            }

            [TearDown]
            public void TearDown()
            {
                //RemoveAllGlobalDependencies.ForAllViewModels();
                StartScenario();
            } 
        }
    }
}
