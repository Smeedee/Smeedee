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
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.NoSql;
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
                        () => new ChartController(null, settingsViewModel, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object, storageReaderFake.Object, configuration)));
            }

            [Test]
            public void Then_null_timerArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as timer");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(viewModel, settingsViewModel, null, uIInvokerFake.Object, loadingNotifierFake.Object, storageReaderFake.Object, configuration)));
            }

            [Test]
            public void Then_null_uIInvokerArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as uIInvoker");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(viewModel, settingsViewModel, timerFake.Object, null, loadingNotifierFake.Object, storageReaderFake.Object, configuration)));
            }

            [Test]
            public void Then_null_loadingNotifierArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as loadingNotifier");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(viewModel, settingsViewModel, timerFake.Object, uIInvokerFake.Object, null, storageReaderFake.Object, configuration)));
            }

            [Test]
            public void Then_null_storageReaderArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as storageReader");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(viewModel, settingsViewModel, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object, null, configuration)));
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
                         new ChartController(viewModel, settingsViewModel, timerFake.Object, uIInvokerFake.Object,
                                             loadingNotifierFake.Object, storageReaderFake.Object, null)));
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
                Then("the viewModel should not be null", () => controller.ViewModel.ShouldNotBeNull());
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

        [TestFixture]
        public class When_updating_list_of_datasources : Shared
        {


            [Test]
            public void Then_assure_that_there_are_no_databases_in_viewmodel_when_there_are_none_in_storage()
            {
                Given(there_are_no_databases).
                    And(the_controller_has_been_created);
                When("finished loading datasources");
                Then("there should not be any databases in viewmodel", () => 
                    controller.SettingsViewModel.Databases.Count.ShouldBe(0)
                    );
            }

            [Test]
            public void Then_assure_that_one_database_is_returned_when_there_is_only_one()
            {
                Given(there_is_one_database).
                    And(the_controller_has_been_created);
                When("finished loading datasources");
                Then("settingsViewModel should contain a database", () =>
                                                                        {
                                                                            settingsViewModel.Databases.Count.ShouldBe(1);
                                                                            settingsViewModel.Databases[0].ShouldBe("TestDatabase");
                                                                        });
            }

            [Test]
            public void Then_assure_that_two_database_is_returned_when_there_is_two()
            {
                Given(there_are_two_databases).
                    And(the_controller_has_been_created);
                When("finished loading datasources");
                Then("settingsViewModel should contain two databases", () =>
                {
                    settingsViewModel.Databases.Count.ShouldBe(2);
                    settingsViewModel.Databases[0].ShouldBe("TestDatabase1");
                    settingsViewModel.Databases[1].ShouldBe("TestDatabase2");
                });
            }
/*
            [Test]
            public void Then_assure_that_the_collections_is_returned_when_the_datbase_contains_some()
            {
                Given(the_storage_has_been_created).
                    And(there_is_a_database_with_collections).
                    And(the_controller_has_been_created).
                    And(we_are_listening_for_refresh_event);
                When(calling_refresh_datasources);
                Then("settingsViewModel should contain collections", () =>
                                                                         {
                                                                            var db = storageReader.GetDatabases();
                                                                            db.Count.ShouldBe(1);
                                                                            storageReader.GetCollectionsInDatabase(db[0]).Count.ShouldBe(2);
                                                                            controller.AddDatabasesToSettingsViewModel(db);
                                                                            controller.AddCollectionsToSettingsViewModel(db[0]);
                                                                            controller.settingsViewModel.Databases.Count.ShouldBe(1);
                                                                            controller.settingsViewModel.Databases[0].Name.ShouldBe("TestDatabase");
                                                                            
                                                                            controller.databaseViewModel.Collections.Count.ShouldBe(2);

                                                                            controller.databaseViewModel.Collections[0].Name.ShouldBe("TestCollection1");
                                                                            controller.databaseViewModel.Collections[1].Name.ShouldBe("TestCollection2");
                                                                        });
            }

            [Test]
            public void Then_assure_that_the_collections_is_returned_when_the_datbase_contains_some_when_refresh_is_not_called_in_the_test()
            {
                Given(the_storage_has_been_created).
                    And(there_is_a_database_with_collections).
                    And(the_controller_has_been_created);
                When("");
                Then("settingsViewModel should contain collections", () =>
                {
                    var db = storageReader.GetDatabases();
                    db.Count.ShouldBe(1);
                    storageReader.GetCollectionsInDatabase(db[0]).Count.ShouldBe(2);
                    controller.AddDatabasesToSettingsViewModel(db);
                    controller.AddCollectionsToSettingsViewModel(db[0]);
                    controller.settingsViewModel.Databases.Count.ShouldBe(1);
                    controller.settingsViewModel.Databases[0].Name.ShouldBe("TestDatabase");

                    controller.databaseViewModel.Collections.Count.ShouldBe(2);

                    controller.databaseViewModel.Collections[0].Name.ShouldBe("TestCollection1");
                    controller.databaseViewModel.Collections[1].Name.ShouldBe("TestCollection2");
                });
            }*/
        }


        public class Shared : ScenarioClass
        {
            protected static ChartController controller;
            protected static ChartViewModel viewModel;
            protected static ChartSettingsViewModel settingsViewModel;
            protected static DatabaseViewModel databaseViewModel;

            protected static Configuration configuration;
            protected static ChartConfig chartConfig;

            protected static Mock<ITimer> timerFake;
            protected static Mock<IUIInvoker> uIInvokerFake;
            protected static Mock<IProgressbar> loadingNotifierFake;
            protected static Mock<IChartStorageReader> storageReaderFake;

            protected static int refresh_event_raised = 0;

            protected static void RefreshEventRaised(object sender, EventArgs args)
            {
                refresh_event_raised++;
            }


            protected Context the_controller_has_been_created =
                () => controller = new ChartController(viewModel, settingsViewModel, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object, storageReaderFake.Object, configuration);
           
            protected Context onNotifiedToRefresh_has_been_called = () => timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty);

            protected static Context there_are_no_databases = () => storageReaderFake.Setup(s => s.GetDatabases()).Returns(new List<string>());

            protected static Context there_is_one_database =
                () => storageReaderFake.Setup(s => s.GetDatabases()).Returns(new List<string> {"TestDatabase"});

            protected Context there_are_two_databases =
                () =>
                storageReaderFake.Setup(s => s.GetDatabases()).Returns(new List<string>
                                                                           {"TestDatabase1", "TestDatabase2"});

            protected When onNotifiedToRefresh_is_called = () => timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty);




            [SetUp]
            public void Setup()
            {
                Scenario("");
                viewModel = new ChartViewModel();
                settingsViewModel = new ChartSettingsViewModel();
                databaseViewModel = new DatabaseViewModel();
                databaseViewModel.Collections=new ObservableCollection<CollectionViewModel>();

                timerFake = new Mock<ITimer>();
                uIInvokerFake = new Mock<IUIInvoker>();
                loadingNotifierFake = new Mock<IProgressbar>();
                
                storageReaderFake = new Mock<IChartStorageReader>();
                storageReaderFake.Setup(s => s.RefreshDatasources()).Raises(s => s.DatasourcesRefreshed += null, EventArgs.Empty);
                there_are_no_databases(); // defaults to no databases

                configuration = new Configuration();

            }


            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }
    }
}
