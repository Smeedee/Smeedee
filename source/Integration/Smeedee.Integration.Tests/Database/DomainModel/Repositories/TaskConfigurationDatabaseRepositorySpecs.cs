using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.TaskInstanceConfiguration;
using Smeedee.Integration.Database.DomainModel.Repositories;
using Smeedee.IntegrationTests.Database.DomainModel.Repositories;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;

namespace Smeedee.Integration.Tests.Database.DomainModel.Repositories
{
    [TestFixture][Category("IntegrationTest")]
    class TaskConfigurationDatabaseRepositorySpecs : TaskConfigurationRepositoryShared
    {
        [Test]
        public void Assure_there_are_no_rows_in_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                        .And(Repository_is_created);

                scenario.When("Get is called", () =>
                              resultSet = repository.Get(new AllSpecification<TaskConfiguration>()));

                scenario.Then("Assure result set is empty", () =>
                              resultSet.Count().ShouldBe(0));
            });
        }

        [Test]
        public void Assure_there_are_some_rows_in_resultset()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(Database_exists)
                        .And(Repository_is_created)
                        .And(Database_contains_one_configuration);

                scenario.When("Get is called", () =>
                              resultSet = repository.Get(new AllSpecification<TaskConfiguration>()));

                scenario.Then("Assure result set is not empty", () =>
                              resultSet.Count().ShouldNotBe(0));
            });
        }

        [Test]
        public void Assure_database_is_wiped_when_saving_set_of_configurations()
        {
            Scenario.StartNew(this, s =>
            {
                s.Given(Database_exists)
                    .And(Repository_is_created)
                    .And(Database_contains_one_configuration);

                s.When("We save a new set of configurations", () => 
                    repository.Save(new List<TaskConfiguration> { new TaskConfiguration() { Name = "New Config" } }));

                s.Then("Only the last set of configuration should be in the database", () =>
                {
                    resultSet = repository.Get(new AllSpecification<TaskConfiguration>());
                    resultSet.Count().ShouldBe(1);
                    resultSet.First().Name.ShouldBe("New Config");
                });
            });
        }

        [Test]
        public void Assure_types_outside_mscorlib_can_be_saved_then_loaded()
        {
            Scenario.StartNew(this, s =>
            {
                s.Given(Database_exists)
                    .And(Repository_is_created);
                
                s.When("We save a configuration containing a System.Uri", () =>
                    repository.Save(TaskConfigurationListWithUrlConfiguration()));

                s.Then("Only the last set of configuration should be in the database", () =>
                {
                    var uriConfigurationEntry = repository.Get(new AllSpecification<TaskConfiguration>()).First().Entries.First();
                    uriConfigurationEntry.Value.ToString().ShouldBe("http://smeedee.org/");
                });
            });
        }
    }

    public class TaskConfigurationRepositoryShared : Shared
    {
        protected static TaskConfigurationDatabaseRepository repository;
        protected IEnumerable<TaskConfiguration> resultSet;

        protected Context Repository_is_created = () =>
        {
            repository = new TaskConfigurationDatabaseRepository(sessionFactory);
        };

        protected Context Database_contains_one_configuration = () =>
        {
            var config = new TaskConfiguration { Name = "name", TaskName = "taskName" };
            repository.Save(config);
        };

        protected Context Database_contains_many_configurations = () =>
        {
            var configs = new List<TaskConfiguration>();

            for (int i = 0; i < 8; i++)
                configs.Add(new TaskConfiguration { Name = "name", TaskName = "taskName" });

            repository.Save(configs);
        };

        protected IEnumerable<TaskConfiguration> TaskConfigurationListWithUrlConfiguration()
        {

            return new List<TaskConfiguration>
                       {
                           new TaskConfiguration
                               {
                                   Name = "New Config",
                                   Entries = new List<TaskConfigurationEntry> { UrlConfigEntry() }
                               }
                       };
        }

        private TaskConfigurationEntry UrlConfigEntry()
        {
            return new TaskConfigurationEntry
                       {
                           Name = "Hello World",
                           Type = typeof (Uri),
                           Value = new Uri("http://smeedee.org")
                       };
        }
    }
}

