using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Smeedee.DomainModel.ProjectInfo;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;


namespace Smeedee.Integration.PMT.ScrumForTFS.DomainModel.Repositories
{
    public class WorkItemFetcher : IFetchWorkItems
    {
        // Defaulting to the values used by Conchango's Scrum for Team System
        private readonly string WORK_REMAINING_FIELD = "Work Remaining (Scrum v3)";
        private readonly string ESTIMATED_EFFORT_FIELD = "Estimated Effort (Scrum v3)";
        
        private readonly TeamFoundationServer tfsServer;
        private readonly WorkItemStore workItemStore;

        private readonly string projectName;


        public WorkItemFetcher(String serverAddress, String projectName, ICredentials credentials,
                               Dictionary<String, String> configuration)
        {
            String configValue = "";
            WORK_REMAINING_FIELD = (configuration.TryGetValue("tfswi-remaining-field", out configValue))
                                       ? configValue
                                       : WORK_REMAINING_FIELD;
            ESTIMATED_EFFORT_FIELD = (configuration.TryGetValue("tfswi-estimated-field", out configValue))
                                         ? configValue
                                         : ESTIMATED_EFFORT_FIELD;

            this.projectName = projectName;

            tfsServer = new TeamFoundationServer(serverAddress, credentials);
            tfsServer.Authenticate();

            workItemStore = tfsServer.GetService(typeof (WorkItemStore)) as WorkItemStore;
        }

        #region IFetchWorkItems Members

        public List<Task> GetAllWorkEffortInSprint(string iterationPath)
        {
            var allWorkItems = GetCurrentWorkItemsInSprint(iterationPath);
            var allRevisions = GetWorkItemRevisions(allWorkItems);
            return ConvertWorkItemsToTasks(allRevisions);
        }

        public List<Task> GetAllWorkEffort()
        {
            var allWorkItemRevisions = GetWorkItemRevisions(GetAllCurrentWorkItems());
            return ConvertWorkItemsToTasks(allWorkItemRevisions);
        }

        public IEnumerable<String> GetAllIterations()
        {
            var allWorkItems = GetAllCurrentWorkItems();
            var iterationPaths = new List<String>();
            foreach (WorkItem item in allWorkItems)
            {
                if (!iterationPaths.Contains(item.IterationPath))
                {
                    iterationPaths.Add(item.IterationPath);
                }
            }
            return iterationPaths;
        }

        #endregion

        public List<Task> GetCurrentTasksInSprint(string iterationPath)
        {
            return ConvertWorkItemsToTasks(GetCurrentWorkItemsInSprint(iterationPath));
        }

        private WorkItemCollection GetCurrentWorkItemsInSprint(string iterationPath)
        {
            string wiqlQuery =
                @"SELECT [Estimated Effort (Scrum v3)], " +
                @"[Work Remaining (Scrum v3)] " +
                @"FROM [WorkItems] " +
                @"WHERE [Team Project] = '" + projectName + "'" +
                @"AND [Iteration Path] = '" + iterationPath + "'" +
                @"AND [Work Item Type] = 'Sprint Backlog Task'";
            return workItemStore.Query(wiqlQuery);
        }

        // NOTE: Horrible run time. We should probably look into other ways to talk with TFS.
        private List<WorkItem> GetWorkItemRevisions(WorkItemCollection allWorkItems)
        {
            var allWorkItemRevisions = new List<WorkItem>();

            for (int i = 0; i < allWorkItems.Count; i++)
            {
                var workItem = allWorkItems[i];
                for (int j = 0; j < workItem.Revisions.Count; j++)
                {
                    var revision = workItem.Revisions[j];
                    var revisionItem = workItemStore.GetWorkItem(workItem.Id, ParseFieldToInt(revision.Fields[CoreField.Rev]));
                    allWorkItemRevisions.Add(revisionItem);
                }
            }

            return allWorkItemRevisions;
        }

        private List<Task> ConvertWorkItemsToTasks(IEnumerable workItems)
        {
            var tasks = new List<Task>();

            foreach (WorkItem workItem in workItems)
            {
                var remainingWork = ParseFieldToInt(workItem.Fields[WORK_REMAINING_FIELD]);
                var estimatedWork = ParseFieldToInt(workItem.Fields[ESTIMATED_EFFORT_FIELD]);

                // NOTE: We have at least one corner case here.. What if someone changes a name as part of a revision?
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

                // NOTE: To naive?
                // We add the WorkEffortHistoryItem to the first task we can find with the same name.
                var taskToAddTo = tasks.Where((t) => t.Name == workItem.Title).First();
                taskToAddTo.AddWorkEffortHistoryItem(new WorkEffortHistoryItem(remainingWork, workItem.ChangedDate));
                taskToAddTo.WorkEffortEstimate = estimatedWork;
            }

            return tasks;
        }

        public List<Task> GetAllCurrentTasks()
        {
            return ConvertWorkItemsToTasks(GetAllCurrentWorkItems());
        }

        private WorkItemCollection GetAllCurrentWorkItems()
        {
            var wiqlQuery =
                @"SELECT [Estimated Effort (Scrum v3)], " +
                @"[Work Remaining (Scrum v3)] " +
                @"FROM [WorkItems] " +
                @"WHERE [System.TeamProject] = '" + projectName + "'" +
                @"AND [Work Item Type] = 'Sprint Backlog Task'";
            return workItemStore.Query(wiqlQuery);
        }

        private static int ParseFieldToInt(Field field)
        {
            return field.Value != null ? Int32.Parse(field.Value.ToString()) : 0;
        }
    }
}