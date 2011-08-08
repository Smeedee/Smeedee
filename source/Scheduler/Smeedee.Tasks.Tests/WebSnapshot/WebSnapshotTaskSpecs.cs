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

    [TestFixture]
    public class When_initializing_a_WebSnapshotTask : Shared
    {

        [Test]
        public void Assure_filename_is_generated_from_task_name()
        {
            Given(Task_is_created);
            When("");
            Then("filename should be generated", () =>
            {
                var fileName = task.GenerateFilename();
                fileName.ShouldNotBeNull();
                task.ValidateFilename(fileName).ShouldBeTrue();
            });

        }

        [Test]
        public void assure_it_can_validate_filenames()
        {
            Given(Task_is_created);
            When("");
            Then("invalid filename is detected",
                 () => task.ValidateFilename("not / valid -@#$%^%& at =\\ all").ShouldBeFalse());
        }

        [Test]
        public void Assure_invalid_filename_is_fixed()
        {
            Given(Broken_task_is_created);
            When("");
            Then("correct filename is generated", () =>
            {
                var fileName = task.GenerateFilename();
                fileName.ShouldNotBeNull();
                task.ValidateFilename(fileName).ShouldBeTrue();
            });
        }

    }


    public class Shared : ScenarioClass
    {
        protected static WebSnapshotTask task;
        protected static TaskConfiguration config;
        protected static TaskConfiguration brokenConfig;

        protected static Mock<IPersistDomainModels<Smeedee.DomainModel.WebSnapshot.WebSnapshot>> databasepersister;

        protected Context Task_is_created = () => { task = new WebSnapshotTask(config, databasepersister.Object); };
        
        protected Context Broken_task_is_created = 
            () => task = new WebSnapshotTask(brokenConfig, databasepersister.Object);
        
        protected When Task_is_dispatched = () => task.Execute();


        [SetUp]
        public void Setup()
        {
            Scenario("");
            config = new TaskConfiguration
                         {
                             Entries =
                                 new List<TaskConfigurationEntry>()
                                     {
                                         new TaskConfigurationEntry
                                             {
                                                 Name = WebSnapshotTask.WEBPAGE,
                                                 Value = "http://smeedee.org/",
                                                 Type = typeof (string)
                                             },
                                         new TaskConfigurationEntry
                                             {
                                                 Name = WebSnapshotTask.XPATH,
                                                 Value = "/html/body/div[2]/div[2]/div/div/div/img",
                                                 Type = typeof (string)
                                             },
                                         new TaskConfigurationEntry
                                             {
                                                 Name = WebSnapshotTask.SMEEDEEPATH,
                                                 Value = @"C:\path\to\smeedee",
                                                 Type = typeof (string)
                                             }
                                     }
                         };
            config.Name = "New Websnapshot Task";

            brokenConfig = new TaskConfiguration
                               {
                                   Entries =
                                       new List<TaskConfigurationEntry>()
                                           {
                                               new TaskConfigurationEntry
                                                   {
                                                       Name = WebSnapshotTask.WEBPAGE,
                                                       Value = "http://smeedee.org/",
                                                       Type = typeof (string)
                                                   },
                                               new TaskConfigurationEntry
                                                   {Name = WebSnapshotTask.XPATH, Value = "", Type = typeof (string)},
                                               new TaskConfigurationEntry
                                                   {
                                                       Name = WebSnapshotTask.SMEEDEEPATH,
                                                       Value = @"C:\path\to\smeedee",
                                                       Type = typeof (string)
                                                   }
                                           }
                               };
            brokenConfig.Name = "Not valid//in some:; \\ / &^ filesystems!@#$%()";
            
            databasepersister = new Mock<IPersistDomainModels<DomainModel.WebSnapshot.WebSnapshot>>();
        }

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }

    }



}
