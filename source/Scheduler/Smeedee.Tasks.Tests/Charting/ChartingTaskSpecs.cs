using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Charting;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
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
        [SetUp]
        public void SetUp()
        {
            Given(Task_is_created);
        }
        [Test]
        public void get_the_correct_format()
        {
            var chart = new Chart();

            var dataset = new DataSet();
            dataset.DataPoints.Add(new DataPoint { X = 1, Y = 1 });
            dataset.DataPoints.Add(new DataPoint { X = 2, Y = 1 });
            dataset.DataPoints.Add(new DataPoint { X = 3, Y = 1 });
            chart.DataSets.Add(dataset);

            Mock<IFile> filesystem = new Mock<IFile>();
            filesystem.Setup(fs => fs.ReadAllLines(It.IsAny<string>())).Returns(new [] {"inntekt,1,2,3"});
            
            
            Assert.AreEqual("", Task.GetDataSetFromFile("asd",","));
            
        }
    }

    [TestFixture]
    public class When_dispatched : Shared
    {
        [Test]
        [Ignore]
        public void assure_will_save_all_charts_in_repo()
        {
            Given(Task_is_created).And("have some charts in repo");
            When(Task_is_dispatched);
            Then("charts are added to the database", () =>
                                                         {
                                                             // insert code to check..
                                                         });

        }

    }


    public class Shared : ScenarioClass
    {
        protected static ChartingTask Task;
        protected static Mock<IPersistDomainModels<Chart>> databasePersister;
        protected static Mock<TaskConfiguration> config;


        protected Context Task_is_created = () =>
                                                {
                                                    databasePersister = new Mock<IPersistDomainModels<Chart>>();
                                                    //databasePersister.Setup()
                                                    config = new Mock<TaskConfiguration>();
                                                    config.Setup(c => c.Entries).Returns(new List<TaskConfigurationEntry>() { null, null, null, null });

                                                    Task = new ChartingTask(null, null, null);
                                                };

        protected When Task_is_dispatched = () => Task.Execute();

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }

    public interface IFile
    {
        string[] ReadAllLines(string path);
    }

    public class FileImpl : IFile
    {
        public virtual string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }
    }
}
