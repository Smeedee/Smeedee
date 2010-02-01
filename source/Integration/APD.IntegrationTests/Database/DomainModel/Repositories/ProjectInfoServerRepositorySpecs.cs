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
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;
using APD.Integration.Database.DomainModel.Repositories;

using NUnit.Framework;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;
using APD.IntegrationTests.Database.DomainModel.Repositories;


namespace APD.Plugin.DatabaseTests.ProjectInfoServerRepositorySpecs
{

    public class ProjectInfoServerRepositorySpecsShared : Shared
    {
        protected static ProjectInfoServerDatabaseRepository repository;

        protected static List<Project> projects = new List<Project>();
       
        protected static ProjectInfoServer firstPIServer = new ProjectInfoServer()
        {
            Name = "ProjectInfoServer 1", 
            Url = "www.firstPIServer.com",
            Projects = projects
        };

        protected static ProjectInfoServer secondPIServer = new ProjectInfoServer()
        {
            Name = "ProjectInfoServer 2",
            Url = "www.secondPIServer.com",
            Projects = projects
        };

        

        protected Context a_ProjectInfoServerRepository_is_created = () =>
        {
            repository = new ProjectInfoServerDatabaseRepository(sessionFactory);
        };


        protected Context there_is_ProjectInfoServers_in_the_database = () =>
        {
            repository.Save(firstPIServer);
            repository.Save(secondPIServer);
        };

    }
   
