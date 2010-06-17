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
// The project webpage is located at http://smeedee.org/
// which contains all the neccessary information.
// </contactinfo>

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Smeedee.DomainModel.ProjectInfo;
using Smeedee.DomainModel.ProjectInfo.Repositories;
using NUnit.Framework;

using TinyBDD.Specification.NUnit;


namespace Smeedee.DomainModel.ProjectInfoTest.Repositories
{
    [TestFixture]
    public class IterationUtilitiesSpecs
    {
        private IList<Task> oldTasks;
        private IList<Task> newTasks;

        [SetUp]
        public void SetUp()
        {
            oldTasks = GetOldTasks();
            newTasks = GetNewTasks();
        }

        #region mock tasks
        private static IList<Task> GetOldTasks()
        {
            IList<Task> oldTasks = new List<Task>();

            Task task1 = new Task
            {
                Name = "Task 1",
                Status = "In progress",
                SystemId = "1",
                WorkEffortEstimate = 30
            };
            Task task2 = new Task
            {
                Name = "Task 2",
                Status = "In progress",
                SystemId = "2",
                WorkEffortEstimate = 20
            };

            task1.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(25, new DateTime(2009, 7, 6)));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(15, new DateTime(2009, 7, 6)));
            task1.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(20, new DateTime(2009, 7, 7)));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(10, new DateTime(2009, 7, 7)));
            oldTasks.Add(task1);
            oldTasks.Add(task2);

            SetParentsforTasks(oldTasks);

            return oldTasks;
        }

        private static IList<Task> GetNewTasks()
        {
            IList<Task> newTasks = new List<Task>();
            
            Task task2 = new Task
            {
                Name = "Task 2",
                Status = "Completed",
                SystemId = "2",
                WorkEffortEstimate = 20
            };
            Task task3 = new Task
            {
                Name = "Task 3",
                Status = "In progress",
                SystemId = "3",
                WorkEffortEstimate = 15
            };

            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(15, new DateTime(2009, 7, 6)));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(10, new DateTime(2009, 7, 7)));
            task2.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(0, new DateTime(2009, 7, 8)));
            task3.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(12, new DateTime(2009, 7, 8)));
            newTasks.Add(task2);
            newTasks.Add(task3);

            SetParentsforTasks(newTasks);

            return newTasks;
        }

        private static void SetParentsforTasks(IList<Task> tasks)
        {
            ProjectInfoServer server = new ProjectInfoServer();
            server.Url = "url";
            Project project = new Project();
            project.SystemId = "11";
            Iteration iteration = new Iteration();
            iteration.SystemId = "15";
            iteration.Tasks = tasks;
            project.AddIteration(iteration);
            server.AddProject(project);
        }
        #endregion

        [Test]
        public void Should_be_able_to_get_all_the_new_tasks()
        {
            IList<Task> theNewTasks = IterationUtilities.GetNewlyCreatedTasks(oldTasks, newTasks);

            Assert.AreEqual(theNewTasks.Count, 1);
            Assert.AreEqual(theNewTasks[0].Name, "Task 3");
            Assert.AreEqual(1, theNewTasks[0].WorkEffortHistory.Count());
        }

        [Test]
        public void Should_be_able_to_get_all_the_updated_tasks()
        {
            IList<Task> theUpdatedTasks = IterationUtilities.GetUpdatedTasks(oldTasks, newTasks);

            theUpdatedTasks.Count.ShouldBe(1);
            theUpdatedTasks[0].Name.ShouldBe("Task 2");
            theUpdatedTasks[0].Status.ShouldBe("Completed");
            theUpdatedTasks[0].WorkEffortHistory.Count().ShouldBe(3);
        }

        [Test]
        public void Should_be_able_to_get_all_the_removed_tasks()
        {
            IList<Task> theRemovedTasks = IterationUtilities.GetRemovedTasks(oldTasks, newTasks);

            theRemovedTasks.Count.ShouldBe(1);
            theRemovedTasks[0].Name.ShouldBe("Task 1");
            theRemovedTasks[0].WorkEffortHistory.Count().ShouldBe(2);
        }

        [Test]
        public void Should_be_able_to_merge_tasks_correctly()
        {
            IList<Task> theMergedTasks = IterationUtilities.MergeTasks(oldTasks, newTasks);

            Assert.AreEqual(theMergedTasks.Count, 3);
            Assert.AreEqual(2, theMergedTasks[0].WorkEffortHistory.Count());
            Assert.AreEqual(3, theMergedTasks[1].WorkEffortHistory.Count());
            Assert.AreEqual(1, theMergedTasks[2].WorkEffortHistory.Count());
        }
    }
}