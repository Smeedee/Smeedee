using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Smeedee.Client.Framework.Services;
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Integration.Database.DomainModel.Charting;
using Smeedee.Tasks.Charting;
using Smeedee.Tasks.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Tests.Charting
{
    [TestFixture]
    public class When_Spawned : Shared
    {

        [SetUp]
        public void SetUp()
        {
            Given(Task_is_created);
            When("");
        }

        [Test]
        public void assure_it_is_instansiated()
        {
            Then("the task should not be null", () => Task.ShouldNotBeNull());
        }
        [Test]
        public void assure_task_name_is_set()
        {
            Then("Name should not be null", () =>
                Task.Name.ShouldNotBeNull());
        }
        [Test]
        public void assure_it_is_of_type_task_base()
        {
            Then("The task should be tyoe of TaskBase", () =>
                Task.ShouldBeInstanceOfType<TaskBase>());
        }

    }

    [TestFixture]
    public class When_getting_data_From_file : Shared
    {

        protected override void Before()
        {
            base.Before();
            datasets = null;
        }

        [Test]
        public void assure_one_dataset_is_created_when_one_line_in_file()
        {
            Given(Task_is_created).And(File_contains_one_row_with_three_values);
            When("loading data from file", () => Task.GetDataSetFromFile("http://my.url", ",", DataFromFile));
            Then("one dataset should contain three values", () =>
                                                            {
                                                                datasets[0].DataPoints.Count.ShouldBe(3);
                                                                datasets.Count.ShouldBe(1);
                                                            });
        }

        [Test]
        public void assure_two_dataset_is_created_when_two_lines_in_file()
        {
            Given(Task_is_created).And(File_contains_two_rowes_with_three_values_each);
            When("loading data from file", () => Task.GetDataSetFromFile("http://my.url", "-", DataFromFile));
            Then("two datasets should contain three values each", () =>
                                                                      {
                                                                          datasets[0].DataPoints.Count.ShouldBe(3);
                                                                          datasets[1].DataPoints.Count.ShouldBe(3);
                                                                          datasets.Count.ShouldBe(2);
                                                                      });
        }

        [Test]
        public void assure_that_chart_is_saved_when_execute_is_called()
        {
            Given(Task_is_created).And(File_contains_one_row_with_three_values);
            When(Task_is_dispatched);
            Then(Chart_should_be_saved_in_storage);
        }


    }

    [TestFixture]
    public class When_executing_task : Shared
    {


        [Test]
        public void assure_datasets_have_same_length()
        {
            Given(Task_is_created).And(File_contains_two_rowes_with_three_and_four_values);
            When("data is loaded from file", () => Task.GetDataSetFromFile("http://my.url", ",", DataFromFile));
            Then("the datasets should be have the same number of values", () =>
                                                                              {
                                                                                  var largestDataset = datasets[1];
                                                                                  var smallestDataset = datasets[0];
                                                                                  smallestDataset.DataPoints.Count.ShouldBe(3);
                                                                                  largestDataset.DataPoints.Count.ShouldBe(smallestDataset.DataPoints.Count);
                         });

        }
        [Test]
        public void assure_filepath_is_valid() // does the file exist?
        {
            Given(Task_is_created);
            When("file is loading");
            Then("file should be valid", () =>
            {
                var filepath = config.ReadEntryValue(ChartingTask.FILEPATH) as string;
                ChartingTask.IsValidURL(filepath).ShouldBeTrue();

                ChartingTask.IsValidURL("not an URL").ShouldBeFalse();

                ChartingTask.IsValidURL(@"file:///c:/someFolder/someFile.txt").ShouldBeTrue();
            });
        }
        
        [Test]
        public void check_if_dataset_starts_with_a_name()
        {
             Given(Task_is_created).And(File_contains_one_row_with_a_name_and_three_values);
             When("data is loaded from file", () => Task.GetDataSetFromFile("http://my.url", ",", DataFromFile));
            Then("check if the first value in each dataset is a \"name\"", () =>
                                                                               {
                                                                                   datasets[0].Name.ShouldBe("name");
                                                                                   datasets[0].DataPoints.Count.ShouldBe(3);
                                                                               });

        }

    }


    public class Shared : ScenarioClass
    {
        protected static ChartingTask Task;
        protected static TaskConfiguration config;
        protected static Mock<IChartStorage> chartStorage;
        protected static Mock<IDownloadStringService> downloadStringServiceFake;

        protected Context Task_is_created = () =>
                                                {
                                                    Task = new ChartingTask(chartStorage.Object, config, downloadStringServiceFake.Object);
                                                };

        protected Context File_contains_one_row_with_three_values = () => DownloadStringServiceFakeReturns("1,1,1");
        protected Context File_contains_one_row_with_a_name_and_three_values = () => DownloadStringServiceFakeReturns("name,1,1,1");
        protected Context File_contains_two_rowes_with_three_values_each = () => DownloadStringServiceFakeReturns("1-1-1 \n 2-2-2");
        protected Context File_contains_two_rowes_with_three_and_four_values = () => DownloadStringServiceFakeReturns("1,1,1 \n 2,2,2,3");

        protected When Task_is_dispatched = () => Task.Execute();

        protected static IList<DataSet> datasets = null;

        protected static void DataFromFile(IList<DataSet> data)
        {
            datasets = data;
        }

        protected static Then Chart_should_be_saved_in_storage = () =>
                                                                       chartStorage.Verify(
                                                                           s =>
                                                                           s.Save(It.Is<Chart>(c => VerifyChart(c))));


        protected static bool VerifyChart(Chart chart)
        {
            return chart.DataSets.Count == 1 && chart.DataSets[0].DataPoints.Count == 3;

        }

        protected virtual void Before()
        {
        }

        [SetUp]
        public void Setup()
        {
            Scenario("");
            config = new TaskConfiguration { Entries = new List<TaskConfigurationEntry>() { new TaskConfigurationEntry { Name = ChartingTask.VALUE_SEPARATOR, Value = ",", Type = typeof(string) }, new TaskConfigurationEntry { Name = ChartingTask.FILEPATH, Value = "http://my.url", Type = typeof(string) }, new TaskConfigurationEntry { Name = ChartingTask.DATABASE_NAME, Value = "database", Type = typeof(string) }, new TaskConfigurationEntry { Name = ChartingTask.COLLECTIONS_NAME, Value = "collection", Type = typeof(string) } } };
            chartStorage = new Mock<IChartStorage>();
            downloadStringServiceFake = new Mock<IDownloadStringService>();
            Before();
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }

        protected static void DownloadStringServiceFakeReturns(string data)
        {
            downloadStringServiceFake.Setup(s => s.DownloadAsync(
                It.IsAny<Uri>(),
                It.IsAny<Action<string>>())).Callback((Uri url, Action<string> callback) =>
                    callback(data));
        }
    }

}
