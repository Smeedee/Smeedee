#region File header

// <copyright>
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
// </copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>
// 
// <author>Your Name</author>
// <email>your@email.com</email>
// <date>2009-MM-DD</date>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.CI;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.Tasks.CI;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.Factories;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.TaskTests.CI.CITaskSpecs
{
    public class Shared : ScenarioClass
    {
        protected static Mock<IPersistDomainModels<CIServer>> databasePersisterMock = new Mock<IPersistDomainModels<CIServer>>();
        protected static Mock<IRepository<CIServer>> sourceRepositoryMock = new Mock<IRepository<CIServer>>();
        protected static Mock<IRepository<CIServer>> ciServerRepositoryMock = new Mock<IRepository<CIServer>>();
        protected static Mock<IAssembleRepository<CIServer>> repositoryFactoryMock = new Mock<IAssembleRepository<CIServer>>();
        protected static Mock<IRepository<Configuration>> configurationRepositoryMock = new Mock<IRepository<Configuration>>();
        protected static CITask Task;

        protected Context Task_is_not_created = () => { };

        protected Context RepositoryFactory_is_created = () =>
        {
            sourceRepositoryMock = new Mock<IRepository<CIServer>>();

            repositoryFactoryMock = new Mock<IAssembleRepository<CIServer>>();
            repositoryFactoryMock.Setup(r => r.Assemble(It.IsAny<Configuration>())).
                Returns(sourceRepositoryMock.Object);
        };

        protected Context DatabasePersister_is_created = () =>
        {
            databasePersisterMock = new Mock<IPersistDomainModels<CIServer>>();
        };

        protected Context ConfigurationRepository_is_created = () =>
        {
            configurationRepositoryMock = new Mock<IRepository<Configuration>>();
        };

        protected Context ConfigurationRepository_contains_CI_Configuration = () =>
        {
            var configs = new List<Configuration>();
            var ciConfig = Configuration.DefaultCIConfiguration();
            ciConfig.NewSetting("provider", "cc.net");
            configs.Add(ciConfig);

            configurationRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).
                Returns(configs);
        };

        protected Context ConfigurationRepository_does_not_contain_CI_Configuration = () =>
        {
            var configs = new List<Configuration>();
            configurationRepositoryMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).
                Returns(configs);
        };

        protected Context Task_is_created = () =>
        {
            Task = new CITask(repositoryFactoryMock.Object, databasePersisterMock.Object, configurationRepositoryMock.Object);
        };

        protected When spawning_Task = () => { };

        protected When dispatching = () =>
        {
            Task.Execute();
        };
    
        public GivenSemantics Dependencies_are_created()
        {
            return Given(RepositoryFactory_is_created).
                And(DatabasePersister_is_created).
                And(ConfigurationRepository_is_created);
        }

        protected Then assure_CI_Configuration_is_fetched = () =>
        {
            configurationRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<Configuration>>()), Times.Once());
        };

        [TearDown]
        public void TearDown()
        {
            StartScenario();
        }
    }

    [TestFixture]
    public class When_spawning : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Spawning new CITask");
            Given(Task_is_not_created);
        }

        [Test]
        public void Assure_CIServerRepositoryFactory_is_validated()
        {
            Given("CIServerRepositoryFactory is NullObj");
            When(spawning_Task);
            Then("assure ArgumentNullException is thrown", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new CITask(null, null, null), ex => { }));
        }

        [Test]
        public void Assure_DatabasePersister_arg_is_validated()
        {
            Given("DatabasePersister is NullObj");
            When(spawning_Task);
            Then("assure ArgumentNullException is thrown", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new CITask(repositoryFactoryMock.Object, null, null), ex => { }));
        }

        [Test]
        public void Assure_ConfigurationRepository_arg_is_validated()
        {
            Given("ConfigurationRepository is NullObj");
            When(spawning_Task);
            Then("assure ArgumentNullException is thrown", () =>
                this.ShouldThrowException<ArgumentNullException>(() =>
                    new CITask(repositoryFactoryMock.Object, databasePersisterMock.Object, null), ex => { }));
        }

    }   

    [TestFixture]
    public class When_dispatched : Shared
    {
        [SetUp]
        public void Setup()
        {
            Scenario("Dispatching Task");
            Given(Dependencies_are_created()).
                And(ConfigurationRepository_contains_CI_Configuration).
                And(Task_is_created);

            When(dispatching);
        }

        [Test]
        public void Assure_CI_Configuration_is_fetched()
        {
            Then(assure_CI_Configuration_is_fetched);
        }

        [Test]
        public void Assure_data_is_fetched_from_SourceRepository()
        {
            Then("assure data is fetched from SourceRepository", () =>
                sourceRepositoryMock.Verify(r => r.Get(It.IsAny<Specification<CIServer>>()), Times.Once()));
        }

        [Test]
        public void Assure_fetched_data_is_persisted()
        {
            Then("assure fetched data is persisted", () =>
                databasePersisterMock.Verify(r => r.Save(It.IsAny<IEnumerable<CIServer>>()), Times.Once()));
        }
    }

    [TestFixture]
    public class When_dispatched_and_CI_is_not_Configured : ScenarioClass
    {
        [Test]
        [ExpectedException(typeof(TaskConfigurationException))]
        public void Assure_TaskConfigurationException_Is_thrown()
        {
                var configRepoMock = new Mock<IRepository<Configuration>>();
                CITask Task = null;

                Scenario("Dispatching Task when CI is not configured");
                Given("There is no config for CI", ()=> 
                        configRepoMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>()))
                            .Returns(new List<Configuration>())).
                    And("The Task is created", ()=> 
                        Task = new CITask(new Mock<IAssembleRepository<CIServer>>().Object, new Mock<IPersistDomainModels<CIServer>>().Object, configRepoMock.Object));

                When("dispacthing", ()=> Task.Execute());
                Then("assure exception is thrown");
                StartScenario();
        }
    }
}
