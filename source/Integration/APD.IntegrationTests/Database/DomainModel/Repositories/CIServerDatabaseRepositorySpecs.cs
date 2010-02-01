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
using APD.DomainModel.CI;
using APD.DomainModel.Framework;

using NHibernate;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.Integration.Database.DomainModel.Repositories;


namespace APD.IntegrationTests.Database.DomainModel.Repositories.CIServerDatabaseRepositorySpecs
{
    public class CIServerRepositoryShared : Shared
    {
        protected static CIServerDatabaseRepository _databaseRepository;
        protected static CIServerDatabaseRepository persister;

        protected static List<CIProject> projects = new List<CIProject> { };
        protected static CIServer ciServer = new CIServer
        {
            Name = "Mock CI Server",
            Projects = projects,
            Url = "hjsdlkfjds"
        };
        protected static CIServer ciServer2 = new CIServer
        {
            Name = "Another CI Server",
            Projects = projects,
            Url = "afdsafd"
        };


        protected Context a_CIServerRepository_is_created = () =>
        {
            var factory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);
            _databaseRepository = new CIServerDatabaseRepository(factory);
            persister = new CIServerDatabaseRepository(factory);
        };

        protected Context there_are_CIServer_objects_in_the_database = () =>
        {

            persister.Save(ciServer);
            persister.Save(ciServer2);
        };

    }

    [TestFixture]
    public class when_created : CIServerRepositoryShared
    {

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }

        [Test]
        public void should_be_created()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_CIServerRepository_is_created);
                scenario.When("instance is accessed");
                scenario.Then("it is not null", () => _databaseRepository.ShouldNotBeNull());
            });
        }

        [Test]
        public void should_implement_IRepository_of_type_CIServer()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_CIServerRepository_is_created);
                scenario.When("instance is accessed");
                scenario.Then("it implements IRepository<CIServer>", () =>
                    _databaseRepository.ShouldBeInstanceOfType<IRepository<CIServer>>());
            });
        }
    }

    [TestFixture]
    public class when_accessed : CIServerRepositoryShared
    {

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }        
        
        [Test]
        public void should_fetch_all_CI_servers_in_database()
        {
            IEnumerable<CIServer> result = null;
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_CIServerRepository_is_created).
                         And(there_are_CIServer_objects_in_the_database);
                scenario.When("all CI Servers are requested", () =>
                    result = _databaseRepository.Get(new AllSpecification<CIServer>()));
                scenario.Then("all CI Servers are returned", () =>
                {
                    result.Count().ShouldBe(2);
                    result.Contains(ciServer);
                    result.Contains(ciServer2);
                });
            });
        }
    }

    [TestFixture]
    public class When_saving_identical_projects_on_different_servers : CIServerRepositoryShared
    {
        private ISession session;

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();

            var sessionFactory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);
            var persister = new CIServerDatabaseRepository(sessionFactory);

            var server = new CIServer("goeran", "http://goeran.no");
            server.AddProject(new CIProject("goeran - development")
            {
                SystemId = "1"
            });

            var server2 = new CIServer("jonas", "http://jonas.no");
            server2.AddProject(new CIProject("jonas - development")
            {
                SystemId = "2"
            });

            persister.Save(server);
            persister.Save(server2);

            session = sessionFactory.OpenSession();
        }

        [Test]
        public void Assure_both_servers_are_saved()
        {
            var servers = session.CreateCriteria(typeof(CIServer)).List();

            servers.Count.ShouldBe(2);
        }

        [Test]
        public void Assure_both_projects_are_saved()
        {
            var projects = session.CreateCriteria(typeof(CIProject)).List();

            projects.Count.ShouldBe(2);
        }
    }

    [TestFixture]
    public class When_saving : CIServerRepositoryShared
    {
        private CIServer server;
        private CIProject project;
        private ISessionFactory factory;

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
            factory = NHibernateFactory.AssembleSessionFactory(DATABASE_TEST_FILE);

            SetupObjectGraph();
        }

        private void SetupObjectGraph()
        {
            this.server = new CIServer()
            {
                Name = "CruiseControl.NET",
                Url = "http://agileprojectdashboard.org/ccnet"
            };

            this.project = new CIProject(TESTPROJECTNAME_ONE)
            {
                SystemId = "ProjectNr.1",
            };

            var project2 = new CIProject(TESTPROJECTNAME_TWO)
            {
                SystemId = "ProjectNr.2",
            };

            server.AddProject(project);
            server.AddProject(project2);


            var build1 = new Build
            {
                SystemId = "Build.1",
                StartTime = new DateTime(2000, 1, 1),
                FinishedTime = new DateTime(2000, 1, 2),
                Status = BuildStatus.FinishedSuccefully,
                Trigger = new CodeModifiedTrigger("tuxbear")
            };

            var build2 = new Build
            {
                SystemId = "Build.2",
                StartTime = new DateTime(2000, 1, 3),
                FinishedTime = new DateTime(2000, 1, 4),
                Status = BuildStatus.FinishedWithFailure,
                Trigger = new CodeModifiedTrigger("tuxbear")
            };

            var build3 = new Build
            {
                SystemId = "Build.3",
                StartTime = new DateTime(2000, 1, 5),
                FinishedTime = new DateTime(2000, 1, 6),
                Status = BuildStatus.Unknown,
                Trigger = new CodeModifiedTrigger("tuxbear")
            };

            var build4 = new Build
            {
                SystemId = "Build.4",
                StartTime = new DateTime(2000, 1, 7),
                FinishedTime = new DateTime(2000, 1, 8),
                Status = BuildStatus.Building,
                Trigger = new CodeModifiedTrigger("tuxbear")
            };

            project.AddBuild(build1);
            project.AddBuild(build2);
            project.AddBuild(new Build
            {
                SystemId = "Build.Common",
                StartTime = DateTime.MinValue,
                FinishedTime = DateTime.MaxValue,
                Status = BuildStatus.FinishedSuccefully,
                Trigger = new CodeModifiedTrigger("tuxbear")
            });

            project2.AddBuild(build3);
            project2.AddBuild(build4);
            project2.AddBuild(new Build
            {
                SystemId = "Build.Common",
                StartTime = DateTime.MinValue,
                FinishedTime = DateTime.MaxValue,
                Status = BuildStatus.FinishedSuccefully,
                Trigger = new CodeModifiedTrigger("tuxbear")
            });

        }

        [Test]
        public void Assure_entire_hierarcy_is_correctly_persisted()
        {

            var persister = new  CIServerDatabaseRepository(factory);

            persister.Save(server);

            RecreateSessionFactory();

            var results = factory.OpenSession().CreateCriteria(typeof(CIServer)).List<CIServer>();

            results.Count.ShouldBe(1);

            var ciServer = results[0];
            ciServer.Projects.Count().ShouldBe(2);

            var project1 = ciServer.Projects.ElementAt(0);
            var project2 = ciServer.Projects.ElementAt(1);

            project1.ProjectName.ShouldBe(TESTPROJECTNAME_ONE);
            project1.Server.ShouldBe(ciServer);
            project1.Builds.Count().ShouldBe(3);
            project1.Builds.ElementAt(0).Project.ShouldBe(project1);
            project1.Builds.ElementAt(1).Project.ShouldBe(project1);
            project1.Builds.ElementAt(2).Project.ShouldBe(project1);

            project2.ProjectName.ShouldBe(TESTPROJECTNAME_TWO);
            project2.Server.ShouldBe(ciServer);
            project2.Builds.Count().ShouldBe(3);
            project2.Builds.ElementAt(0).Project.ShouldBe(project2);
            project2.Builds.ElementAt(1).Project.ShouldBe(project2);
            project2.Builds.ElementAt(2).Project.ShouldBe(project2);
        }


        [Test]
        public void Assure_identical_builds_from_different_projects_are_distinct_in_database()
        {
            var ciProjectPersister = new CIServerDatabaseRepository(factory);
            ciProjectPersister.Save(server);

            var dbResults = factory.OpenSession().CreateCriteria(typeof(CIServer)).List<CIServer>();

            dbResults.Count.ShouldBe(1);
            dbResults[0].Name.ShouldBe(server.Name);
            var commonBuildInProject1 = dbResults[0].Projects.ElementAt(0).Builds.Last();
            var commonBuildInProject2 = dbResults[0].Projects.ElementAt(1).Builds.Last();

            commonBuildInProject1.ShouldNotBe(commonBuildInProject2);
        }
    }

}
