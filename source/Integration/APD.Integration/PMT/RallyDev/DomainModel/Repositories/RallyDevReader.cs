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
using System.Linq;
using System.Collections.Generic;
using APD.DomainModel.Framework;
using APD.DomainModel.ProjectInfo;

//TODO: I (Daniel) will (in Week 34) redesign the plugin. What ever you do here it will probobly be gone next week, so save yourself the effort.
namespace APD.Integration.PMT.RallyDev.DomainModel.Repositories
{
    public class RallyDevReader : IRepository<ProjectInfoServer>
    {
        private readonly string webServiceUrl;
        private readonly IDownloadXml downloader;
        private const int PAGE_SIZE = 200;  //Max 200

        public RallyDevReader(string webServiceUrl, IDownloadXml downloader)
        {
            this.webServiceUrl = webServiceUrl;
            this.downloader = downloader;
        }

        public Project GetProjectWithAllInfo(string projectName)
        {
            var project = new Project();
            project.SystemId = GetProjectId(projectName);

            if (project.SystemId != null)
            {
                project.Name = projectName;
                project.Iterations = GetIterationsForProject(projectName);

                foreach (var iteration in project.Iterations)
                    iteration.Tasks = GetTasksForIteration(iteration.Name);
            }
            else
                throw new NoSuchProjectException(projectName + " does not exist in RallyDev", new Exception());

            return project;
        }

        public List<Iteration> GetIterationsForProject(string projectName)
        {
            List<string> iterationReferences = GetIterationReferences(projectName);

            if (iterationReferences.Count == 0)
                return new List<Iteration>();

            var iterations = new List<Iteration>();
            var iterationParser = new XmlIterationParser();

            foreach (var iterationQuery in iterationReferences)
            {
                iterationParser.LoadXml(downloader.GetXmlDocumentString(iterationQuery));
                iterations.Add(iterationParser.ParseIteration());
            }

            return iterations;
        }

        public List<Task> GetTasksForIteration(string iterationName)
        {
            List<string> taskReferences = GetTaskReferences(iterationName);
            if (taskReferences.Count == 0)
                return new List<Task>();
            
            return GetTasksWithWorkHistoryItems(taskReferences);
        }

        private List<string> GetTaskReferences(string iterationName)
        {
            int start = 1;
            var queryString = webServiceUrl + "Task?query=(Iteration.Name = %22" + iterationName + "%22)&pagesize=" + PAGE_SIZE + "&start=" + start;
            var iterationParser = new XmlIterationParser(downloader.GetXmlDocumentString(queryString));
            var totalReferenceCount = iterationParser.GetTotalResultCount();
            var taskReferences = new List<string>();

            while (start <= totalReferenceCount)
            {
                taskReferences.AddRange(iterationParser.ParseTaskReferencesForIteration());
                if (taskReferences.Count != totalReferenceCount)
                {
                    start += PAGE_SIZE;
                    queryString = webServiceUrl + "Task?query=(Iteration.Name = %22" + iterationName + "%22)&pagesize=" + PAGE_SIZE + "&start=" + start;
                    iterationParser = new XmlIterationParser(downloader.GetXmlDocumentString(queryString));
                }
                else
                    break;
            }

            return taskReferences;
        }

        private List<string> GetIterationReferences(string projectName)
        {
            int start = 1;
            var queryString = string.Format("{0}Iteration?query=(Project.Name = %22{1}%22)&pagesize={2}&start={3}",
                                            webServiceUrl, projectName, PAGE_SIZE, start);
            var projectParser = new XmlProjectParser(downloader.GetXmlDocumentString(queryString));
            var totalReferenceCount = projectParser.GetTotalResultCount();
            var iterationReferences = new List<string>();

            while (start <= totalReferenceCount)
            {
                iterationReferences.AddRange(projectParser.ParseIterationReferencesForProject());

                if (iterationReferences.Count != totalReferenceCount)
                {
                    start += PAGE_SIZE;
                    queryString = string.Format("{0}Iteration?query=(Project.Name = %22{1}%22)&pagesize={2}&start={3}",
                                                webServiceUrl, projectName, PAGE_SIZE, start);
                    projectParser = new XmlProjectParser(downloader.GetXmlDocumentString(queryString));
                }
                else
                    break;
            }

            return iterationReferences;
        }

        private List<Task> GetTasksWithWorkHistoryItems(List<string> taskReferences)
        {
            var tasks = new List<Task>();
            var taskParser = new XmlTaskParser();

            foreach (var taskQuery in taskReferences)
            {
                taskParser.LoadXml(downloader.GetXmlDocumentString(taskQuery));
                var task = taskParser.ParseTaskWithData();
                var workHistoryItems = GetWorkHistoryItemsForTask(taskParser.ParseRevisionHistoryReference());
                workHistoryItems.Sort();

                foreach (WorkEffortHistoryItem workHistoryItem in workHistoryItems)
                    task.AddWorkEffortHistoryItem(workHistoryItem);

                tasks.Add(task);
            }

            return tasks;
        }

        private List<WorkEffortHistoryItem> GetWorkHistoryItemsForTask(string historyReference)
        {
            var taskParser = new XmlTaskParser(downloader.GetXmlDocumentString(historyReference));

            return taskParser.ParseWorkEffortHistoryFromRevisionHistory();
        }


        private string GetProjectId(string projectName)
        {
            string queryString = string.Format("{0}Project?query=(Name = %22{1}%22)",
                                               webServiceUrl, projectName);
            var parser = new XmlProjectParser(downloader.GetXmlDocumentString(queryString));
            queryString = parser.ParseProjectReference();

            if (queryString != null)
            {
                parser.LoadXml(downloader.GetXmlDocumentString(parser.ParseProjectReference()));
                return parser.ParseIdFromReference();
            }

            return null;
        }

        public IEnumerable<ProjectInfoServer> Get(Specification<ProjectInfoServer> specification)
        {
            var queryString = webServiceUrl + "Project?pagesize=" + PAGE_SIZE;
            var projectParser = new XmlProjectParser(downloader.GetXmlDocumentString(queryString));
            var projectNames = projectParser.ParseProjectNames();
            var server = new ProjectInfoServer("RallyDev", "https://community.rallydev.com/slm/rally.sp");

            foreach (var projectName in projectNames)
            {
                server.AddProject(GetProjectWithAllInfo(projectName));
            }

            return new List<ProjectInfoServer> { server };
        }
    }
}