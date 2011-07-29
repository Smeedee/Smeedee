using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tasks.WebSnapshot;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Tests.WebSnapshot
{

    [TestFixture]
    public class When_Spawned : Shared
    {

        [Test]
        public void assure_it_is_instantiated()
        {
            Given(Task_is_created);
            When("");
            Then("the task should not be null", () => task.ShouldNotBeNull());
        }



    }

    //[TestFixture]
    //public class When_initializing_a_WebSnapshotTask : Shared
    //{
    //    [Test]
    //    [ExpectedException(typeof(TaskConfigurationException))]
    //    public void Assure_an_exception_is_thrown_if_URL_is_missing()
    //    {
    //        Given(Broken_task_is_created);
    //        When("");
    //        Then("exception is thrown");
    //    } 
    //}

    //[TestFixture]
    //public class When_page_URL_is_set : Shared
    //{
    //    [Test]
    //    public void assure_it_is_valid()
    //    {
    //        Given(Task_is_created).And("url is set");
    //        When("");
    //        Then("assure it is valid", () => { config.ReadEntryValue(WebSnapshotTask.WEBPAGE) })
    //    }
    //}


    public class Shared : ScenarioClass
    {
        protected static WebSnapshotTask task;
        protected static TaskConfiguration config;
        protected static TaskConfiguration brokenConfig;
        //protected static TaskConfiguration emptyConfig;
        protected static Mock<IPersistDomainModels<Smeedee.DomainModel.WebSnapshot.WebSnapshot>> databasepersister;

        protected Context Task_is_created = () => { task = new WebSnapshotTask(config, databasepersister.Object); };
        
        protected Context Broken_task_is_created = 
            () => { task = new WebSnapshotTask(brokenConfig, databasepersister.Object); };
        //protected Context Empty_task_is_created =
        //    () => { task = new WebSnapshotTask(emptyConfig, databasepersister.Object); };

        protected When Task_is_dispatched = () => task.Execute();
        


        [SetUp]
        public void Setup()
        {
            Scenario("");
            config = new TaskConfiguration { Entries = new List<TaskConfigurationEntry>() { new TaskConfigurationEntry { Name = WebSnapshotTask.WEBPAGE, Value = "http://smeedee.org/", Type = typeof(string) }, new TaskConfigurationEntry { Name = WebSnapshotTask.XPATH, Value = "/html/body/div[2]/div[2]/div/div/div/img", Type = typeof(string) } } };
            brokenConfig = new TaskConfiguration { Entries = new List<TaskConfigurationEntry>() { new TaskConfigurationEntry { Name = WebSnapshotTask.WEBPAGE, Value = "not an url", Type = typeof(string) }, new TaskConfigurationEntry { Name = WebSnapshotTask.XPATH, Value = "", Type = typeof(string) } } };
            //emptyConfig = new TaskConfiguration { Entries = new List<TaskConfigurationEntry>() { new TaskConfigurationEntry { Name = WebSnapshotTask.WEBPAGE, Value = "", Type = typeof(string) }, new TaskConfigurationEntry { Name = WebSnapshotTask.XPATH, Value = "", Type = typeof(string) } } };
            
            databasepersister = new Mock<IPersistDomainModels<DomainModel.WebSnapshot.WebSnapshot>>();


        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }

    }



}
