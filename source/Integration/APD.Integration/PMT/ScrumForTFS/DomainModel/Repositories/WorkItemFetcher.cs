using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using APD.DomainModel.ProjectInfo;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System.Net;
using System.Collections;


namespace APD.Integration.PMT.ScrumForTFS.DomainModel.Repositories
{
    public class WorkItemFetcher
    {
        private readonly TeamFoundationServer tfsServer;
        private readonly WorkItemStore workItemStore;
        // TODO: Introduce settings for these.
        private const string WORK_REMAINING_FIELD = "Conchango.TeamSystem.Scrum.WorkRemaining";
        private const string ESTIMATED_EFFORT_FIELD = "Conchango.TeamSystem.Scrum.EstimatedEffort";

        public String ProjectName { get; private set; }
        public String IterationPath { get; private set; }

        
        public WorkItemFetcher(String serverAddress, String projectName, String iterationPath, ICredentials credentials)
        {
            ProjectName = projectName;
            IterationPath = iterationPath;

            tfsServer = new TeamFoundationServer(serverAddress, credentials);
            tfsServer.Authenticate();

            workItemStore = tfsServer.GetService(typeof(WorkItemStore)) as WorkItemStore;
        }


        public List<Task> GetAllWorkEffortInSprint()
        {
            var allWorkItems = GetCurrentWorkItemsInSprint();

            List<WorkItem> allWorkItemRevisions = GetWorkItemRevisions(allWorkItems);

            return ConvertWorkItemsToTasks(allWorkItemRevisions);
        }

        public List<Task> GetCurrentTasksInSprint()
        {
            return ConvertWorkItemsToTasks(GetCurrentWorkItemsInSprint());
        }


        private WorkItemCollection GetCurrentWorkItemsInSprint()
        {
            var wiqlQuery =
                @"SELECT [Conchango.TeamSystem.Scrum.EstimatedEffort], " +
                @"[Conchango.TeamSystem.Scrum.WorkRemaining] " +
                @"FROM [WorkItems] " +
                @"WHERE [System.TeamProject] = '" + ProjectName + "'" +
                @"AND [System.IterationPath] = '" + IterationPath + "'" +
                @"AND [Work Item Type] = 'Sprint Backlog Item'";
            return workItemStore.Query(wiqlQuery);
        }


        // TODO: Performance
        private List<WorkItem> GetWorkItemRevisions(WorkItemCollection allWorkItems)
        {
            List<WorkItem> allWorkItemRevisions = new List<WorkItem>();


            for (int i = 0; i < allWorkItems.Count; i++)
            {
                var item = allWorkItems[i];
                for (int j = 0; j < item.Revisions.Count; j++)
                {
                    var revision = item.Revisions[j];
                    var itemRevision = workItemStore.GetWorkItem(item.Id, (int)revision.Fields[CoreField.Rev].Value);
                    allWorkItemRevisions.Add(itemRevision);
                }
            }
            return allWorkItemRevisions;
        }


        private static List<Task> ConvertWorkItemsToTasks(IEnumerable workItems)
        {
            var tasks = new List<Task>();

            foreach (WorkItem workItem in workItems)
            {
                // These are specific to Conchango's Scrum for Team System template.
                var remainingWork = ParseFieldToInt(workItem.Fields[WORK_REMAINING_FIELD]);
                var estimatedWork = ParseFieldToInt(workItem.Fields[ESTIMATED_EFFORT_FIELD]);

                Console.WriteLine(workItem.Fields["Title"].Value);

                // TODO: We have at least one corner case here.. What if someone changes a name as part of a revision?
                if (!tasks.Select((t) => t.Name).Contains(workItem.Title))
                {
                    var task = new Task
                    {
                        SystemId = workItem.Id.ToString(),
                        Name = workItem.Title,
                        Status = workItem.State,
                        WorkEffortEstimate = estimatedWork
                    };

                    tasks.Add(task);
                }

                var taskToAddTo = tasks.Where((t) => t.Name == workItem.Title).First();
                taskToAddTo.AddWorkEffortHistoryItem(
                    new WorkEffortHistoryItem(remainingWork, workItem.ChangedDate));
                taskToAddTo.WorkEffortEstimate = estimatedWork;
            }

            return tasks;
        }


        public List<Task> GetAllCurrentTasks()
        {
            return ConvertWorkItemsToTasks(GetAllCurrentWorkItems());
        }

        public List<Task> GetAllWorkEffort()
        {
            var allWorkItemRevisions = GetWorkItemRevisions(GetAllCurrentWorkItems());

            return ConvertWorkItemsToTasks(allWorkItemRevisions);
        }

        private WorkItemCollection GetAllCurrentWorkItems()
        {
            var wiqlQuery =
                @"SELECT [Conchango.TeamSystem.Scrum.EstimatedEffort], " +
                @"[Conchango.TeamSystem.Scrum.WorkRemaining] " +
                @"FROM [WorkItems] " +
                @"WHERE [System.TeamProject] = '" + ProjectName + "'" +
                @"AND [Work Item Type] = 'Sprint Backlog Item'";
            return workItemStore.Query(wiqlQuery);
        }

        private static int ParseFieldToInt(Field field)
        {
            return field.Value != null ? Int32.Parse(field.Value.ToString()) : 0;
        }
    }
}
