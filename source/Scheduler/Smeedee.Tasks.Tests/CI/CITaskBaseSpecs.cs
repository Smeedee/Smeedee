using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Tasks.CI;
using Smeedee.Tasks.Framework.Factories;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Tasks.Tests.CI
{
    public class CITaskBaseSpecs
    {
        [TestFixture]
        public class When_spawned : Shared
        {
            [Test]
            public void should_be_instance_of_CITaskBase()
            {
                Given(task_is_created);
                When("");
                Then("the Task is an instance of TaskBase", () =>
                Task.ShouldBeInstanceOfType<CiTaskBase>());
            }

            [Test]
            public void should_be_instansiated()
            {
                Given(task_is_created);
                When("");
                Then("Task should not be null", () =>
                    Task.ShouldNotBeNull());
            }

            [Test]
            public void Assure_DatabasePersister_arg_is_validated()
            {
                Given("DatabasePersister is NullObj");
                When(task_is_instansiated);
                Then("assure ArgumentNullException is thrown", () =>
                    this.ShouldThrowException<ArgumentNullException>(() =>
                        new MockCiTask(null, null), ex => { }));
            }

            [Test]
            public void Assure_ConfigurationRepository_arg_is_validated()
            {
                Given("ConfigurationRepository is NullObj");
                When(task_is_instansiated);
                Then("assure ArgumentNullException is thrown", () =>
                    this.ShouldThrowException<ArgumentNullException>(() =>
                        new MockCiTask(databasePersisterMock.Object, null), ex => { }));
            }
        }

        [TestFixture]
        public class When_dispatced : Shared
        {

            [SetUp]
            public void SetUp()
            {
                Given(task_is_created);
                When(dispatching);
            }

            [Test]
            public void assure_CI_servers_are_persisted_once()
            {
                Then("Assure that save has been used on the persister", () =>
                    databasePersisterMock.Verify(d => d.Save(It.IsAny<IEnumerable<CIServer>>()), Times.Once()));
            }
        }

        public class MockCiTask :  CiTaskBase
        {
            private TaskConfiguration _config;

            public MockCiTask(IPersistDomainModels<CIServer> databasePersister, TaskConfiguration config) : base(databasePersister, config)
            {
                _config = config;
            }

            public override void Execute()
            {
                PersistCiServers(GetAllCIServers());
            }

            public IRepository<CIServer> GetAllCIServers()
            {
                var sourceRepoMock = new Mock<IRepository<CIServer>>();

                return sourceRepoMock.Object;
            }
        }

        public class Shared : ScenarioClass
        {
            protected static Mock<IPersistDomainModels<CIServer>> databasePersisterMock = new Mock<IPersistDomainModels<CIServer>>();
            protected static Mock<IRepository<CIServer>> sourceRepositoryMock = new Mock<IRepository<CIServer>>();
            protected static Mock<IRepository<CIServer>> ciServerRepositoryMock = new Mock<IRepository<CIServer>>();
            protected static Mock<IAssembleRepository<CIServer>> repositoryFactoryMock = new Mock<IAssembleRepository<CIServer>>();
            protected static MockCiTask Task;
            protected static Mock<TaskConfiguration> configuration = new Mock<TaskConfiguration>(); 

            protected Context task_is_created = () =>
            {
                Task = new MockCiTask(databasePersisterMock.Object, configuration.Object);
            };

            protected When task_is_instansiated = () =>
            {
                Task = new MockCiTask(databasePersisterMock.Object, configuration.Object);
            };

            protected When dispatching = () =>
            {
                Task.Execute();
            };

            [TearDown]
            public void TearDown()
            {
                StartScenario();
            }
        }
    }
}
