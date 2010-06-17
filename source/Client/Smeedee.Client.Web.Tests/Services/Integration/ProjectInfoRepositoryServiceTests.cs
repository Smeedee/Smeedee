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
// The project webpage is located at http://smeedee.org/
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
using NUnit.Framework;
using Smeedee.Client.Web.Tests.ProjectInfoRepositoryService;
using Smeedee.DomainModel.Framework;
using Smeedee.DomainModel.ProjectInfo;
using TinyBDD.Specification.NUnit;
using TinyBDD.Dsl.GivenWhenThen;


namespace Smeedee.Client.WebTests.Services.Integration.ProjectInfoRepositoryServiceTests
{
    public class ProjectInfoRepositoryServiceTestsShared : Shared
    {
        protected static ProjectInfoRepositoryServiceClient webServiceClient;

        protected Context WebServiceClient_is_created = () =>
        {
            webServiceClient = new ProjectInfoRepositoryServiceClient();
        };

        protected Context Database_contains_data = () =>
        {
            var projectInfoServer = new ProjectInfoServer()
            {
                Name = "RallyDev",
                Url = "http://community.rallydev.com"
            };

            var project = new Project()
            {
                Name = "Smeedee",
                SystemId = "Smeedee"
            };
            projectInfoServer.AddProject(project);

            var iteration = new Iteration()
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                Name = "SPRINT 3",
                SystemId = "SPRINT 3"
            };
            project.AddIteration(iteration);

            var task = new Task()
            {
                Name = "Task 1",
                Status = "Done",
                SystemId = "Task 1",
                WorkEffortEstimate = 10
            };
            task.AddWorkEffortHistoryItem(new WorkEffortHistoryItem()
            {
                RemainingWorkEffort = 9,
                TimeStampForUpdate = DateTime.Now
            });
            task.AddWorkEffortHistoryItem(new WorkEffortHistoryItem()
            {
                RemainingWorkEffort = 8,
                TimeStampForUpdate = DateTime.Now.AddHours(1)
            });
            iteration.AddTask(task);

            var task2 = new Task()
            {
                Name = "Task 2",
                Status = "Done",
                SystemId = "Task 2",
                WorkEffortEstimate = 5
            };
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem()
            {
                RemainingWorkEffort = 5,
                TimeStampForUpdate = DateTime.Now.AddHours(2)
            });
            iteration.AddTask(task2);