    [TestFixture]
    public class When_spawned : ProjectInfoServerRepositorySpecsShared
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
                scenario.Given(a_ProjectInfoServerRepository_is_created);
                scenario.When("instance is accessed");
                scenario.Then("it is not null", () => repository.ShouldNotBeNull());
            });
            
        }


        [Test]
        public void should_be_instance_of_type_IRepository_of_type_ProjectInfoServer()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ProjectInfoServerRepository_is_created);
                scenario.When("instance is created");
                scenario.Then("it implements IRepository<ProjectInfoServer", () =>
                    repository.ShouldBeInstanceOfType<IRepository<ProjectInfoServer>>());
            });
        }
    }


    [TestFixture]
    public class When_accessed : ProjectInfoServerRepositorySpecsShared
    {
        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }

        [Test]
        public void should_retrieve_all_ProjectInfoServers_in_database()
        {
            IEnumerable<ProjectInfoServer> retrievedPIServers = new List<ProjectInfoServer>();
            
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ProjectInfoServerRepository_is_created)
                    .And(there_is_ProjectInfoServers_in_the_database);
                scenario.When("ProjectInfoServers are requested", () =>
                    retrievedPIServers = repository.Get(new AllSpecification<ProjectInfoServer>()));                
                scenario.Then("all ProjectInfoServers are be returned", () =>
                    retrievedPIServers.Count().ShouldBe(2));
                    retrievedPIServers.Contains(firstPIServer);
                    retrievedPIServers.Contains(secondPIServer);
            });
        }

    }

    [TestFixture]
    public class When_saving : Shared
    {
        protected new const string TESTPROJECTNAME_ONE = "Project 1";
        protected new const string TESTPROJECTNAME_TWO = "Project 2";
        protected const string TESTITERATIONNAME_ONE = "Iteration 1";
        protected const string TESTITERATIONNAME_TWO = "Iteration 2";
        protected const string TESTTASKNAME_ONE = "Task 1";
        protected const string TESTTASKNAME_TWO = "Task 2";

        private ProjectInfoServer server;


        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();

            SetupObjectGraph();
        }

        private void SetupObjectGraph()
        {
            server = new ProjectInfoServer()
            {
                Name = "RallyDev",
                Url = "http://community.rally.com"
            };

            #region Project setup

            var project1 = new Project()
            {
                Name = TESTPROJECTNAME_ONE,
                SystemId = "project1",
            };
            server.AddProject(project1);

            var project2 = new Project()
            {
                Name = TESTPROJECTNAME_TWO,
                SystemId = "project2",
            };
            server.AddProject(project2);

            #endregion

            #region Iteration setup

            var iteration1 = new Iteration()
            {
                Name = TESTITERATIONNAME_ONE,
                SystemId = "iteration1",
            };
            project1.AddIteration(iteration1);

            var iteration2 = new Iteration()
            {
                Name = TESTITERATIONNAME_TWO,
                SystemId = "iteration2",
            };
            project2.AddIteration(iteration2);

            #endregion

            #region Task setup

            var task1 = new Task()
            {
                Name = TESTTASKNAME_ONE,
                SystemId = "task1",
                WorkEffortEstimate = 1
            };
            iteration1.AddTask(task1);

            var task2 = new Task()
            {
                Name = TESTTASKNAME_TWO,
                SystemId = "task2",
                WorkEffortEstimate = 2,
            };
            iteration2.AddTask(task2);

            #endregion

            #region WorkEffortHistoryItem setup

            var wehi_1_1 = new WorkEffortHistoryItem()
            {
                RemainingWorkEffort = 11,
                TimeStampForUpdate = new DateTime(2009, 01, 01)
            };
            task1.AddWorkEffortHistoryItem(wehi_1_1);

            var wehi_1_2 = new WorkEffortHistoryItem()
            {
                RemainingWorkEffort = 12,
                TimeStampForUpdate = new DateTime(2009, 01, 02)
            };
            task1.AddWorkEffortHistoryItem(wehi_1_2);

            var wehi_2_1 = new WorkEffortHistoryItem()
            {
                RemainingWorkEffort = 21,
                TimeStampForUpdate = new DateTime(2009, 02, 01)
            };
            task2.AddWorkEffortHistoryItem(wehi_2_1);

            var wehi_2_2 = new WorkEffortHistoryItem()
            {
                RemainingWorkEffort = 22,
                TimeStampForUpdate = new DateTime(2009, 02, 02)
            };
            task2.AddWorkEffortHistoryItem(wehi_2_2);

            #endregion
        }

        [Test]
        [Ignore]
        public void Assure_entire_hierarcy_is_correctly_persisted()
        {
            var persister = new ProjectInfoServerDatabaseRepository(sessionFactory);
            persister.Save(server);

            RecreateSessionFactory();
            var projectInfoServers = DatabaseRetriever.GetDatamodelFromDatabase<ProjectInfoServer>(sessionFactory);

            projectInfoServers.Count.ShouldBe(1);
            projectInfoServers[0].Name.ShouldBe(server.Name);

            var piServer = projectInfoServers.First();
            piServer.Projects.Count().ShouldBe(2);

            var project1 = piServer.Projects.ElementAt(0);
            project1.Name.ShouldBe(TESTPROJECTNAME_ONE);
            project1.Server.ShouldBe(piServer);
            project1.Iterations.Count().ShouldBe(1);

            var project2 = piServer.Projects.ElementAt(1);
            project2.Name.ShouldBe(TESTPROJECTNAME_TWO);
            project2.Server.ShouldBe(piServer);
            project2.Iterations.Count().ShouldBe(1);

            var iteration1 = project1.Iterations.ElementAt(0);
            iteration1.Name.ShouldBe(TESTITERATIONNAME_ONE);
            iteration1.Project.ShouldBe(project1);
            iteration1.Tasks.Count().ShouldBe(1);
            iteration1.Tasks.ElementAt(0).Iteration.ShouldBe(iteration1);

            var iteration2 = project2.Iterations.ElementAt(0);
            iteration2.Name.ShouldBe(TESTITERATIONNAME_TWO);
            iteration2.Project.ShouldBe(project2);
            iteration2.Tasks.Count().ShouldBe(1);
            iteration2.Tasks.ElementAt(0).Iteration.ShouldBe(iteration2);

            var task1 = iteration1.Tasks.ElementAt(0);
            task1.Name.ShouldBe(TESTTASKNAME_ONE);
            task1.Iteration.ShouldBe(iteration1);
            task1.WorkEffortHistory.Count().ShouldBe(2);
            task1.WorkEffortHistory.ElementAt(0).Task.ShouldBe(task1);

            var task2 = iteration2.Tasks.ElementAt(0);
            task2.Name.ShouldBe(TESTTASKNAME_TWO);
            task2.Iteration.ShouldBe(iteration2);
            task2.WorkEffortHistory.Count().ShouldBe(2);
            task2.WorkEffortHistory.ElementAt(0).Task.ShouldBe(task2);

            var whei_1_1 = task1.WorkEffortHistory.ElementAt(0);
            whei_1_1.Task.ShouldBe(task1);
            var whei_1_2 = task1.WorkEffortHistory.ElementAt(1);
            whei_1_2.Task.ShouldBe(task1);

            var whei_2_1 = task2.WorkEffortHistory.ElementAt(0);
            whei_2_1.Task.ShouldBe(task2);
            var whei_2_2 = task2.WorkEffortHistory.ElementAt(1);
            whei_2_2.Task.ShouldBe(task2);
        }

        [Test]
        public void Assure_identical_Iterations_in_different_projects_are_distinct_in_database()
        {
            var persister = new ProjectInfoServerDatabaseRepository(sessionFactory);
            persister.Save(server);

            RecreateSessionFactory();

            var projectInfoServers =
                DatabaseRetriever.GetDatamodelFromDatabase<ProjectInfoServer>(sessionFactory);

            var CommonIterationInProject1 = projectInfoServers[0].Projects.ElementAt(0).Iterations.Last();
            var CommonIterationInProject2 = projectInfoServers[0].Projects.ElementAt(1).Iterations.Last();

            CommonIterationInProject1.ShouldNotBe(CommonIterationInProject2);
        }

        [Test]
        public void Assure_identical_Tasks_in_different_projects_are_distinct_in_database()
        {
            var persister = new ProjectInfoServerDatabaseRepository(sessionFactory);
            persister.Save(server);

            RecreateSessionFactory();

            var projectInfoServers =
                DatabaseRetriever.GetDatamodelFromDatabase<ProjectInfoServer>(sessionFactory);

            var CommonTaskInProject1 =
                projectInfoServers[0].Projects.ElementAt(0).Iterations.ElementAt(0).Tasks.Last();
            var CommonTaskInProject2 =
                projectInfoServers[0].Projects.ElementAt(1).Iterations.ElementAt(0).Tasks.Last();

            CommonTaskInProject1.ShouldNotBe(CommonTaskInProject2);
        }

        [Test]
        public void Assure_identical_WorkEffortHistoryItem_in_different_projects_are_distinct_in_database()
        {
            var persister = new ProjectInfoServerDatabaseRepository(sessionFactory);
            persister.Save(server);

            RecreateSessionFactory();

            var projectInfoServers =
                DatabaseRetriever.GetDatamodelFromDatabase<ProjectInfoServer>(sessionFactory);

            var CommonWorkEffortHistoryItemInProject1 =
                projectInfoServers[0].Projects.ElementAt(0).Iterations.ElementAt(0).Tasks.ElementAt(0).
                    WorkEffortHistory.Last();
            var CommonWorkEffortHistoryItemInProject2 =
                projectInfoServers[0].Projects.ElementAt(1).Iterations.ElementAt(0).Tasks.ElementAt(0).
                    WorkEffortHistory.Last();

            CommonWorkEffortHistoryItemInProject1.ShouldNotBe(CommonWorkEffortHistoryItemInProject2);
        }
    }

}
