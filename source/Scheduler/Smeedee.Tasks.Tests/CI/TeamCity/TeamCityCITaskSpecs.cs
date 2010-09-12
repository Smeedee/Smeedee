using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tasks.CI.TeamCityCITask;
using Smeedee.Tasks.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Tests.CI.TeamCity
{
    [TestFixture]
    public class When_spawned : Shared
    {
        [SetUp]
        public void SetUp()
        {
            Given(task_is_created_with_config_entries);
            When("");
        }

        [Test]
        public void assure_it_is_instansiated()
        {
            Then("The object should be instansiated", () =>
                Task.ShouldNotBeNull());
        }

        [Test]
        public void assure_it_is_of_type_task_base()
        {
            Then("The task should be tyoe of TaskBase", () => 
                Task.ShouldBeInstanceOfType<TaskBase>());
        }

        [Test]
        public void assure_task_name_is_set()
        {
            Then("Name should not be null", () => 
                Task.Name.ShouldNotBeNull());
        }
    }

    [TestFixture]
    public class When_spawned_with_null_objects : Shared
    {
        [SetUp]
        public void SetUp()
        {
            Given(task_is_created_with_config_entries);
            When("");
        }
        [Test]
        public void assure_exception_is_thrown()
        {
            Then("assure ArgumentNullException is thrown", () =>
                    this.ShouldThrowException<ArgumentNullException>(() =>
                        new TeamCityCiTask(null, null), ex => { }));
        }
        [Test]
        public void assure_exception_for_null_IPersistRepository_is_thrown()
        {
            Then("assure ArgumentNullException is thrown", () =>
                    this.ShouldThrowException<ArgumentNullException>(() =>
                        new TeamCityCiTask(null, config.Object), ex => { }));
        }

        [Test]
        public void assure_exception_for_null_TaskConfig_is_thrown()
        {
            Then("assure ArgumentNullException is thrown", () =>
                    this.ShouldThrowException<ArgumentNullException>(() =>
                        new TeamCityCiTask(dataPersisterMock.Object, null), ex => { }));
        }
    }

    public class Shared : ScenarioClass
    {
        protected static Mock<IPersistDomainModels<CIServer>> dataPersisterMock;
        protected static Mock<TaskConfiguration> config;
        protected static TeamCityCiTask Task;

        protected Context task_is_created_with_config_entries = () =>
        {
            config = new Mock<TaskConfiguration>();
            config.Setup(c => c.Entries).Returns(
            new List<TaskConfigurationEntry>() { null, null, null, null });

            dataPersisterMock = new Mock<IPersistDomainModels<CIServer>>();

            Task = new TeamCityCiTask(dataPersisterMock.Object, config.Object);
        };

        protected Context task_is_created_without_config_entries = () =>
        {
            config = new Mock<TaskConfiguration>();
            config.Setup(c => c.Entries).Returns(new List<TaskConfigurationEntry>());

            dataPersisterMock = new Mock<IPersistDomainModels<CIServer>>();

            Task = new TeamCityCiTask(dataPersisterMock.Object, config.Object);
        };

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }

}
