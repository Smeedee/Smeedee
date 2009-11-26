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
using System.Net;

using APD.DomainModel.ProjectInfo;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;


namespace APD.Integration.PMT.TFS.DomainModel.Repositories
{
    public class WorkItemFetcher
    {
        private readonly TeamFoundationServer tfsServer;
        private readonly WorkItemStore workItemStore;

        public String ProjectName { get; private set; }

        public WorkItemFetcher(String serverAddress, String projectName, ICredentials credentials)
        {
            ProjectName = projectName;

            tfsServer = new TeamFoundationServer(serverAddress, credentials);
            tfsServer.Authenticate();

            workItemStore = tfsServer.GetService(typeof (WorkItemStore)) as WorkItemStore;
        }


        public WorkItemCollection GetAllWorkItems()
        {
            var wiqlQuery =
                @"SELECT [Completed Work], [Remaining Work] " +
                @"FROM WorkItems " +
                @"WHERE [System.TeamProject] = '" + ProjectName + "'" +
                @"AND [System.State] = 'Active'";
            return workItemStore.Query(wiqlQuery);
        }

        public WorkItemCollection GetWorkItemsByIteration(String iteration)
        {
            var wiqlQuery =
                @"SELECT [Completed Work], [Remaining Work] " +
                @"FROM WorkItems " +
                @"WHERE [System.TeamProject] = '" + ProjectName + "' " +
                @"AND [System.State] = 'Active'" +
                @"AND [System.IterationPath] = '" + ProjectName + @"\" + iteration + "'";
            return workItemStore.Query(wiqlQuery);
        }

        public WorkItemCollection GetAllWorkItemsAsOfDate(DateTime dateTime)
        {
            var wiqlQuery =
                @"SELECT [Completed Work], [Remaining Work] " +
                @"FROM WorkItems " +
                @"WHERE [System.TeamProject] = '" + ProjectName + "' " +
                @"AND [System.State] = 'Active'" +
                @"ASOF '" + dateTime.ToString("dd/MM/yyyy HH:mm") + "'";
            return workItemStore.Query(wiqlQuery);
        }

        public WorkItemCollection GetWorkItemsByIterationAsOfDate(String iteration, DateTime dateTime)
        {
            var wiqlQuery =
                @"SELECT [Completed Work], [Remaining Work] " +
                @"FROM WorkItems " +
                @"WHERE [System.TeamProject] = '" + ProjectName + "' " +
                @"AND [System.IterationPath] = '" + ProjectName + @"\" + iteration + "' " +
                @"AND [System.State] = 'Active'" +
                @"ASOF '" + dateTime.ToString("dd/MM/yyyy HH:mm") + "'";
            return workItemStore.Query(wiqlQuery);
        }

        public List<Task> ConvertWorkItemCollectionToTaskList(WorkItemCollection workItems)
        {
            return ConvertWorkItemCollectionToTaskListAsOfDate(workItems, DateTime.Now);
        }

        public List<Task> ConvertWorkItemCollectionToTaskListAsOfDate(WorkItemCollection workItems, DateTime date)
        {
            var tasks = new List<Task>();
            date = date.Date; // Removes H:M:S

            foreach (var item in workItems)
            {
                var workItem = (WorkItem) item;
                var task = new Task()
                {
                    SystemId = workItem.Id.ToString(),
                    Name = workItem.Title,
                    Status = workItem.State
                };

                var completedWork = ParseFieldToInt(workItem.Fields["Completed Work"]);
                var remainingWork = ParseFieldToInt(workItem.Fields["Remaining Work"]);

                task.WorkEffortEstimate = completedWork + remainingWork;

                var taskRemainingWork = new WorkEffortHistoryItem(remainingWork, date);
                task.AddWorkEffortHistoryItem(taskRemainingWork);

                tasks.Add(task);
            }

            return tasks;
        }

        private static int ParseFieldToInt(Field field)
        {
            if (field.Value != null)
            {
                return Int32.Parse(field.Value.ToString());
            }
            return 0;
        }
    }
}