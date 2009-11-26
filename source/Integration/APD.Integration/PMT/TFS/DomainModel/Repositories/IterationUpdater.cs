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
using System.Linq;
using System.Linq.Expressions;
using System.Net;

using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;
using APD.DomainModel.ProjectInfo.Repositories;

using Microsoft.TeamFoundation.WorkItemTracking.Client;

using Project=APD.DomainModel.ProjectInfo.Project;


// SRP!
// Naming
namespace APD.Integration.PMT.TFS.DomainModel.Repositories
{
    public class IterationUpdater : IRepository<ProjectInfoServer>
    {
        private readonly WorkItemFetcher workItemFetcher;
        private readonly string projectName;    

        public IterationUpdater(string serverAddress, string projectName, ICredentials credentials)
        {
            this.projectName = projectName;
            workItemFetcher = new WorkItemFetcher(serverAddress, projectName, credentials);
        }

        public Iteration UpdateIteration(Iteration iteration)
        {
            var workItems = workItemFetcher.GetWorkItemsByIteration(iteration.Name);
            var tasksFromTfs = workItemFetcher.ConvertWorkItemCollectionToTaskList(workItems);

            tasksFromTfs.ForEach(t => t.Iteration = iteration);

            var oldTasks = new List<Task>();
            oldTasks.AddRange(iteration.Tasks);

            IList<Task> mergedTasks = IterationUtilities.MergeTasks(oldTasks, tasksFromTfs);
            iteration.Tasks = mergedTasks;

            return iteration;
        }

        public Iteration UpdateIterationByDate(Iteration iteration, DateTime date)
        {
            var workItems = workItemFetcher.GetWorkItemsByIterationAsOfDate(iteration.Name, date);
            var tasksFromTfs = workItemFetcher.ConvertWorkItemCollectionToTaskListAsOfDate(workItems, date);

            tasksFromTfs.ForEach(t => t.Iteration = iteration);

            var oldTasks = new List<Task>();
            oldTasks.AddRange(iteration.Tasks);

            var mergedTasks = IterationUtilities.MergeTasks(oldTasks, tasksFromTfs);
            iteration.Tasks = mergedTasks;

            return iteration;
        }

        public Project Get(string iterationName, DateTime startDate, DateTime endDate)
        {
            var server = new ProjectInfoServer("sName", "url");

            var project = new Project {Name = projectName, SystemId = "123"};

            var iteration = new Iteration(startDate, endDate, new HolidayProvider()) {Name = iterationName, SystemId = "12"};

            endDate = ConvertDateToTodayIfDateInFuture(endDate);
            var dates = new List<DateTime>();
            var numberOfDays = (int) Math.Ceiling(( endDate - startDate ).TotalDays);

            for (var i = 0; i < numberOfDays; i++)
                dates.Add(startDate.AddDays(i));

            foreach (var date in dates)
                iteration = UpdateIterationByDate(iteration, date);

            project.AddIteration(iteration);
            server.AddProject(project);
            return project;
        }

        public DateTime ConvertDateToTodayIfDateInFuture(DateTime date)
        {
            if (date > DateTime.Today)
            {
                return new DateTime(DateTime.Today.Year, 
                                    DateTime.Today.Month, 
                                    DateTime.Today.Day, 23, 59, 59);
            }
            return date;
        }

        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            // TODO: Add config
            var project = Get("Sprint 1", new DateTime(2009, 07, 28), new DateTime(2009, 08, 14));

            var server = new ProjectInfoServer("Mock Server", "http://agileprojectdashboard.org/projectinfo");
            server.AddProject(project);

            return new List<ProjectInfoServer>() {server};
        }
    }
}