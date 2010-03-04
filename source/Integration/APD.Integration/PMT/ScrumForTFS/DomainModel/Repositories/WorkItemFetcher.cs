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


        public List<Task> GetAllCurrentItemsInSprint()
        {
            var wiqlQuery =
                @"SELECT [Conchango.TeamSystem.Scrum.EstimatedEffort], " +
                @"[Conchango.TeamSystem.Scrum.WorkRemaining] " +
                @"FROM [WorkItems] " +
                @"WHERE [System.TeamProject] = '" + ProjectName + "'" +
                @"AND [System.IterationPath] = '" + IterationPath + "'" +
                @"AND [Work Item Type] = 'Sprint Backlog Item'";
            return ConvertWICollectionToTaskList(workItemStore.Query(wiqlQuery));
        }


        public List<Task> GetAllRevisionsForAllItemsInSprint()
        {
            List<Task> allTasks = new List<Task>();
            var wiqlQuery =
                @"SELECT [Conchango.TeamSystem.Scrum.EstimatedEffort], " +
                @"[Conchango.TeamSystem.Scrum.WorkRemaining] " +
                @"FROM [WorkItems] " +
                @"WHERE [System.TeamProject] = '" + ProjectName + "'" +
                @"AND [System.IterationPath] = '" + IterationPath + "'" +
                @"AND [Work Item Type] = 'Sprint Backlog Item'";

            var allWorkItems = workItemStore.Query(wiqlQuery);

            List<WorkItem> allWorkItemRevisions = GetAllWorkItemRevisions(allWorkItems);

            return ConvertWICollectionToTaskList(allWorkItemRevisions);
        }


        // TODO: Performance
        private List<WorkItem> GetAllWorkItemRevisions(WorkItemCollection allWorkItems) {
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


        private static List<Task> ConvertWICollectionToTaskList(IEnumerable workItems)
        {
            var tasks = new List<Task>();

            foreach (WorkItem workItem in workItems)
            {
                // These are specific to Conchango's Scrum for Team System template.
                var remainingWork = ParseFieldToInt(workItem.Fields["Conchango.TeamSystem.Scrum.WorkRemaining"]);
                var estimatedWork = ParseFieldToInt(workItem.Fields["Conchango.TeamSystem.Scrum.EstimatedEffort"]);

                // TODO: We have at least one corner case here.. What if someone changes a name as part of a revision?
                if (!tasks.Select((t) => t.Name).Contains(workItem.Title))
                {
                    var task = new Task()
                    {
                        SystemId = workItem.Id.ToString(),
                        Name = workItem.Title,
                        Status = workItem.State
                    };

                    task.WorkEffortEstimate = estimatedWork;
                    tasks.Add(task);
                }

                var taskToAddTo = tasks.Where((t) => t.Name == workItem.Title).First();
                taskToAddTo.AddWorkEffortHistoryItem(
                     new WorkEffortHistoryItem(remainingWork, workItem.ChangedDate));
                taskToAddTo.WorkEffortEstimate = estimatedWork;
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
