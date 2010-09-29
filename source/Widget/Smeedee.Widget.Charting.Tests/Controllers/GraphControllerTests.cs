using System;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Config;
using Smeedee.Widget.Charting.Tests.Properties;
using TinyBDD.Specification.NUnit;
using Tskjortebutikken.Widgets.Controllers;
using Tskjortebutikken.Widgets.ViewModel;

namespace Tskjortebutikken.Widgets.Tests.Controllers
{
    public class GraphControllerTests
    {
        [TestFixture]
        public class When_spawning : Shared
        {
            [Test]
            public void Then_assure_Graph_ViewModel_arg_is_validated()
            {
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new GraphController(null, downloadStringServiceFake.Object, timerFake.Object, configuration, progressBarServiceFake.Object));
            }

            [Test]
            public void Then_assure_DownloadStringService_arg_is_validated()
            {
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new GraphController(new Graph(), null, timerFake.Object, configuration, progressBarServiceFake.Object));
            }

            [Test]
            public void Then_assure_Timer_args_is_validated()
            {
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new GraphController(graph, downloadStringServiceFake.Object, null, configuration, progressBarServiceFake.Object));
            }

            [Test]
            public void Then_assure_ProgressbarService_arg_is_validated()
            {
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new GraphController(graph, downloadStringServiceFake.Object, timerFake.Object, configuration, null));
            }

            [Test]
            public void Then_assure_Configuration_args_is_validated()
            {
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new GraphController(graph, downloadStringServiceFake.Object, timerFake.Object, null, progressBarServiceFake.Object));
            }

            [Test]
            public void Then_assure_Configuration_contains_RefreshInterval_setting()
            {
                configuration = new Configuration();
                this.ShouldThrowException<ArgumentException>(() =>
                    NewController());
            }

            [Test]
            public void Then_assure_Configuration_contains_Database_setting()
            {
                configuration = new Configuration();
                configuration.NewSetting(GraphConfig.refrehInterval_setting_name, "0");

                this.ShouldThrowException<ArgumentException>(() =>
                    NewController());
            }

            [Test]
            public void Then_assure_Configuration_contians_Collection_setting()
            {
                configuration = new Configuration();
                configuration.NewSetting(GraphConfig.refrehInterval_setting_name, "0");
                configuration.NewSetting(GraphConfig.database_setting_name);

                this.ShouldThrowException<ArgumentException>(() =>
                    NewController());
            }

            [Test]
            public void Then_assure_Configuration_contains_XAxisPropertyName()
            {
                configuration = new Configuration();
                configuration.NewSetting(GraphConfig.refrehInterval_setting_name, "5");
                configuration.NewSetting(GraphConfig.database_setting_name);
                configuration.NewSetting(GraphConfig.collection_setting_name);

                this.ShouldThrowException<ArgumentException>(() =>
                    NewController(), ex =>
                        ex.Message.ShouldBe("Config setting is missing; " + GraphConfig.xaxis_property_setting_name));
            }

            [Test]
            public void Then_assure_Configuration_contains_YAxisPropertyName()
            {
                configuration = new Configuration();
                configuration.NewSetting(GraphConfig.refrehInterval_setting_name, "5");
                configuration.NewSetting(GraphConfig.database_setting_name);
                configuration.NewSetting(GraphConfig.collection_setting_name);
                configuration.NewSetting(GraphConfig.xaxis_property_setting_name);

                this.ShouldThrowException<ArgumentException>(() =>
                    NewController(), ex =>
                        ex.Message.ShouldBe("Config setting is missing; " + GraphConfig.yaxis_property_Setting_name));
            }
        }

        [TestFixture]
        public class When_spawned : Shared
        {
            protected override void Before()
            {
                DownloadStringServiceFakeReturns(Resources.ordersPrDay);

                NewController();
            }

            [Test]
            public void Then_assure_Timer_is_started()
            {
                timerFake.Verify(t => t.Start(It.IsAny<int>()), Times.Once());
            }

            [Test]
            public void Then_assure_refresh_interval_from_Configuration_is_passed_to_the_Timer()
            {
                timerFake.Verify(t => t.Start(
                    It.Is<int>(v => v == graphConfig.RefreshInteval)), Times.Once());
            }
        }

        [TestFixture]
        public class When_notified_to_refresh_by_Timer : Shared
        {
            private int downloadedDataPoints;

            protected override void Before()
            {
                DownloadStringServiceFakeReturns(Resources.ordersPrDay);

                NewController();
                downloadedDataPoints = graph.Data.Count;
           
                timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty);
            }

            [Test]
            public void Then_assure_data_is_downloaded()
            {
                downloadStringServiceFake.Verify(s =>
                    s.DownloadAsync(It.IsAny<Uri>(), It.IsAny<Action<string>>()), Times.Exactly(2));
            }

            [Test]
            public void Then_assure_data_is_not_accumulated()
            {
                graph.Data.Count.ShouldBe(downloadedDataPoints);
            }
        }

        [TestFixture]
        public class When_stop_fetching_data_in_background : Shared
        {
            [SetUp]
            public void Setup()
            {
                DownloadStringServiceFakeReturns(Resources.ordersPrDay);

                NewController();

                controller.StopFetchingDataInBackground();
            }

            [Test]
            public void Then_assure_background_fetching_is_stopped()
            {
                timerFake.Verify(t => t.Stop());
            }
        }

        [TestFixture]
        public class When_start_fetching_data_in_background : Shared
        {
            [SetUp]
            public void Setup()
            {
                DownloadStringServiceFakeReturns(Resources.ordersPrDay);

                NewController();
                controller.StopFetchingDataInBackground();

                controller.StartFetchingDataInBackground();
            }

            [Test]
            public void Then_assure_background_fetching_is_started()
            {
                timerFake.Verify(t => t.Start(It.IsAny<int>()), Times.Exactly(2));
            }
        }

        [TestFixture]
        public class When_downloading_data : Shared
        {
            protected override void Before()
            {
                DownloadStringServiceFakeReturns(Resources.ordersPrDay);

                NewController();
            }

            [Test]
            public void Then_assure_Database_and_Collection_config_setting_is_set_as_args_in_url()
            {
                downloadStringServiceFake.Verify(s => 
                    s.DownloadAsync(
                        It.Is<Uri>(url => url.ToString() == string.Format(GraphController.relativeUrl, 
                            "Webshop",
                            "ordersPrDay")),
                        It.IsAny<Action<string>>()));
            }
        }

        [TestFixture]
        public class When_data_is_downloaded : Shared
        {
            protected override void Before()
            {
                DownloadStringServiceFakeReturns(Resources.ordersPrDay);

                NewController();

                timerFake.Raise(t => t.Elapsed += null, EventArgs.Empty);
            }

            [Test]
            public void Then_assure_DataPoints_is_loaded_into_ViewModel()
            {
                graph.Data.Count.ShouldNotBe(0);
            }

            [Test]
            public void Then_assure_DataPoints_contains_data()
            {
                foreach (var dataPoint in graph.Data)
                {
                    dataPoint.X.ShouldNotBeNull();
                    dataPoint.X.ShouldNotBe(DateTime.MinValue);
                    dataPoint.Y.ShouldNotBeNull();
                    dataPoint.Y.ShouldNotBe(0);
                }
            }

            [Test]
            public void Then_assure_number_of_DataPoints_dont_exceed_MaxNumberOfDataPoints_setting()
            {
                graph.Data.Count.ShouldBe(graphConfig.MaxNumberOfDataPoints);
            }

            [Test]
            public void Then_assure_Progressbar_is_displayed_while_data_is_downloading()
            {
                progressBarServiceFake.Verify(s => s.ShowInView(It.IsAny<string>()));
            }

            [Test]
            public void Then_assure_progressbar_is_hidden()
            {
                progressBarServiceFake.Verify(s => s.HideInView());
            }
        }

        [TestFixture]
        public class When_Configuration_is_changed : Shared
        {
            protected override void Before()
            {
                DownloadStringServiceFakeReturns(Resources.ordersPrDay, () =>
                    DownloadStringServiceFakeReturns(Resources.revenuePrDay));

                NewController();

                graphConfig.Collection = "revenuePrDay";
                graphConfig.XAxisProperty = "Date";
                graphConfig.YAxisProperty = "Revenue";
                graphConfig.IsConfigured = true;
                controller.UpdateConfiguration(graphConfig.Configuration);
            }

            [Test]
            public void Then_assure_new_data_is_downloaded()
            {
                downloadStringServiceFake.Verify(s => s.DownloadAsync(
                    It.IsAny<Uri>(),
                    It.IsAny<Action<string>>()), Times.Exactly(2));
            }


        }

        public class Shared
        {
            protected GraphController controller;
            protected Mock<IDownloadStringService> downloadStringServiceFake;
            protected Graph graph;
            protected Mock<ITimer> timerFake;
            protected Mock<IProgressbar> progressBarServiceFake;
            protected GraphConfig graphConfig;

            [SetUp]
            public void Setup()
            {
                graphConfig =new GraphConfig(GraphConfig.NewDefaultConfiguration());
                graphConfig.Database = "Webshop";
                graphConfig.Collection = "ordersPrDay";
                graphConfig.MaxNumberOfDataPoints = 3;
                graphConfig.XAxisProperty = "Date";
                graphConfig.YAxisProperty = "NumberOfSales";
                graphConfig.IsConfigured = true;

                downloadStringServiceFake = new Mock<IDownloadStringService>();
                graph = new Graph();
                timerFake = new Mock<ITimer>();
                progressBarServiceFake = new Mock<IProgressbar>();

                Before();
            }

            protected Configuration configuration
            {
                set { graphConfig = new GraphConfig(value); }
                get { return graphConfig.Configuration; }
            }

            protected virtual void Before()
            {
                
            }

            protected void DownloadStringServiceFakeReturns(string data)
            {
                DownloadStringServiceFakeReturns(data, null);
            }

            protected void DownloadStringServiceFakeReturns(string data, Action whenDownloadedCallback)
            {
                downloadStringServiceFake.Setup(s => s.DownloadAsync(
                    It.IsAny<Uri>(),
                    It.IsAny<Action<string>>())).Callback((Uri url, Action<string> callback) =>
                    {
                        callback.Invoke(data);
                        if (whenDownloadedCallback != null)
                            whenDownloadedCallback();
                    });
            }

            protected void NewController()
            {
                controller = new GraphController(graph, downloadStringServiceFake.Object, timerFake.Object, graphConfig.Configuration, progressBarServiceFake.Object);
            }
        }
    }
}
