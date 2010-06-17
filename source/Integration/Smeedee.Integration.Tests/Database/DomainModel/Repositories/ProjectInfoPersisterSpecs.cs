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
using Smeedee.DomainModel.ProjectInfo;
using NUnit.Framework;
using TinyBDD.Specification.NUnit;
using Smeedee.Integration.Database.DomainModel.Repositories;


namespace Smeedee.IntegrationTests.Database.DomainModel.Repositories.ProjectInfoPersisterSpecs
{
    [TestFixture]
    public class when_saving : Shared
    {
        private const string TEST_TASK_NAME = "Perform test";
        private const string TEST_ITERATION_NAME = "Test Iteration";
        private const string TEST_PROJECT_NAME_ONE = "Test Project";
        private const string TEST_PROJECT_NAME_TWO = "Test Project number two";

        [SetUp]
        public void Setup()
        {
            DeleteDatabaseIfExists();
            RecreateSessionFactory();
        }

        [Test]
        [Ignore]
        public void Assure_all_ProjectInfos_are_saved()
        {
            var projectInfoPersister = new ProjectInfoDatabaseRepository(sessionFactory);

            var workEffortHistoryItem = new WorkEffortHistoryItem(30, DateTime.Now);

            var server = new ProjectInfoServer() {Name = "RallyDev", Url = "http://community.rally.com"};

            var testProject = new Project(TEST_PROJECT_NAME_ONE)
            {
                SystemId = "1",
                Server = server
            };

            var testIteration = new Iteration
            {
                SystemId = "3",
                Name = TEST_ITERATION_NAME,
                StartDate = new DateTime(2009, 01, 01),
                EndDate = new DateTime(2009, 01, 14),
                Project = testProject
            };
            testProject.AddIteration(testIteration);

            var testTask = new Task
                               {
                                   SystemId = "31224",
                                   Name = TEST_TASK_NAME,
                                   Status = "In progress",
                                   WorkEffortEstimate = 40.0f,
                                   Iteration = testIteration
                               };

            workEffortHistoryItem.Task = testTask;
            testTask.AddWorkEffortHistoryItem(workEffortHistoryItem);
            testIteration.AddTask(testTask);

            projectInfoPersister.Save(testProject);

            RecreateSessionFactory();
            var retrievedProjects = DatabaseRetriever.GetDatamodelFromDatabase<Project>(sessionFactory);
            
            Console.WriteLine("Project:");
            retrievedProjects.Count.ShouldBe(1);
            retrievedProjects[0].Name.ShouldBe(TEST_PROJECT_NAME_ONE);
            Console.WriteLine("Iteration:");
            retrievedProjects[0].GetIterationCount().ShouldBe(1);
            retrievedProjects[0].CurrentIteration.Name.ShouldBe(TEST_ITERATION_NAME);
            Console.WriteLine("Task:");
            retrievedProjects[0].CurrentIteration.GetTaskCount().ShouldBe(1);

            Task tempTask = null;
            foreach (Task task_ in retrievedProjects[0].CurrentIteration.Tasks)
            {
                tempTask = task_;
            }
            tempTask.Name.ShouldBe(TEST_TASK_NAME);
        }
    }
}
