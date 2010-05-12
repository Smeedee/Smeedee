using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using APD.DomainModel.ProjectInfo;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;


namespace APD.Integration.PMT.ScrumForTFS.DomainModel.Repositories
{
    public class WorkItemFetcher : IFetchWorkItems
    {
        // Defaulting to the values used by Conchango's Scrum for Team System
        private readonly string WORK_REMAINING_FIELD = "Conchango.TeamSystem.Scrum.WorkRemaining";
        private readonly string ESTIMATED_EFFORT_FIELD = "Conchango.TeamSystem.Scrum.EstimatedEffort";
        private readonly string SPRINT_START_DATE_FIELD = "Conchango.TeamSystem.Scrum.SprintStart";
        private readonly string SPRINT_END_DATE_FIELD = "Conchango.TeamSystem.Scrum.SprintStart";
        
        private readonly TeamFoundationServer tfsServer;
        private readonly WorkItemStore workItemStore;
        private readonly String serverAddress;

        private readonly string projectName;


        public WorkItemFetcher(String serverAddress, String projectName, ICredentials credentials,
                               Dictionary<String, String> configuration)
        {
            this.serverAddress = serverAddress;
            String configValue = "";
            WORK_REMAINING_FIELD = (configuration.TryGetValue("tfswi-remaining-field", out configValue))
                                       ? configValue
                                       : WORK_REMAINING_FIELD;
            ESTIMATED_EFFORT_FIELD = (configuration.TryGetValue("tfswi-estimated-field", out configValue))
                                         ? configValue
                                         : ESTIMATED_EFFORT_FIELD;
            SPRINT_START_DATE_FIELD = (configuration.TryGetValue("tfswi-start-date-field", out configValue))
                                         ? configValue
                                         : SPRINT_START_DATE_FIELD;
            SPRINT_END_DATE_FIELD = (configuration.TryGetValue("tfswi-end-date-field", out configValue))
                                         ? configValue
                                         : SPRINT_END_DATE_FIELD;

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


        public Dictionary<int, String> GetAllSprints()
        {
            var allWorkItems = GetAllSprintsFromTFS();
            var iterationInfo = new Dictionary<int, String>();
            foreach (WorkItem sprint in allWorkItems)
            {
                if (!iterationInfo.Keys.Contains(sprint.Id))
                {
                    iterationInfo.Add(sprint.Id, sprint.IterationPath);
                }
            }
            return iterationInfo;
        }

        public DateTime GetStartDateForIteration(int iterationId)
        {
            var stringDate = workItemStore.GetWorkItem(iterationId).Fields[SPRINT_START_DATE_FIELD].Value as String;
            DateTime startDate = new DateTime();
            DateTime.TryParse(stringDate, out startDate);
            return startDate;
        }

        public DateTime GetEndDateForIteration(int iterationId)
        {
            var stringDate = workItemStore.GetWorkItem(iterationId).Fields[SPRINT_END_DATE_FIELD].Value as String;
            DateTime endDate = new DateTime();
            DateTime.TryParse(stringDate, out endDate);
            return endDate;
        }

        #endregion

        public List<Task> GetCurrentTasksInSprint(string iterationPath)
        {
            return ConvertWorkItemsToTasks(GetCurrentWorkItemsInSprint(iterationPath));
        }


        private WorkItemCollection GetCurrentWorkItemsInSprint(string iterationPath)
        {
            string wiqlQuery =
                @"SELECT [Conchango.TeamSystem.Scrum.EstimatedEffort], " +
                @"[Conchango.TeamSystem.Scrum.WorkRemaining] " +
                @"FROM [WorkItems] " +
                @"WHERE [System.TeamProject] = '" + projectName + "'" +
                @"AND [System.IterationPath] = '" + iterationPath + "'" +
                @"AND [Work Item Type] = 'Sprint Backlog Item'";
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
                @"SELECT [Conchango.TeamSystem.Scrum.EstimatedEffort], " +
                @"[Conchango.TeamSystem.Scrum.WorkRemaining] " +
                @"FROM [WorkItems] " +
                @"WHERE [System.TeamProject] = '" + projectName + @"'" +
                @"AND [Work Item Type] = 'Sprint Backlog Item'";
            return workItemStore.Query(wiqlQuery);
        }

        private WorkItemCollection GetAllSprintsFromTFS()
        {
            var wiqlQuery =
                @"SELECT Id " + 
                @"FROM [WorkItems] " +
                @"WHERE [System.TeamProject] = '" + projectName + @"'" +
                @"AND [Work Item Type] = 'Sprint'";
            return workItemStore.Query(wiqlQuery);
        }


        private static int ParseFieldToInt(Field field)
        {
            return field.Value != null ? Int32.Parse(field.Value.ToString()) : 0;
        }
    }
}