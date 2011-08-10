using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Smeedee.DomainModel.CI;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using TinyBDD.Dsl.GivenWhenThen;
using NUnit.Framework;

using TinyBDD.Specification.NUnit;
using System.IO;

using Environment=System.Environment;
using Smeedee.Integration.Database.DomainModel.Repositories;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories.NHibernateMappings.ContinuousIntegrationDomainModelTests
{
    public class Shared
    {
        protected static ISession session;
        protected static IList<CIServer> data;

        protected Context database_Session_is_created = () =>
        {
            OpenNewSession();
        };

        protected static void OpenNewSession()
        {
            session = CreateSession();
        }

        private static ISession CreateSession()
        {
            var sessionFactory = NHibernateFactory.AssembleSessionFactory("SmeedeeDB_testing.db");

            return sessionFactory.OpenSession();
        }

        protected Context database_is_empty = () =>
        {
            DeleteDatabaseFiles();
        };

        private static void DeleteDatabaseFiles()
        {
            var dbFiles = Directory.GetFiles(Environment.CurrentDirectory).Where(f => f.EndsWith(".db") || f.EndsWith(".DB"));
            foreach (var dbFile in dbFiles)
                File.Delete(dbFile);
        }

        protected Context we_have_data_to_persist = () =>
        {
            data = new List<CIServer>();
            var server = new CIServer("cc.net", "http://smeedee.net/ccnet");
            data.Add(server);

            var project = new CIProject("development")
            {
                SystemId = "Development"
            };
            server.AddProject(project);
            
            project.AddBuild(new Build()
            {
                SystemId =  DateTime.Today.ToString(),
                StartTime = new DateTime(1981, 9, 1),
                Status = BuildStatus.Building,
                FinishedTime = new DateTime(1981, 9, 1).AddHours(1),
                Trigger = new CodeModifiedTrigger("goeran")
            });
        };

        protected Context data_is_persisted = () =>
        {
            PersistData();
        };

        protected When persisting_CIServer = () =>
        {
            PersistData();
        };

        private static void PersistData()
        {
            session.BeginTransaction();
            session.SaveOrUpdate(data[0]);
            session.Transaction.Commit();
        }

        protected int NumberOfObjectsPersisted(Type typeOfObject)
        {
            OpenNewSession();
            return session.CreateCriteria(typeOfObject).List().Count;
        }

        protected IEnumerable<T> QueryPersistedObjects<T>()
        {
            OpenNewSession();
            var retValue = new List<T>();

            foreach (var item in session.CreateCriteria(typeof (T)).List())
            {
                retValue.Add((T)item);
            }

            return retValue;
        }

        protected IEnumerable<CIProject> QueryAllProjectsInMemory(Func<CIProject, bool> predicate)
        {
            var projects = new List<CIProject>();
            foreach (var server in data)
                projects.AddRange(server.Projects);

            return projects.Where(predicate);
        }

        protected IEnumerable<Build> QueryAllBuildsInMemory(Func<Build, bool> predicate)
        {
            var builds = new List<Build>();
            foreach (var server in data)
                foreach (var project in server.Projects)
                    foreach (var build in project.Builds)
                        builds.Add(build);

            return builds.Where(predicate);
        }

        protected GivenSemantics Given_database_exists(Semantics scenario)
        {
            return scenario.Given(database_is_empty).
                    And(database_Session_is_created).
                    And(we_have_data_to_persist);
        }


    }

    [TestFixture][Category("IntegrationTest")]
    public class When_persisting_new_CIServer : Shared
    {
        [Test]
        public void Assure_CIServer_is_persisted()
        {
            Scenario.StartNew(this, scenario =>
            {
                Given_database_exists(scenario);

                scenario.When(persisting_CIServer);
                scenario.Then("assure CIServer is persisted", () =>
                {
                    var servers = QueryPersistedObjects<CIServer>();
                    servers.Count().ShouldBe(1);
                    servers.First().Name.ShouldBe(data.First().Name);
                    servers.First().Url.ShouldBe(data.First().Url);
                });
            });
        }

        [Test]
        public void Assure_CIProject_is_persisted()
        {
            Scenario.StartNew(this, scenario =>
            {
                Given_database_exists(scenario);

                scenario.When(persisting_CIServer);
                scenario.Then("assure CIProject is persisted", () =>
                {
                    var projects = QueryPersistedObjects<CIProject>();
                    projects.Count().ShouldBe(1);
                    
                    var persistedProject = projects.First();
                    var memoryProject = QueryAllProjectsInMemory(p => p.SystemId == persistedProject.SystemId).SingleOrDefault();
                    
                    persistedProject.SystemId.ShouldBe(memoryProject.SystemId);
                    persistedProject.ProjectName.ShouldBe(memoryProject.ProjectName);
                    persistedProject.Server.Url.ShouldBe(memoryProject.Server.Url);
                    persistedProject.Builds.Count().ShouldBe(memoryProject.Builds.Count());
                    persistedProject.LatestBuild.ShouldBe(memoryProject.LatestBuild);
                });
            });   
        }

        [Test]
        public void Assure_Build_is_persisted()
        {
            Scenario.StartNew(this, scenario =>
            {
                Given_database_exists(scenario);

                scenario.When(persisting_CIServer);
                scenario.Then("assure Build is persisted", () =>
                {
                    var builds = QueryPersistedObjects<Build>();
                    builds.Count().ShouldBe(1);

                    var persistedBuild = builds.First();
                    var memoryBuild =
                        QueryAllBuildsInMemory(b => b.SystemId == persistedBuild.SystemId).SingleOrDefault();

                    persistedBuild.SystemId.ShouldBe(memoryBuild.SystemId);
                    persistedBuild.StartTime.ShouldBe(memoryBuild.StartTime);
                    persistedBuild.Status.ShouldBe(memoryBuild.Status);
                    persistedBuild.FinishedTime.ShouldBe(memoryBuild.FinishedTime);
                    persistedBuild.Project.ShouldNotBeNull();
                    persistedBuild.Project.SystemId.ShouldBe(memoryBuild.Project.SystemId);
                    persistedBuild.Duration.ShouldBe(memoryBuild.Duration);
                    persistedBuild.Trigger.ShouldNotBeInstanceOfType<UnknownTrigger>();
                    persistedBuild.Trigger.ShouldBeInstanceOfType<Trigger>();
                    persistedBuild.Trigger.InvokedBy.ShouldBe(memoryBuild.Trigger.InvokedBy);
                    persistedBuild.Trigger.Cause.ShouldBe(memoryBuild.Trigger.Cause);
                });
            });
        }
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_CIServer_is_persisted : Shared
    {
        [Test]
        public void Assure_persisted_data_is_equal_to_in_memory_data()
        {
            Scenario.StartNew(this, scenario =>
            {
                IEnumerable<CIServer> persistedData = null;

                Given_database_exists(scenario).
                    And(data_is_persisted);
                
                scenario.When("fetching persisted data", () =>
                    persistedData = session.CreateCriteria(typeof(CIServer)).List<CIServer>());

                scenario.Then("assure persisted data equals to in-memory data", () =>
                {
                    persistedData.First().ShouldBe(data.First());
                    persistedData.First().Projects.First().ShouldBe(data.First().Projects.First());
                    persistedData.First().Projects.First().Builds.First().ShouldBe(
                        data.First().Projects.First().Builds.First());
                });
            });
        }
   
    }

    [TestFixture][Category("IntegrationTest")]
    public class When_updating_CIServer : Shared
    {
        [Test]
        public void Assure_its_possible_to_update_CIServer()
        {
            Scenario.StartNew(this, scenario =>
            {
                Given_database_exists(scenario).
                    And(data_is_persisted);

                scenario.When(persisting_CIServer);

                scenario.Then("assure it's possible to update CIServer");
            });
        }
    }
}
