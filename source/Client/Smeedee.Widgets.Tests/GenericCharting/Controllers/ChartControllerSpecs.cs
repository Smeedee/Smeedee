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
                        () => new ChartController(null, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object, storageReader, configuration)));
            }

            [Test]
            public void Then_null_timerArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as timer");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, null, uIInvokerFake.Object, loadingNotifierFake.Object, storageReader, configuration)));
            }

            [Test]
            public void Then_null_uIInvokerArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as uIInvoker");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, timerFake.Object, null, loadingNotifierFake.Object, storageReader, configuration)));
            }

            [Test]
            public void Then_null_loadingNotifierArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as loadingNotifier");
                Then("an ArgumentException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentException>(
                        () => new ChartController(chartViewModel, timerFake.Object, uIInvokerFake.Object, null, storageReader, configuration)));
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
                                             loadingNotifierFake.Object, storageReader, null)));
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

        [TestFixture]
        public class When_DownloadAndAddDataToViewModel_is_called : Shared
        {


            [Test]
            public void Then_assure_that_one_database_is_returned_when_there_is_only_one()
            {
                Given(the_storage_has_been_created).
                    And(there_is_one_database).
                    And(the_controller_has_been_created).
                    And(we_are_listening_for_refresh_event);
                When(calling_refresh_datasources);
                Then("settingsViewModel should contain a database", () =>
                                                                        {
                                                                            storageReader.GetDatabases().Count.ShouldBe(1);
                                                                            chartController.AddDatabasesToSettingsViewModel(storageReader.GetDatabases());
                                                                            chartController.settingsViewModel.Databases.Count.ShouldBe(1);
                                                                            chartController.settingsViewModel.Databases[0].Name.ShouldBe("TestDatabase");
                                                                        });
            }

            [Test]
            public void Then_assure_that_two_database_is_returned_when_there_is_two()
            {
                Given(the_storage_has_been_created).
                    And(there_are_two_databases).
                    And(the_controller_has_been_created).
                    And(we_are_listening_for_refresh_event);
                When(calling_refresh_datasources);
                Then("settingsViewModel should contain two databases", () =>
                {
                    storageReader.GetDatabases().Count.ShouldBe(2);
                    chartController.AddDatabasesToSettingsViewModel(storageReader.GetDatabases());
                    chartController.settingsViewModel.Databases.Count.ShouldBe(2);
                    chartController.settingsViewModel.Databases[0].Name.ShouldBe("TestDatabase");
                    chartController.settingsViewModel.Databases[1].Name.ShouldBe("TestDatabase2");
                });
            }
        }
        

        public class Shared : ScenarioClass
        {
            protected static ChartController chartController;
            protected static ChartViewModel chartViewModel;

            protected static Configuration configuration;
            protected static ChartConfig chartConfig;
            
            protected static Mock<ITimer> timerFake;
            protected static Mock<IUIInvoker> uIInvokerFake;
            protected static Mock<IProgressbar> loadingNotifierFake;
            protected static ChartStorageReader storageReader;

            protected static Mock<INoSqlRepository> noSqlRepositoryMock;

            protected static int refresh_event_raised = 0;

            protected static void RefreshEventRaised(object sender, EventArgs args)
            {
                refresh_event_raised++;
            }


            protected Context the_controller_has_been_created = 
                () => chartController = new ChartController(chartViewModel, timerFake.Object, uIInvokerFake.Object, loadingNotifierFake.Object, storageReader, configuration);
            protected Context the_storage_has_been_created = () => storageReader = new ChartStorageReader(noSqlRepositoryMock.Object);
            protected Context onNotifiedToRefresh_has_been_called = () => timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty);
            protected Context there_is_one_database = () =>
                NoSqlRepositoryGetDatabasesReturns(new Collection { Documents = { Document.Parse("{Name:\"TestDatabase\", Collections: []}") } });
            protected Context there_are_two_databases = () =>
                NoSqlRepositoryGetDatabasesReturns(new Collection { Documents =
                                                                        {
                                                                            Document.Parse("{Name:\"TestDatabase\", Collections: []}"), 
                                                                            Document.Parse("{Name:\"TestDatabase2\", Collections: []}")
                                                                        } });

            protected When calling_refresh_datasources = () => storageReader.RefreshDatasources();
            protected When onNotifiedToRefresh_is_called = () => timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty);

           


            protected static void NoSqlRepositoryGetDatabasesReturns(Collection databases)
            {
                noSqlRepositoryMock.Setup(s => s.GetDatabases(It.IsAny<Action<Collection>>())).
                    Callback((Action<Collection> callback) => callback(databases));
            }

            protected Context we_are_listening_for_refresh_event = () =>
            {
                refresh_event_raised = 0;
                storageReader.DatasourcesRefreshed += RefreshEventRaised;
            };


            [SetUp]
            public void Setup()
            {
                Scenario("");
                chartViewModel = new ChartViewModel();

                timerFake = new Mock<ITimer>();
                uIInvokerFake = new Mock<IUIInvoker>();
                loadingNotifierFake = new Mock<IProgressbar>();
                noSqlRepositoryMock = new Mock<INoSqlRepository>();
                storageReader = new ChartStorageReader(noSqlRepositoryMock.Object);
                

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
