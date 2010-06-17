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
// /copyright> 
// 
// <contactinfo>
// The project webpage is located at http://agileprojectdashboard.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Smeedee.DomainModel.Config;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.Tasks.Framework;
using Smeedee.Tasks.Framework.Factories;
using Smeedee.Tasks.ProjectInfo;
using TinyBDD.Dsl.GivenWhenThen;
using TinyBDD.Specification.NUnit;


namespace Smeedee.TaskTests.ProjectInfo
{
    public class Shared
    {
        protected static ProjectInfoTask Task;
        protected static Mock<IPersistDomainModels<ProjectInfoServer>> databasePersister;
        protected static Mock<IRepository<ProjectInfoServer>> projectRepository;
        protected static Mock<IRepository<Configuration>> configRepoMock;
        protected static Mock<IAssembleRepository<ProjectInfoServer>> repositoryFactoryMock;
        protected static List<ProjectInfoServer> SavedProjectInfoServers;

        protected Context a_ProjectInfoTask_is_created = () =>
        {
            databasePersister = new Mock<IPersistDomainModels<ProjectInfoServer>>();
            projectRepository = new Mock<IRepository<ProjectInfoServer>>();
            configRepoMock = new Mock<IRepository<Configuration>>();
            repositoryFactoryMock = new Mock<IAssembleRepository<ProjectInfoServer>>();

            repositoryFactoryMock.Setup(r => r.Assemble(It.IsAny<Configuration>())).
                Returns(projectRepository.Object);
 
            databasePersister.Setup(d => d.Save(It.IsAny<ProjectInfoServer>())).Callback(
                (ProjectInfoServer projectInfoServer) => SavedProjectInfoServers.Add(projectInfoServer));

            var configs = new List<Configuration>();
            var piConfig = new Configuration();
            piConfig.NewSetting("provider", "conchango-tfs");
            configs.Add(piConfig);

            configRepoMock.Setup(r => r.Get(It.IsAny<Specification<Configuration>>())).
                Returns(configs);

            Task = new ProjectInfoTask(repositoryFactoryMock.Object, databasePersister.Object, configRepoMock.Object);
        };

        protected Context there_are_projects_in_repository = () =>
        {
            projectRepository.Setup(p => p.Get(new AllSpecification<ProjectInfoServer>())).Returns(GetMockProjects());
        };

        protected When Task_is_dispatched = () =>
        {
            Task.Execute();
        };

        #region Mock Project
        protected static List<ProjectInfoServer> GetMockProjects()
        {
            var iterationStartDate = new DateTime(2009, 7, 28);
            var iterationEndDate = new DateTime(2009, 8, 9);

            var task1 = new Task
            {
                Status = "Defined",
                Name = "Task1",
                WorkEffortEstimate = 10,
                SystemId = "t1 sys ID"
            };
            var task2 = new Task
            {
                Status = "In-Progress",
                Name = "Task2",
                WorkEffortEstimate = 20,
                SystemId = "t2 sys ID"
            };
            var task3 = new Task
            {
                Status = "In-Progress",
                Name = "Task3",
                WorkEffortEstimate = 30,
                SystemId = "t3 sys ID"
            };
            var task4 = new Task
            {
                Status = "Completed",
                Name = "Task4",
                WorkEffortEstimate = 40,
                SystemId = "t4 sys ID"
            };

            List<Task> tasks = new List<Task>();
            tasks.Add(task1);
            tasks.Add(task2);
            tasks.Add(task3);
            tasks.Add(task4);

            tasks[0].AddWorkEffortHistoryItem(new WorkEffortHistoryItem(2, new DateTime(2009, 7, 31)));
            tasks[1].AddWorkEffortHistoryItem(new WorkEffortHistoryItem(5, new DateTime(2009, 7, 31)));
            tasks[2].AddWorkEffortHistoryItem(new WorkEffortHistoryItem(15, new DateTime(2009, 7, 31)));
            tasks[3].AddWorkEffortHistoryItem(new WorkEffortHistoryItem(25, new DateTime(2009, 7, 31)));

            Iteration iteration = new Iteration
            {
                EndDate = iterationEndDate,
                Name = "Sprint 3",
                StartDate = iterationStartDate,
                SystemId = "iteration sys ID"
            };
            iteration.Tasks = tasks;

            Project mockProject = new Project
            {
                Name = "Smeedee",
                SystemId = "project sys ID"
            };

            mockProject.AddIteration(iteration);

            var server = new ProjectInfoServer("Mock server", "http://mock.url");
            server.AddProject(mockProject);
            
            return new List<ProjectInfoServer>{server};
        }
        #endregion
    }

    [TestFixture]
    public class when_created : Shared
    {
        [SetUp]
        public void SetUp()
        {
            SavedProjectInfoServers = new List<ProjectInfoServer>();
        }

        [Test]
        public void should_be_instanced()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ProjectInfoTask_is_created);
                scenario.When("instance is accessed");
                scenario.Then("instance excists", () => Task.ShouldNotBeNull());
            });
        }

        [Test]
        public void should_be_instance_of_AbstractTask()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ProjectInfoTask_is_created);
                scenario.When("instance is accessed");
                scenario.Then("the Task is an instance of TaskBase", () =>
                    Task.ShouldBeInstanceOfType<TaskBase>());
            });
        }

        [Test]
        public void should_have_correct_name()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ProjectInfoTask_is_created);
                scenario.When("instance is accessed");
                scenario.Then("the Task has name 'ProjectInfo Task'", () =>
                    {
                        Task.Name.ShouldBe("ProjectInfo Task");
                    });
            });
        }

        [Test]
        public void should_have_correct_time_span_interval()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ProjectInfoTask_is_created);
                scenario.When("instance is accessed");
                scenario.Then("the Task has time span of 20 min", () =>
                {
                    Task.Interval.ShouldBe(new TimeSpan(0, 20, 0));
                });
            });
        }
    }

    [TestFixture]
    public class when_dispatched : Shared
    {
        [SetUp]
        public void SetUp()
        {
            SavedProjectInfoServers = new List<ProjectInfoServer>();
        }

        [Test]
        public void assure_will_save_all_projects_in_repository()
        {
            Scenario.StartNew(this, scenario =>
            {
                scenario.Given(a_ProjectInfoTask_is_created).
                    And(there_are_projects_in_repository);
                scenario.When(Task_is_dispatched);
                scenario.Then("projects are added to the database",()=>
                {
                    foreach (var project in GetMockProjects())
                        SavedProjectInfoServers.Contains(project);
                });
            });
        }
    }
}