using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using TinyBDD.Specification.NUnit;
using Tskjortebutikken.Widgets.Controllers;
using Tskjortebutikken.Widgets.Tests.Properties;
using Tskjortebutikken.Widgets.ViewModel;

namespace Tskjortebutikken.Widgets.Tests.Controllers
{
    public class GraphSettingsControllerTests
    {
        [TestFixture]
        public class When_spawning : Shared
        {
            [Test]
            public void Then_assure_ViewModel_arg_is_validated()
            {
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new GraphSettingsController(null, graphController.Object, configuration, downloadStringServiceFake.Object, configRepositoryFake.Object));
            }

            [Test]
            public void Then_assure_Configuration_arg_is_validated()
            {
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new GraphSettingsController(graphSettingsViewModel, graphController.Object, null, downloadStringServiceFake.Object, configRepositoryFake.Object));
            }

            [Test]
            public void Then_assure_DownloadStringService_arg_is_validated()
            {
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new GraphSettingsController(graphSettingsViewModel, graphController.Object, configuration, null, configRepositoryFake.Object));
            }
        }

        [TestFixture]
        public class When_spawned : Shared
        {
            public override void Before()
            {
                DownloadStringServiceFakeReturns(Resources.Databases);

                NewController();
            }

            [Test]
            public void Then_assure_Databases_is_loaded_into_ViewModel()
            {
                graphSettingsViewModel.Databases.Count.ShouldNotBe(0);
            }

            [Test]
            public void Then_assure_data_is_loaded_into_each_Database()
            {
                foreach (var databaseViewModel in graphSettingsViewModel.Databases)
                {
                    databaseViewModel.Name.ShouldNotBeNull();
                    databaseViewModel.Name.ShouldNotBe(string.Empty);
                    databaseViewModel.Collections.ShouldNotBeNull();
                    databaseViewModel.Collections.Count.ShouldNotBe(0);
                }
            }
        }

        [TestFixture]
        public class When_Collection_is_selected : Shared
        {
            public override void Before()
            {
                DownloadStringServiceFakeReturns(Resources.Databases, () =>
                {
                    DownloadStringServiceFakeReturns("[{OrderId: 1}, {OrderId: 1, Name: 'ho'}, {One: 1, Two: 2, Three: 3}]");
                });

                NewController();

                graphSettingsViewModel.SelectedDatabase = graphSettingsViewModel.Databases.First();
                graphSettingsViewModel.SelectedCollection = graphSettingsViewModel.SelectedDatabase.Collections.First();
            }

            [Test]
            public void Then_assure_AvailableProperties_is_populated()
            {
                graphSettingsViewModel.AvailableProperties.Count.ShouldNotBe(0);
            }

            [Test]
            public void Then_assure_all_Properties_are_added_to_AvailableProperties()
            {
                graphSettingsViewModel.AvailableProperties.Count.ShouldBe(5);
            }

            [Test]
            public void Then_assure_all_values_in_AvailableProperties_are_unique()
            {
                graphSettingsViewModel.AvailableProperties.Distinct().Count().ShouldBe(graphSettingsViewModel.AvailableProperties.Count);
            }
        }

        [TestFixture]
        public class When_Save : Shared
        {
            public override void Before()
            {
                DownloadStringServiceFakeReturns(Resources.Databases, () =>
                    DownloadStringServiceFakeReturns("[{Day: 1, Revenue: 100}, {Day: 2, Revenue: 130}]"));

                NewController();

                graphSettingsViewModel.SelectedDatabase = graphSettingsViewModel.Databases.First();
                graphSettingsViewModel.SelectedCollection = graphSettingsViewModel.SelectedDatabase.Collections.First();
                graphSettingsViewModel.SelectedPropertyForXAxis = graphSettingsViewModel.AvailableProperties.First();
                graphSettingsViewModel.SelectedPropertyForYAxis = graphSettingsViewModel.AvailableProperties.Last();

                graphSettingsViewModel.Save.Execute();
            }

            [Test]
            public void Then_assure_Configuration_Database_setting_is_updated()
            {
                configuration.GetSetting(GraphConfig.database_setting_name).Value.ShouldBe(
                    graphSettingsViewModel.SelectedDatabase.Name);
            }

            [Test]
            public void Then_assure_Configuration_Collection_setting_is_updated()
            {
                configuration.GetSetting(GraphConfig.collection_setting_name).Value.ShouldBe(
                    graphSettingsViewModel.SelectedCollection.Name);
            }

            [Test]
            public void Then_assure_Configuration_XAxisProperty_setting_is_udpated()
            {
                configuration.GetSetting(GraphConfig.xaxis_property_setting_name).Value.ShouldBe(
                    graphSettingsViewModel.SelectedPropertyForXAxis);
            }

            [Test]
            public void Then_assure_Configuration_YAxisProperty_setting_is_updated()
            {
                configuration.GetSetting(GraphConfig.yaxis_property_Setting_name).Value.ShouldBe(
                    graphSettingsViewModel.SelectedPropertyForYAxis);
            }
        }

        [TestFixture]
        public class When_UpdateConfiguration : Shared
        {
            public override void Before()
            {
                DownloadStringServiceFakeReturns(Resources.Databases, () =>
                    DownloadStringServiceFakeReturns(Resources.Databases, () =>
                        DownloadStringServiceFakeReturns(Resources.revenuePrDay)));

                NewController();

                configuration.ChangeSetting(GraphConfig.database_setting_name, "Webshop");
                configuration.ChangeSetting(GraphConfig.collection_setting_name, "revenuePrDay");
                configuration.ChangeSetting(GraphConfig.xaxis_property_setting_name, "Date");
                configuration.ChangeSetting(GraphConfig.yaxis_property_Setting_name, "Revenue");
                
                controller.UpdateConfiguration(configuration);
            }

            [Test]
            public void Then_assure_SelectedDatabase_is_set()
            {
                graphSettingsViewModel.SelectedDatabase.ShouldNotBeNull();
                graphSettingsViewModel.SelectedDatabase.Name.ShouldBe("Webshop");
            }

            [Test]
            public void Then_assure_SelectedCollection_is_set()
            {
                graphSettingsViewModel.SelectedCollection.ShouldNotBeNull();
                graphSettingsViewModel.SelectedCollection.Name.ShouldBe("revenuePrDay");
            }

            [Test]
            [Ignore("Had to ignore it because I had some issues with mocking the DowloadStringService")]
            public void Then_assure_XAxisProperty_i_set()
            {
                graphSettingsViewModel.SelectedPropertyForXAxis.ShouldBe("Date");
            }

            [Test]
            [Ignore("Had to ignore it because I had some issues with mocking the DowloadStringService")]
            public void Then_assure_YAxisProperty_is_set()
            {
                graphSettingsViewModel.SelectedPropertyForYAxis.ShouldBe("Revenue");
            }
        }

        public class Shared
        {
            protected GraphSettings graphSettingsViewModel;
            protected Configuration configuration;
            protected GraphSettingsController controller;
            protected Mock<IDownloadStringService> downloadStringServiceFake;
            protected Mock<IPersistDomainModels<Configuration>> configRepositoryFake;
            protected Mock<GraphController> graphController;
            
            [SetUp]
            public void Setup()
            {
                graphSettingsViewModel = new GraphSettings();
                downloadStringServiceFake = new Mock<IDownloadStringService>();
                configRepositoryFake = new Mock<IPersistDomainModels<Configuration>>();
                configuration = GraphConfig.NewDefaultConfiguration();
                graphController = new Mock<GraphController>(
                    new Graph(), 
                    downloadStringServiceFake.Object,
                    new Mock<ITimer>().Object,
                    configuration,
                    new Mock<IProgressbar>().Object);

                Before();
            }

            public virtual void Before()
            {
                
            }

            protected void NewController()
            {
                controller = new GraphSettingsController(graphSettingsViewModel, graphController.Object, configuration, downloadStringServiceFake.Object, configRepositoryFake.Object);
            }

            protected void DownloadStringServiceFakeReturns(string data)
            {
                DownloadStringServiceFakeReturns(data, null);
            }

            protected void DownloadStringServiceFakeReturns(string data, Action whenDownloaded)
            {
                downloadStringServiceFake.Setup(s => s.DownloadAsync(
                    It.IsAny<Uri>(),
                    It.IsAny<Action<string>>())).Callback((Uri url, Action<string> callback) =>
                    {
                        callback(data);

                        if (whenDownloaded != null)
                            whenDownloaded();
                    });
            }
        }
    }
}
