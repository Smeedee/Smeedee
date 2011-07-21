﻿using System;
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
                When("creating new controller with null as viewModel", () => viewModel = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateController));
            }

            [Test]
            public void Then_null_timerArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as timer", () => timerFake = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateController));
            }

            [Test]
            public void Then_null_uIInvokerArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as uIInvoker", () => uIInvoker = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateController));
            }

            [Test]
            public void Then_null_loadingNotifierArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as loadingNotifier", () => loadingNotifierFake = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateController));
            }

            [Test]
            public void Then_null_storageReaderArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as storageReader", () => storageReaderFake = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateController));
            }

            [Test]
            public void Then_null_configPersisterArgs_should_return_exception()
            {
                Given("");
                When("creating new controller with null as configuration", () => configuration = null);
                When("creating new controller with null as configPersister", () => configPersisterFake = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateController));
            }

            [Test]
            public void Then_null_configuration_should_return_exception()
            {
                Given("");
                When("creating new controller with null as configRepo", () => configPersisterFake = null);
                When("creating new controller with null as configuration", () => configuration = null);
                Then("an ArgumentNullException should be thrown",
                     () =>
                     this.ShouldThrowException<ArgumentNullException>(CreateController));
            }

            [Test]
            [Ignore]
            public void Then_assure_Configuration_contains_Database_setting()
            {
                Given("a new configuration is made", () => configuration = new Configuration());
                When("no database setting is entered", () => configuration.NewSetting(ChartConfig.chart_setting_name, ""));
                Then("", () =>
                         this.ShouldThrowException<ArgumentException>(CreateController, ex =>
                        ex.Message.ShouldBe("Config setting is missing; " + ChartConfig.chart_setting_name)));
            }

            [Test]
            [Ignore]
            public void Then_assure_complete_configuration_does_not_throw_exception()
            {
                Given("a new configuration is made", () => configuration = new Configuration());
                When("settings are entered", () =>
                                                 {
                                                     configuration.NewSetting(ChartConfig.chart_setting_name, "cName");
                                                     configuration.NewSetting(ChartConfig.x_axis_setting_name,"xName");
                                                     configuration.NewSetting(ChartConfig.y_axis_setting_name, "yName");
                                                     configuration.NewSetting(ChartConfig.x_axis_setting_type, "xType");
                                                 });
                Then("no exceptions should be thrown", CreateController);
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
                When("finished loading");
                Then("there should not be any databases in viewmodel", () => 
                    controller.SettingsViewModel.Databases.Count.ShouldBe(0)
                    );
            }

            [Test]
            public void Then_assure_that_one_database_is_returned_when_there_is_only_one()
            {
                Given(there_is_one_database_TestDatabase).
                    And(the_controller_has_been_created);
                When("finished loading");
                Then("settingsViewModel should contain a database", () =>
                {
                    settingsViewModel.Databases.Count.ShouldBe(1);
                    settingsViewModel.Databases[0].ShouldBe("TestDatabase");
                });
            }

            [Test]
            public void Then_assure_that_two_databases_are_returned_when_there_are_two()
            {
                Given(there_are_two_databases_TestDatabase1_TestDatabase2).
                    And(the_controller_has_been_created);
                When("finished loading");
                Then("settingsViewModel should contain two databases", () =>
                {
                    settingsViewModel.Databases.Count.ShouldBe(2);
                    settingsViewModel.Databases[0].ShouldBe("TestDatabase1");
                    settingsViewModel.Databases[1].ShouldBe("TestDatabase2");
                });
            }

            [Test]
            public void Then_assure_that_two_databases_are_returned_when_there_are_two_and_refresh_is_called()
            {
                Given(there_are_two_databases_TestDatabase1_TestDatabase2).
                    And(the_controller_has_been_created);
                When(refresh_is_called);
                Then("settingsViewModel should contain two databases", () =>
                {
                    settingsViewModel.Databases.Count.ShouldBe(2);
                    settingsViewModel.Databases[0].ShouldBe("TestDatabase1");
                    settingsViewModel.Databases[1].ShouldBe("TestDatabase2");
                });
            }

            [Test]
            public void Then_assure_that_there_are_no_collections_in_viewmodel_when_there_is_none_in_storage()
            {
                Given(there_are_no_collections).
                    And(the_controller_has_been_created);
                When("finished loading");
                Then("settingsViewModel should not contain any collections", () => settingsViewModel.Collections.Count.ShouldBe(0));
            }

            [Test]
            public void Then_assure_that_there_are_no_collections_in_viewmodel_when_there_is_none_in_storage_but_database_is_selected()
            {
                Given(there_are_no_collections).
                    And(there_is_one_database_TestDatabase).
                    And(the_controller_has_been_created);
                When(testdatabase_is_selected);
                Then("settingsViewModel should not contain any collections", () => settingsViewModel.Collections.Count.ShouldBe(0));
            }

            [Test]
            public void Then_assure_that_one_collection_is_returned_when_the_database_contains_one()
            {
                Given(there_is_one_database_TestDatabase).
                    And(there_is_a_collection_in_TestDatabase).
                    And(the_controller_has_been_created);
                When(testdatabase_is_selected);
                Then("settingsViewModel should contain the collection of the first database",
                     () =>
                     {
                         settingsViewModel.Collections.Count.ShouldBe(1);
                         settingsViewModel.Collections[0].ShouldBe("TestCollection");
                     });
            }

            [Test]
            public void Then_assure_that_two_collections_are_returned_when_the_database_contains_two()
            {
                Given(there_is_one_database_TestDatabase).
                    And(there_are_two_collections_in_TestDatabase).
                    And(the_controller_has_been_created);
                When(testdatabase_is_selected);
                Then("settingsViewModel should contain the two collections of the first database",
                     () =>
                     {
                         settingsViewModel.Collections.Count.ShouldBe(2);
                         settingsViewModel.Collections[0].ShouldBe("TestCollection1");
                         settingsViewModel.Collections[1].ShouldBe("TestCollection2");
                     });
            }

            [Test]
            public void Then_assure_that_if_two_databases_and_several_collections_the_collections_of_the_selected_database_is_returned()
            {
                Given(there_are_two_databases_TestDatabase1_TestDatabase2).
                    And(TestDatabase1_has_collections).
                    And(TestDatabase2_has_collections).
                    And(the_controller_has_been_created);
                When(testdatabase2_is_selected);
                Then("settingsViewModel should contain a collection",
                     () =>
                        {
                             settingsViewModel.Databases.Count.ShouldBe(2);
                             settingsViewModel.Collections.Count.ShouldBe(3);
                             settingsViewModel.Collections[0].ShouldBe("TestCollection3");
                         });
            }
        }

        [TestFixture]
        public class When_AddDataSettings_is_called : Shared
        {
            private Context SelectedDatabase_is_null = () => controller.SettingsViewModel.SelectedDatabase=null;
            private Context SelectedCollection_is_null = () => controller.SettingsViewModel.SelectedCollection = null;
            
            private Context selected_database_is_charting = () => controller.SettingsViewModel.SelectedDatabase = "Charting";
            private Context selected_collection_is_collection = () => controller.SettingsViewModel.SelectedCollection = "Collection";
            private Context selected_collection_contains_one_empty_dataset = () => StorageReaderLoadChartReturns(new Chart("Charting", "Collection") { DataSets = { new DataSet { Name = "DataSet" } } });

            private Context selected_collection_contains_two_empty_datasets = () => StorageReaderLoadChartReturns(new Chart("Charting", "Collection") { DataSets = { new DataSet { Name = "DataSet1" } , new DataSet { Name = "DataSet2" } } });

            private Context seriesconfig_already_has_a_dataset = () => controller.SettingsViewModel.SeriesConfig.Add(new SeriesConfigViewModel { Database = "Old", Collection = "Collection", Name = "OldName" , Legend = "OldName"});

            private When AddDataSettings_is_pressed = () => controller.SettingsViewModel.AddDataSettings.ExecuteDelegate();


            [Test]
            public void Assure_nothing_happens_if_selectedDatabase_and_selectedCollection_is_empty()
            {
                Given(the_controller_has_been_created).
                    And(SelectedCollection_is_null);
                When(AddDataSettings_is_pressed);
                Then("nothing happens", () => controller.SettingsViewModel.SeriesConfig.Count.ShouldBe(0));
            }

            [Test]
            public void Assure_that_adding_a_collection_with_one_dataset_will_add_one_dataset()
            {
                Given(the_controller_has_been_created).
                    And(selected_database_is_charting).
                    And(selected_collection_is_collection).
                    And(selected_collection_contains_one_empty_dataset);
                When(AddDataSettings_is_pressed);
                Then("SeriesConfig should contain one dataset", () => controller.SettingsViewModel.SeriesConfig.Count.ShouldBe(1));
            }

            [Test]
            public void Assure_that_adding_a_dataset_will_add_correct_dataset()
            {
                Given(the_controller_has_been_created).
                    And(selected_database_is_charting).
                    And(selected_collection_is_collection).
                    And(selected_collection_contains_one_empty_dataset);
                When(AddDataSettings_is_pressed);
                Then("SeriesConfig should contain one dataset", () =>
                                                                    {
                                                                        controller.SettingsViewModel.SeriesConfig[0].Database.ShouldBe("Charting");
                                                                        controller.SettingsViewModel.SeriesConfig[0].Collection.ShouldBe("Collection");
                                                                        controller.SettingsViewModel.SeriesConfig[0].Name.ShouldBe("DataSet");
                                                                    });
            }

            [Test]
            public void Assure_that_adding_two_datasets_will_add_correct_datasets()
            {
                Given(the_controller_has_been_created).
                    And(selected_database_is_charting).
                    And(selected_collection_is_collection).
                    And(selected_collection_contains_two_empty_datasets);
                When(AddDataSettings_is_pressed);
                Then("SeriesConfig should contain two dataset", () =>
                {
                    controller.SettingsViewModel.SeriesConfig.Count.ShouldBe(2);

                    controller.SettingsViewModel.SeriesConfig[0].Database.ShouldBe("Charting");
                    controller.SettingsViewModel.SeriesConfig[0].Collection.ShouldBe("Collection");
                    controller.SettingsViewModel.SeriesConfig[0].Name.ShouldBe("DataSet1");

                    controller.SettingsViewModel.SeriesConfig[1].Database.ShouldBe("Charting");
                    controller.SettingsViewModel.SeriesConfig[1].Collection.ShouldBe("Collection");
                    controller.SettingsViewModel.SeriesConfig[1].Name.ShouldBe("DataSet2");
                });
            }

            [Test]
            public void Assure_that_adding_a_dataset_when_one_already_exists_results_in_two_datasets()
            {
                Given(the_controller_has_been_created).
                    And(seriesconfig_already_has_a_dataset).
                    And(selected_database_is_charting).
                    And(selected_collection_is_collection).
                    And(selected_collection_contains_one_empty_dataset);
                When(AddDataSettings_is_pressed);
                Then("SeriesConfig should contain two dataset", () =>
                {
                    controller.SettingsViewModel.SeriesConfig.Count.ShouldBe(2);

                    controller.SettingsViewModel.SeriesConfig[0].Database.ShouldBe("Old");
                    controller.SettingsViewModel.SeriesConfig[0].Collection.ShouldBe("Collection");
                    controller.SettingsViewModel.SeriesConfig[0].Name.ShouldBe("OldName");

                    controller.SettingsViewModel.SeriesConfig[1].Database.ShouldBe("Charting");
                    controller.SettingsViewModel.SeriesConfig[1].Collection.ShouldBe("Collection");
                    controller.SettingsViewModel.SeriesConfig[1].Name.ShouldBe("DataSet");
                });
            }

            [Test]
            public void Assure_that_default_legend_is_set_to_null()
            {
                Given(the_controller_has_been_created).
                    And(selected_database_is_charting).
                    And(selected_collection_is_collection).
                    And(selected_collection_contains_one_empty_dataset);
                When(AddDataSettings_is_pressed);
                Then("default legend should be null",
                     () => controller.SettingsViewModel.SeriesConfig[0].Legend.ShouldBe(null));
            }

        }

        [TestFixture]
        public class When_saving_configuration : Shared
        {
            [Test]
            public void Then_series_settings_from_viewmodel_should_be_copied_into_configuration()
            {
                Given(the_controller_has_been_created).
                    And(there_is_a_series_configured_in_viewmodel);
                When(SaveSettings_is_executed);
                Then("configuration should contain the given series", () =>
                    {
                        var series = controller.ChartConfig.GetSeries();
                        series.Count.ShouldBe(1);
                        series[0].Name.ShouldBe("series1");
                    });
            }

            [Test]
            public void Then_two_series_settings_from_viewmodel_should_be_copied_into_configuration()
            {
                Given(the_controller_has_been_created).
                    And(there_is_a_series_configured_in_viewmodel).
                    And(there_is_another_series_configured_in_viewmodel);
                When(SaveSettings_is_executed);
                Then("configuration should contain the given series", () =>
                {
                    var series = controller.ChartConfig.GetSeries();
                    series.Count.ShouldBe(2);
                    series[0].Name.ShouldBe("series1");
                    series[1].Name.ShouldBe("series2");
                });
            }

            [Test]
            public void Then_string_settings_from_viewmodel_should_be_copied_into_configuration()
            {
                Given(the_controller_has_been_created).
                    And(there_is_settings_in_viewmodel);
                When(SaveSettings_is_executed);
                Then("configuration should contain string settings", () =>
                    {
                        controller.ChartConfig.ChartName.ShouldBe("TestName");
                        controller.ChartConfig.XAxisName.ShouldBe("X-Axis");
                        controller.ChartConfig.YAxisName.ShouldBe("Y-Axis");
                        controller.ChartConfig.XAxisType.ShouldBe("Linear");
                    });
            }

            [Test]
            public void Then_save_should_be_called_on_on_configPersister()
            {
                Given(the_controller_has_been_created);
                When(SaveSettings_is_executed);
                Then("save should be called on the configPersister",
                     () => configPersisterFake.Verify(c => c.Save(It.IsAny<Configuration>()), Times.AtLeastOnce()));
            }


            private When SaveSettings_is_executed = () => controller.SettingsViewModel.SaveSettings.ExecuteDelegate();

            private Context there_is_a_series_configured_in_viewmodel = () =>
                    controller.SettingsViewModel.SeriesConfig.Add(
                        new SeriesConfigViewModel
                        {
                            Name = "series1",
                            Database = "db",
                            Collection = "col",
                            Action = "show",
                            ChartType = "Lines",
                            Legend = ""
                        });

            private Context there_is_another_series_configured_in_viewmodel = () =>
                    controller.SettingsViewModel.SeriesConfig.Add(
                        new SeriesConfigViewModel
                        {
                            Name = "series2",
                            Database = "db2",
                            Collection = "col2",
                            Action = "hide",
                            ChartType = "Area",
                            Legend = "legend"
                        });

            private Context there_is_settings_in_viewmodel = () =>
                                                                 {
                                                                     var vm = controller.SettingsViewModel;
                                                                     vm.ChartName = "TestName";
                                                                     vm.XAxisName = "X-Axis";
                                                                     vm.YAxisName = "Y-Axis";
                                                                     vm.XAxisType = "Linear";
                                                                 };
        }

        [TestFixture]
        public class When_updating_configuration : Shared
        {
            [Test]
            public void Then_string_settings_should_be_copied_to_viewmodel()
            {
                Given(the_controller_has_been_created);
                When(update_configuration_is_called);
                Then("string values should be in settingsview", () =>
                    {
                        controller.SettingsViewModel.ChartName.ShouldBe("AName");
                        controller.SettingsViewModel.XAxisName.ShouldBe("AnX");
                        controller.SettingsViewModel.YAxisName.ShouldBe("AnY");
                        controller.SettingsViewModel.XAxisType.ShouldBe("Category");
                    });
            }

            [Test]
            public void Then_series_settings_should_be_copied_to_settings_viewmodel()
            {
                Given(the_controller_has_been_created);
                When(update_configuration_is_called);
                Then("series settings should be in settingsview", () =>
                    {
                        controller.SettingsViewModel.SeriesConfig.Count.ShouldBe(1);
                        controller.SettingsViewModel.SeriesConfig[0].Name.ShouldBe("ASeries");
                    });
            }

            private When update_configuration_is_called = () => controller.UpdateConfiguration(AConfiguration());

            private static Configuration AConfiguration()
            {
                var chartConf = new ChartConfig(ChartConfig.NewDefaultConfiguration());
                chartConf.ChartName = "AName";
                chartConf.XAxisName = "AnX";
                chartConf.YAxisName = "AnY";
                chartConf.XAxisType = "Category";
                chartConf.SetSeries(new Collection<SeriesConfigViewModel> { new SeriesConfigViewModel {Name="ASeries"} });
                return chartConf.Configuration;
            }
        }

        public class Shared : ScenarioClass
        {
            protected static ChartController controller;
            protected static ChartViewModel viewModel;
            protected static ChartSettingsViewModel settingsViewModel;

            protected static IUIInvoker uIInvoker;
            protected static Mock<ITimer> timerFake;
            protected static Mock<IProgressbar> loadingNotifierFake;
            protected static Mock<IChartStorageReader> storageReaderFake;
            protected static Mock<IPersistDomainModelsAsync<Configuration>> configPersisterFake;
            protected static Configuration configuration;

            protected static ITimer GetTimerObject()
            {
                return timerFake != null ? timerFake.Object : null; 
            }

            protected static IProgressbar GetLoadingNotifier()
            {
                return loadingNotifierFake != null ? loadingNotifierFake.Object : null;
            }

            protected static IChartStorageReader GetStorageReader()
            {
                return storageReaderFake != null ? storageReaderFake.Object : null;
            }

            protected static IPersistDomainModelsAsync<Configuration> GetConfigPersister()
            {
                return configPersisterFake != null ? configPersisterFake.Object : null;
            }

          

            protected Context the_controller_has_been_created = CreateController;
            protected When controller_is_created = CreateController;
            
            protected static Context there_are_no_databases = () => storageReaderFake.Setup(s => s.GetDatabases()).Returns(new List<string>());
            protected static Context there_is_one_database_TestDatabase =
                () => storageReaderFake.Setup(s => s.GetDatabases()).Returns(new List<string> {"TestDatabase"});
            protected Context there_are_two_databases_TestDatabase1_TestDatabase2 =
                () => storageReaderFake.Setup(s => s.GetDatabases()).Returns(new List<string>
                                                                           {"TestDatabase1", "TestDatabase2"});

            protected Context there_are_no_collections =
                () => storageReaderFake.Setup(s => s.GetCollectionsInDatabase(It.Is<string>(t => t == "TestDatabase"))).Returns(new List<string>());
            protected Context there_is_a_collection_in_TestDatabase =
                () => storageReaderFake.Setup(s => s.GetCollectionsInDatabase(It.Is<string>(t => t == "TestDatabase"))).Returns(new List<string> { "TestCollection" });
            protected Context there_are_two_collections_in_TestDatabase =
                () => storageReaderFake.Setup(s => s.GetCollectionsInDatabase(It.Is<string>(t => t == "TestDatabase"))).Returns(new List<string>
                                                                           {"TestCollection1", "TestCollection2" });

            protected Context the_selectedDatabase_is_TestDatabase2 = () => controller.SettingsViewModel.SelectedDatabase = "TestDatabase2";
            protected Context the_selected_database_is_TestDatabase = () => controller.SettingsViewModel.SelectedDatabase = "TestDatabase";

            protected Context TestDatabase1_has_collections =
               () => storageReaderFake.Setup(s => s.GetCollectionsInDatabase(It.Is<string>(t => t == "TestDatabase1"))).Returns(new List<string> { "TestCollection1", "TestCollection2" });
            protected Context TestDatabase2_has_collections =
              () => storageReaderFake.Setup(s => s.GetCollectionsInDatabase(It.Is<string>(t => t == "TestDatabase2"))).Returns(new List<string> { "TestCollection3", "TestCollection4", "TestCollection5" });

            protected Context onNotifiedToRefresh_has_been_called = () => timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty);

            protected When onNotifiedToRefresh_is_called = () => timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty);
            protected When refresh_is_called = () => timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty);

            protected When testdatabase_is_selected = () => controller.SettingsViewModel.SelectedDatabase = "TestDatabase";
            protected When testdatabase2_is_selected = () => controller.SettingsViewModel.SelectedDatabase = "TestDatabase2";

            protected static void CreateController()
            {
                controller = new ChartController
                    (viewModel, settingsViewModel, GetTimerObject(), uIInvoker, GetLoadingNotifier(), GetStorageReader(), configuration, GetConfigPersister());
            }

            protected static void StorageReaderLoadChartReturns(Chart chart)
            {
                storageReaderFake.Setup(s => s.LoadChart(It.IsAny<string>(), It.IsAny<string>())).Raises(
                    t => t.ChartLoaded += null, new ChartLoadedEventArgs(chart));
            }

            [SetUp]
            public void Setup()
            {
                Scenario("");
                viewModel = new ChartViewModel();
                settingsViewModel = new ChartSettingsViewModel();
              

                timerFake = new Mock<ITimer>();
                uIInvoker = new NoUIInvokation();
                loadingNotifierFake = new Mock<IProgressbar>();
                
                storageReaderFake = new Mock<IChartStorageReader>();
                storageReaderFake.Setup(s => s.RefreshDatasources()).Raises(s => s.DatasourcesRefreshed += null, EventArgs.Empty);
                there_are_no_databases(); // defaults to no databases
                there_are_no_collections();

                configPersisterFake = new Mock<IPersistDomainModelsAsync<Configuration>>();
                configuration = ChartConfig.NewDefaultConfiguration();

            }


            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }
    }
}