            databaseSession.Save(projectInfoServer);
            databaseSession.Flush();
        };
    }

    [TestFixture]
    public class When_get_ProjectInfo_data_via_WebService : ProjectInfoRepositoryServiceTestsShared
    {
        [Test]
        public void Assure_all_data_is_successfully_serialized()
        {
            Scenario.StartNew(this, scenario =>
            {
                IEnumerable<ProjectInfoServer> resultsetWS = null;
                IEnumerable<ProjectInfoServer> resultsetDB = null;

                scenario.Given(Database_is_created).
                    And(Database_contains_data).
                    And(WebServiceClient_is_created).
                    And("ProjectInfo data is fetched from database", () =>
                        resultsetDB = databaseSession.CreateCriteria(typeof(ProjectInfoServer)).List<ProjectInfoServer>());

                try
                {
                    scenario.When("get all", () =>
                   resultsetWS = webServiceClient.Get(new AllSpecification<ProjectInfoServer>()));
                }
                catch (Exception)
                {
                    throw;
                }

                scenario.Then("assure all data is successfully serialized", () =>
                {
                    AssertProjectInfoServers(resultsetWS, resultsetDB);

                    foreach (var piServerDB in resultsetDB)
                    {
                        var piServerWS = resultsetWS.Where(r => r.Url == piServerDB.Url).SingleOrDefault();
                        AssertProjectInfoServer(piServerWS, piServerDB);

                        foreach (var projectDB in piServerDB.Projects)
                        {
                            var projectWS = piServerWS.Projects.Where(r => r.SystemId == projectDB.SystemId).SingleOrDefault();

                            AssertProject(projectWS, projectDB);

                            foreach (var iterationDB in projectDB.Iterations)
                            {
                                var iterationWS =
                                    projectWS.Iterations.Where(r => r.SystemId == iterationDB.SystemId).
                                        SingleOrDefault();

                                AssertIteration(iterationWS, iterationDB);

                                foreach (var taskDB in iterationDB.Tasks)
                                {
                                    var taskWS =
                                        iterationWS.Tasks.Where(r => r.SystemId == taskDB.SystemId).
                                            SingleOrDefault();

                                    AssertTask(taskWS, taskDB);

                                    foreach (var taskHistoryItemDB in taskDB.WorkEffortHistory)
                                    {
                                        Console.WriteLine(taskHistoryItemDB.TimeStampForUpdate);
                                        var taskHistoryItemWS =
                                            taskWS.WorkEffortHistory.Where(r => r.Equals(taskHistoryItemDB)).
                                                SingleOrDefault();

                                        AssertTaskHistoryItem(taskHistoryItemWS, taskHistoryItemDB);
                                    }
                                }
                            }
                        }
                    }
                });
            });
        }

        private void AssertTaskHistoryItem(WorkEffortHistoryItem actual, WorkEffortHistoryItem expected)
        {
            Console.WriteLine("AssertTaskHistoryItem");

            actual.ShouldNotBeNull();
            //Reason for ToString is that SQLite doesnt have millisecond precition DateTime values
            actual.TimeStampForUpdate.ToString(DATE_FORMAT).
                ShouldBe(expected.TimeStampForUpdate.ToString(DATE_FORMAT));
            actual.RemainingWorkEffort.ShouldBe(expected.RemainingWorkEffort);

            actual.Task.ShouldNotBeNull();
            actual.Task.SystemId.ShouldBe(expected.Task.SystemId);
        }

        private void AssertTask(Task actual, Task expected)
        {
            Console.WriteLine("AssertTask");

            actual.ShouldNotBeNull();

            actual.Iteration.ShouldNotBeNull();
            actual.Iteration.SystemId.ShouldBe(expected.Iteration.SystemId);

            actual.SystemId.ShouldBe(expected.SystemId);
            actual.Status.ShouldBe(expected.Status);
            actual.Name.ShouldBe(expected.Name);
            actual.WorkEffortEstimate.ShouldBe(expected.WorkEffortEstimate);
        }

        private void AssertIteration(Iteration actual, Iteration expected)
        {
            Console.WriteLine("AssertIteration");

            actual.ShouldNotBeNull();
            actual.Project.ShouldNotBeNull();
            actual.Project.SystemId.ShouldBe(expected.Project.SystemId);
            actual.StartDate.ToString(DATE_FORMAT).
                ShouldBe(expected.StartDate.ToString(DATE_FORMAT));
            actual.EndDate.ToString(DATE_FORMAT).
                ShouldBe(expected.EndDate.ToString(DATE_FORMAT));
            actual.Name.ShouldBe(actual.Name);
            actual.SystemId.ShouldBe(expected.SystemId);

            actual.Tasks.ShouldNotBeNull();
            actual.Tasks.Count().ShouldBe(expected.Tasks.Count());


        }

        private void AssertProject(Project actual, Project expected)
        {
            Console.WriteLine("AssertProject");

            actual.ShouldNotBeNull();
            actual.Name.ShouldBe(expected.Name);
            actual.Server.ShouldNotBeNull();
            actual.Server.Name.ShouldBe(expected.Server.Name);
            actual.Iterations.ShouldNotBeNull();
            actual.Iterations.Count().ShouldBe(expected.Iterations.Count());
            actual.CurrentIteration.ShouldBe(expected.CurrentIteration);
        }

        private void AssertProjectInfoServer(ProjectInfoServer piServerWS, ProjectInfoServer piServerDB)
        {
            Console.WriteLine("AssertProjectInfoServer");

            piServerWS.ShouldNotBeNull();
            piServerWS.Name.ShouldBe(piServerDB.Name);
            piServerWS.Url.ShouldBe(piServerDB.Url);
        }

        private void AssertProjectInfoServers(IEnumerable<ProjectInfoServer> actual, IEnumerable<ProjectInfoServer> expected)
        {
            Console.WriteLine("AssertProjectInfoServers");

            actual.ShouldNotBeNull();
            expected.ShouldNotBeNull();

            actual.Count().ShouldBe(expected.Count());
        }
    }
}
